﻿using ClassLibrary;
using ClassLibrary.Entities;
using ClassLibrary.Notifications;
using ClassLibrary.Notifications.Abstract;
using ClassLibrary.Settings.Email;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var emailSettingsProvider = new EmailSettingsProvider(configuration);
var emailNotificationSender = new EmailNotificationSender(emailSettingsProvider);
var displayNotificationSender = new DisplayNotificationSender();
var multiNotificationSender = new MultiNotificationSender(displayNotificationSender, emailNotificationSender);

var bank = new Bank("StereoBank");
InitializeBank(bank, multiNotificationSender);

static void InitializeBank(Bank bank, INotificationSender notificationSender)
{
    InitializeATMs(bank);
    InitializeAccounts(bank, notificationSender);
}

static void InitializeATMs(Bank bank)
{
    bank.AddATM(new AutomatedTellerMachine("ATM001", "Main Street", 500));
    bank.AddATM(new AutomatedTellerMachine("ATM002", "Park Avenue", 150000));
}

static void InitializeAccounts(Bank bank, INotificationSender notificationSender)
{
    bank.AddAccount(new Account("John Doe", "mail@gmail.com", "1234", "1234", 1000), notificationSender);
    bank.AddAccount(new Account("Jane Smith", "mail@gmail.com", "4321", "4321", 2000), notificationSender);
    bank.AddAccount(new Account("Bob Johnson", "mail@gmail.com", "1111", "1111", 1500), notificationSender);
}

while (true)
{
    Console.Clear();
    Console.WriteLine($"Welcome to {bank.Name} ATM System!");
    Console.WriteLine("1. Select ATM");
    Console.WriteLine("2. Exit");
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
        Console.WriteLine("1. ATM001 - Main Street");
        Console.WriteLine("2. ATM002 - Park Avenue");
        Console.WriteLine("3. Back to main menu");
        Console.Write("Choose an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                RunATM(bank.CreateATMManager("ATM001", notificationSender));
                break;
            case "2":
                RunATM(bank.CreateATMManager("ATM002", notificationSender));
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
        Console.WriteLine("\n1. Check Balance");
        Console.WriteLine("2. Withdraw");
        Console.WriteLine("3. Deposit");
        Console.WriteLine("4. Transfer");
        Console.WriteLine("5. Logout");
        Console.Write("Choose an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                atmManager.CheckBalance();
                break;
            case "2":
                PerformWithdrawal(atmManager);
                break;
            case "3":
                PerformDeposit(atmManager);
                break;
            case "4":
                PerformTransfer(atmManager);
                break;
            case "5":
                return;
            default:
                Console.WriteLine("Invalid option!");
                break;
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}

static void PerformWithdrawal(ATMManager atmManager)
{
    atmManager.GetAvailableCash();
    decimal? amount = GetAmountFromUser("Enter amount to withdraw: ");
    atmManager.Withdraw(amount!.Value);
}

static void PerformDeposit(ATMManager atmManager)
{
    decimal? amount = GetAmountFromUser("Enter amount to deposit: ");
    atmManager.Deposit(amount!.Value);
}

static void PerformTransfer(ATMManager atmManager)
{
    Console.Write("Enter recipient's card number: ");
    string toCardNumber = Console.ReadLine()!;

    decimal? amount = GetAmountFromUser("Enter amount to transfer: ");
    atmManager.Transfer(toCardNumber, amount!.Value);
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
