using ClassLibrary.Entities;
using ClassLibrary.Notifications.Abstract;
using ConsoleApp.Services.Storage;

namespace ConsoleApp.Services.BankService;

public class BankService(string atmFilePath, string accountFilePath, INotificationSender notificationSender)
{
    public void InitializeBank(Bank bank)
    {
        InitializeATMs(bank);
        InitializeAccounts(bank);
    }

    private void InitializeATMs(Bank bank)
    {
        var atms = JsonService.LoadFromJson<AutomatedTellerMachine>(atmFilePath);
        foreach (var atm in atms)
        {
            bank.AddATM(atm);
        }
    }

    private void InitializeAccounts(Bank bank)
    {
        var accounts = JsonService.LoadFromJson<Account>(accountFilePath);
        foreach (var account in accounts)
        {
            bank.AddAccount(account, notificationSender);
        }
    }
}
