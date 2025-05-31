using API.Database;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, 
        IMapper mapper, IHubContext<PresenceHub> presenceHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["user"];

            if (Context.User is null || string.IsNullOrEmpty(otherUser)) 
                throw new Exception("Cannot join group");

            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser!);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages); 
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User?.GetUserName() ?? throw new Exception("Could not get user");

            if (username == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You can not message yourself");

            var sender = await userRepository.GetUserByUsernameAsync(username);

            var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername.ToLower());

            if (sender is null || recipient is null || sender.UserName is null || recipient.UserName is null)
                throw new HubException("Can not send message");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = username,
                Content = createMessageDto.Content,
                RecipientUsername = recipient.UserName
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await messageRepository.GetMessageGroup(groupName);

            if(group is not null && group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DataRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                if(connections is not null && connections?.Count is not null)
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs }); 
                }
            }
            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
            }
            
        }
        private async Task<Group> AddToGroup(string groupName)
        {
            var userName = Context.User?.GetUserName() ?? throw new Exception("Could not get user");
            var group = await messageRepository.GetMessageGroup(groupName);
            var connection = new Connection { ConnectionId = Context.ConnectionId, Username = userName };
            if(group is null)
            {
                group = new Group { Name = groupName };
                messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);

            if (await messageRepository.SaveAllAsync())
                return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if(connection is not null && group is not null)
            {
                messageRepository.RemoveConnection(connection);
                if (await messageRepository.SaveAllAsync()) return group;
            }

            throw new Exception("Failed to remove from group");
        }

        private string GetGroupName(string caller, string? other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
