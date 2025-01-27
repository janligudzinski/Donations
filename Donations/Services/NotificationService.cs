using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Donations.Entities.User;
using Donations.Database;

namespace Donations.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotification(Guid userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Date = DateTime.Now,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotifications(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Date)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCount(Guid userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllAsRead(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearAll(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();
        }
    }
}