﻿using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Database
{
    public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
    {
        public void RemoveConnection(Connection connection)
        {
            context.Connections.Remove(connection);
        }
        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await context.Connections.FindAsync(connectionId);
        }

        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
        }
        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await context.Groups
                    .Include(x => x.Connections)
                    .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message?> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages
                            .OrderBy (q => q.MessageSent)
                            .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username && x.SenderDeleted == false),
                _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DataRead == null
                    && x.RecipientDeleted == false)
            };

            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var query = context.Messages
                            .Where(x => (x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername && x.RecipientDeleted == false) ||
                                    (x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername && x.SenderDeleted == false))
                            .OrderBy(x => x.MessageSent)
                            .AsQueryable();

            var unreadMessages = query.Where(x => x.DataRead == null && x.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Count > 0)
            {
                unreadMessages.ForEach(x => x.DataRead = DateTime.UtcNow);
            }

            return await query.ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                            .ToListAsync(); ; 
        }

        public async Task<Group?> GetGroupForConnection(string connectionId)
        {
            return await context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
    }
}
