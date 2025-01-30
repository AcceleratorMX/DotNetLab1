using System;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        var bank = new Bank("StereoBank");
        var notificationSender = new MultiNotificationSender(new DisplayNotificationSender(), new EmailNotificationSender());

        // Ініціалізація банку
        InitializeBank(bank, notificationSender);

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
                    SelectATM(bank, notificationSender);
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
    }

    static void InitializeBank(Bank bank, INotificationSender notificationSender)
    {
        // Ініціалізація банкоматів
        bank.AddATM(new AutomatedTellerMachine("ATM001", "Main Street", 500));
        bank.AddATM(new AutomatedTellerMachine("ATM002", "Park Avenue", 150000));

        // Ініціалізація акаунтів
        bank.AddAccount(new Account("John Doe", "mail@gmail.com", "1234", 1000), notificationSender);
        bank.AddAccount(new Account("Jane Smith", "mail@gmail.com", "4321", 2000), notificationSender);
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

            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    TryRunATM(bank, "ATM001", notificationSender);
                    break;
                case "2":
                    TryRunATM(bank, "ATM002", notificationSender);
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

    static void TryRunATM(Bank bank, string atmId, INotificationSender notificationSender)
    {
        // Отримуємо ATMManager і перевіряємо, чи існує банкомат
        var atmManager = bank.CreateATMManager(atmId, notificationSender);

        if (atmManager != null)
        {
            RunATM(atmManager);
        }
        else
        {
            Console.WriteLine($"⚠ Error: ATM with ID '{atmId}' not found. Please select a valid ATM.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    static void RunATM(ATMManager atmManager)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Please enter your card number (or 'q' to go back):");
            string cardNumber = Console.ReadLine()!;

            if (cardNumber.Equals("q", StringComparison.CurrentCultureIgnoreCase))
                return;

            Console.WriteLine("Enter your PIN:");
            string pin = Console.ReadLine()!;

            if (atmManager.Authenticate(cardNumber, pin))
            {
                RunATMMenu(atmManager);
            }
            else
            {
                Console.WriteLine("Invalid card number or PIN. Press any key to try again...");
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
            Console.WriteLine("4. Logout");
            Console.Write("Choose an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    atmManager.CheckBalance();
                    break;
                case "2":
                    Console.WriteLine("Withdrawal feature coming soon...");
                    break;
                case "3":
                    Console.WriteLine("Deposit feature coming soon...");
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option! Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}

public class Bank
{
    public string Name { get; }
    private List<AutomatedTellerMachine> ATMs = new();
    private List<Account> Accounts = new();

    public Bank(string name) => Name = name;

    public void AddATM(AutomatedTellerMachine atm) => ATMs.Add(atm);

    public void AddAccount(Account account, INotificationSender notificationSender)
    {
        Accounts.Add(account);
        notificationSender.SendNotification($"Account created for {account.Owner}.");
    }

    public ATMManager? CreateATMManager(string atmId, INotificationSender notificationSender)
    {
        var atm = ATMs.Find(a => a.Id == atmId);
        return atm != null ? new ATMManager(atm, notificationSender) : null;
    }
}

public class AutomatedTellerMachine
{
    public string Id { get; }
    public string Location { get; }
    public decimal CashAvailable { get; private set; }

    public AutomatedTellerMachine(string id, string location, decimal cashAvailable)
    {
        Id = id;
        Location = location;
        CashAvailable = cashAvailable;
    }
}

public class Account
{
    public string Owner { get; }
    public string Email { get; }
    public string Pin { get; }
    public decimal Balance { get; private set; }

    public Account(string owner, string email, string pin, decimal balance)
    {
        Owner = owner;
        Email = email;
        Pin = pin;
        Balance = balance;
    }
}

public interface INotificationSender
{
    void SendNotification(string message);
}

public class MultiNotificationSender : INotificationSender
{
    private readonly List<INotificationSender> Senders = new();

    public MultiNotificationSender(params INotificationSender[] senders) => Senders.AddRange(senders);

    public void SendNotification(string message)
    {
        foreach (var sender in Senders)
            sender.SendNotification(message);
    }
}

public class DisplayNotificationSender : INotificationSender
{
    public void SendNotification(string message) => Console.WriteLine($"[DISPLAY]: {message}");
}

public class EmailNotificationSender : INotificationSender
{
    public void SendNotification(string message) => Console.WriteLine($"[EMAIL]: {message}");
}

public class ATMManager
{
    private readonly AutomatedTellerMachine ATM;
    private readonly INotificationSender NotificationSender;

    public ATMManager(AutomatedTellerMachine atm, INotificationSender notificationSender)
    {
        ATM = atm;
        NotificationSender = notificationSender;
    }

    public bool Authenticate(string cardNumber, string pin) => true;

    public void GreetUser() => Console.WriteLine("Welcome!");

    public void CheckBalance() => Console.WriteLine("Your balance is not available in this demo.");
}
