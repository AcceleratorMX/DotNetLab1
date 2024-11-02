using ClassLibrary.Notifications.Abstract;

namespace ClassLibrary.Entities;

public class Account
{
    
    public string FullName { get; }
    public string Email { get; set; }
    public string CardNumber { get; }
    public string Pin { get; }
    public decimal Balance { get; private set; }
    
    public INotificationSender NotificationSender { get; internal set; } = null!;

    public Account(string fullName, string email, string cardNumber, string pin, decimal balance)
    {
        FullName = fullName;
        Email = email;
        CardNumber = cardNumber;
        Pin = pin;
        Balance = balance;
    }

    public bool UpdateBalance(decimal amount)
    {
        if (Balance + amount < 0)
            return false;

        Balance += amount;
        return true;
    }
}