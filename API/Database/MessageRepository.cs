using API.DTOs;
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
            var messages = await context.Messages
                            .Include(x => x.Sender).ThenInclude(x => x.Photos)
                            .Include(r => r.Recipient).ThenInclude(r => r.Photos)
                            .Where(x => (x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername && x.RecipientDeleted == false) ||
                                    (x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername && x.SenderDeleted == false))
                            .OrderBy(x => x.MessageSent)
                            .ToListAsync();
            var unreadMessages = messages.Where(x => x.DataRead == null && x.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Count > 0)
            {
                unreadMessages.ForEach(x => x.DataRead = DateTime.UtcNow);
                await context.SaveChangesAsync();
            }

            return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
