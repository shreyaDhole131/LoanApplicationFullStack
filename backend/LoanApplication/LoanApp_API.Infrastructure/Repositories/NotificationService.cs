using LoanApp_API.Application.Interfaces;
using LoanApp_API.Domain.Entities;
using LoanApp_API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Infrastructure.Repositories
{
    public class NotificationService : INotificationService
    {
        AppDbContext db;
        public NotificationService(AppDbContext db)
        {
            this.db = db;
        }
        public int FetchUnreadCount(int customerId)
        {
            return db.Notifications
                     .Count(x =>
                        x.CustomerId == customerId
                        &&
                        x.IsRead == false);
        }


        public async Task GenerateEmiNotifications()
        {
            var today = DateTime.Today;

            var emiSchedules = await db.EmiSchedules
                .Include(x => x.LoanAccount)
                .Where(x => x.PaymentStatus == "Pending")
                .ToListAsync();

            foreach (var emi in emiSchedules)
            {
                int daysLeft = (emi.DueDate.Date - today).Days;

                string reminder = daysLeft switch
                {
                    3 => "is due in 3 days",
                    2 => "is due in 2 days",
                    1 => "is due tomorrow",
                    0 => "is due today",
                    _ => ""
                };

                if (string.IsNullOrEmpty(reminder))
                    continue;

                int customerId = emi.LoanAccount.CustomerId;

                string message =
                    $"Your EMI installment {emi.InstallmentNo} " +
                    $"{reminder}, on {emi.DueDate:dd MMM yyyy}.";

                bool alreadyExists = await db.Notifications
                    .AnyAsync(x =>
                        x.CustomerId == customerId &&
                        x.Message == message &&
                        x.CreatedAt >= today &&
                        x.CreatedAt < today.AddDays(1));

                if (alreadyExists)
                    continue;

                var notification = new Notification
                {
                    CustomerId = customerId,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                db.Notifications.Add(notification);
            }

            await db.SaveChangesAsync();
        }


        public async Task<bool> MarkAsRead(
    int notificationId, int customerId)
        {
            var notification = await db.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notificationId &&
                    x.CustomerId == customerId);

            if (notification == null)
                return false;

            notification.IsRead = true;

            await db.SaveChangesAsync();

            return true;
        }

        public List<Notification> SendCustomerNotifications(int customerId)
        {
            return db.Notifications
                     .Where(x =>
                        x.CustomerId == customerId
                        &&
                        x.IsRead == false)
                     .OrderByDescending(x => x.CreatedAt)
                     .Take(10)
                     .ToList();
        }
    }
}
