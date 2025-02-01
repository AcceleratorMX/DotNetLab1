using ClassLibrary;
using ClassLibrary.Entities;
using ClassLibrary.Notifications;
using ClassLibrary.Notifications.Abstract;
using ClassLibrary.Settings.Email;
using Microsoft.Extensions.Configuration;
using ConsoleApp.Services.BankService;

string atmFilePath = Path.Combine("data", "atms.json");
string accountFilePath = Path.Combine("data", "accounts.json");
string bankName = "StereoBank";

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var emailSettingsProvider = new EmailSettingsProvider(configuration);
var emailNotificationSender = new EmailNotificationSender(emailSettingsProvider);
var displayNotificationSender = new DisplayNotificationSender();
var multiNotificationSender = new MultiNotificationSender(displayNotificationSender, emailNotificationSender);

var bank = new Bank(bankName);
var bankService = new BankService(atmFilePath, accountFilePath, multiNotificationSender);
bankService.InitializeBank(bank);

while (true)
{
    Console.Clear();
    Console.WriteLine($"Welcome to {bank.Name} ATM System!");
    Console.WriteLine($"{MenuOption.SelectATM}. Select ATM");
    Console.WriteLine($"{MenuOption.Exit}. Exit");
    Console.Write("Choose an option: ");

    switch (Console.ReadLine())
    {
        case "1":
            SelectATM(bank, multiNotificationSender);
            break;
        case "2":
            Console.WriteLine("Goodbye!");
            return;
        default:
            Console.WriteLine("Invalid option! Press any key to continue...");
            Console.ReadKey();
            break;
    }
}

static void SelectATM(Bank bank, INotificationSender notificationSender)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("Select an ATM:");
        Console.WriteLine($"{ATMIdentifiers.ATM001} - Main Street");
        Console.WriteLine($"{ATMIdentifiers.ATM002} - Park Avenue");
        Console.WriteLine($"{MenuOption.BackToMainMenu}. Back to main menu");
        Console.Write("Choose an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                RunATM(bank.CreateATMManager(ATMIdentifiers.ATM001, notificationSender));
                break;
            case "2":
                RunATM(bank.CreateATMManager(ATMIdentifiers.ATM002, notificationSender));
                break;
            case "3":
                return;
            default:
                Console.WriteLine("Invalid option! Press any key to continue...");
                Console.ReadKey();
                break;
        }
    }
}

static void RunATM(ATMManager atmManager)
{
    atmManager.OnATMEvent += DisplayNotifications;

    while (true)
    {
        Console.Clear();
        Console.WriteLine("Please enter your card number (or 'q' to go back):");
        string cardNumber = Console.ReadLine()!;

        if (cardNumber!.Equals("q", StringComparison.CurrentCultureIgnoreCase))
            return;

        Console.WriteLine("Enter your PIN:");
        string pin = Console.ReadLine()!;

        if (atmManager.Authenticate(cardNumber, pin))
        {
            RunATMMenu(atmManager);
        }
        else
        {
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
        }
    }
}

static void RunATMMenu(ATMManager atmManager)
{
    while (true)
    {
        Console.Clear();
        atmManager.GreetUser();
        ShowATMMenu();

        string choice = Console.ReadLine()!;
        if (choice == MenuOption.Logout.ToString()) return;

        HandleATMAction(choice, atmManager);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}

static void ShowATMMenu()
{
    Console.WriteLine($"\n{ATMMenuOption.CheckBalance}. Check Balance");
    Console.WriteLine($"{ATMMenuOption.Withdraw}. Withdraw");
    Console.WriteLine($"{ATMMenuOption.Deposit}. Deposit");
    Console.WriteLine($"{ATMMenuOption.Transfer}. Transfer");
    Console.WriteLine($"{ATMMenuOption.Logout}. Logout");
    Console.Write("Choose an option: ");
}

static void HandleATMAction(string choice, ATMManager atmManager)
{
    switch (choice)
    {
        case "1":
            atmManager.CheckBalance();
            break;
        case "2":
            PerformTransaction(atmManager, TransactionType.Withdraw);
            break;
        case "3":
            PerformTransaction(atmManager, TransactionType.Deposit);
            break;
        case "4":
            PerformTransfer(atmManager);
            break;
        default:
            Console.WriteLine("Invalid option!");
            break;
    }
}

static void PerformTransaction(ATMManager atmManager, TransactionType transactionType)
{
    decimal? amount = GetAmountFromUser($"Enter amount to {transactionType.ToString().ToLower()}: ");
    if (amount.HasValue)
    {
        switch (transactionType)
        {
            case TransactionType.Withdraw:
                atmManager.Withdraw(amount.Value);
                break;
            case TransactionType.Deposit:
                atmManager.Deposit(amount.Value);
                break;
        }
    }
}

static void PerformTransfer(ATMManager atmManager)
{
    Console.Write("Enter recipient's card number: ");
    string toCardNumber = Console.ReadLine()!;

    decimal? amount = GetAmountFromUser("Enter amount to transfer: ");
    if (amount.HasValue)
    {
        atmManager.Transfer(toCardNumber, amount.Value);
    }
}

static decimal? GetAmountFromUser(string prompt)
{
    Console.Write(prompt);
    if (decimal.TryParse(Console.ReadLine(), out decimal amount))
    {
        return amount;
    }
    else
    {
        Console.WriteLine("Invalid amount entered!");
        return null;
    }
}

static void DisplayNotifications(string message)
{
    Console.WriteLine(message);
}

public static class MenuOption
{
    public const int SelectATM = 1;
    public const int Exit = 2;
    public const int BackToMainMenu = 3;
    public const int Logout = 5;
}

public static class ATMIdentifiers
{
    public const string ATM001 = "ATM001";
    public const string ATM002 = "ATM002";
}

public enum ATMMenuOption
{
    CheckBalance = 1,
    Withdraw = 2,
    Deposit = 3,
    Transfer = 4,
    Logout = 5
}

public enum TransactionType
{
    Withdraw,
    Deposit,
    Transfer
}
