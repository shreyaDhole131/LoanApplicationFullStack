using LoanApp_API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanApp_API.Application.Interfaces
{
    public interface INotificationService
    {
        Task GenerateEmiNotifications();

        List<Notification> SendCustomerNotifications(int customerId);

        int FetchUnreadCount(int customerId);

        Task<bool> MarkAsRead(int notificationId, int customerId);
    }
}
