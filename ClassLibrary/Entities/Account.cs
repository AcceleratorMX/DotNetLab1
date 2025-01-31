using ClassLibrary.Notifications.Abstract;
using System;

namespace ClassLibrary.Entities
{
    public class Account
    {
        public string FullName { get; }
        public string Email { get; private set; }
        public string CardNumber { get; }
        private readonly string _hashedPin;
        public decimal Balance { get; private set; }

        public INotificationSender NotificationSender { get; }

        public interface INotificationSender
        {
            void Send(string message);
        }

        public Account(string fullName, string email, string cardNumber, string pin, decimal balance, INotificationSender notificationSender)
        {
            FullName = fullName;
            Email = email;
            CardNumber = cardNumber;
            _hashedPin = HashPin(pin);
            Balance = balance;
            NotificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
        }

        public bool UpdateBalance(decimal amount)
        {
            if (Balance + amount < 0)
            {
                NotificationSender.Send($"Недостатньо коштів на рахунку {CardNumber}. Поточний баланс: {Balance}");
                return false;
            }

            Balance += amount;
            NotificationSender.Send($"Баланс успішно оновлено. Новий баланс: {Balance}");
            return true;
        }

        private string HashPin(string pin)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(pin)));
        }
    }
}
