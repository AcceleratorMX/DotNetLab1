using ClassLibrary.Notifications.Abstract;

namespace ClassLibrary.Entities;

public class Bank(string name)
{
    public string Name { get; } = name;
    public List<AutomatedTellerMachine> ATMs { get; } = [];
    private Dictionary<string, Account> Accounts { get; } = [];

    public void AddATM(AutomatedTellerMachine atm)
    {
        ATMs.Add(atm);
    }

    public void AddAccount(Account account, INotificationSender notificationSender)
    {
        Accounts[account.CardNumber] = account;
        account.NotificationSender = notificationSender;
    }

    public Account? GetAccount(string cardNumber)
    {
        return Accounts.TryGetValue(cardNumber, out var account) ? account : null;
    }

    public ATMManager CreateATMManager(string atmId, INotificationSender notificationSender)
    {
        var atm = ATMs.Find(a => a.Id == atmId);
        return atm != null ? new ATMManager(this, atm, notificationSender) : throw new ArgumentException($"ATM with id {atmId} not found.");
    }
}
