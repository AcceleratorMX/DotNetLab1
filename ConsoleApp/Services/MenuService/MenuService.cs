using ClassLibrary;
using ClassLibrary.Entities;
using ClassLibrary.Notifications.Abstract;
using ConsoleApp.Constants;
using ConsoleApp.Enums;
using ConsoleApp.Extensions;

namespace ConsoleApp.Services.MenuService;

public static class MenuService
{
    public static void RunMainMenu(Bank bank, INotificationSender notificationSender)
    {
        var actions = new Dictionary<MainMenuOption, Action>
        {
            { MainMenuOption.SelectAtm, () => RunATMSelectionMenu(bank, notificationSender) },
            { MainMenuOption.Exit, () => { return; } }
        };

        var exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine(MenuConstants.WelcomeToBank, bank.Name);

            GetMenuOptions<MainMenuOption>(x => x.GetDescription());

            Console.Write(MenuConstants.ChooseOption);

            if (Enum.TryParse(Console.ReadLine(), out MainMenuOption choice) &&
                Enum.IsDefined(typeof(MainMenuOption), choice))
            {
                if (choice == MainMenuOption.Exit)
                {
                    exit = true;
                    Console.WriteLine(MenuConstants.Goodbye);
                }

                actions.TryGetValue(choice, out var action);
                action?.Invoke();
            }
            else
            {
                Console.WriteLine(MenuConstants.InvalidOption);
                Console.ReadKey();
            }
        }
    }
    
    private static void RunATMSelectionMenu(Bank bank, INotificationSender notificationSender)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine(MenuConstants.SelectAtm);

            GetATMs(bank);

            Console.WriteLine(MenuConstants.BackToMainMenu, bank.ATMs.Count + 1);
            Console.Write(MenuConstants.ChooseOption);

            if (int.TryParse(Console.ReadLine(), out var choice) && choice > 0 && choice <= bank.ATMs.Count + 1)
            {
                if (choice == bank.ATMs.Count + 1) return;

                RunATM(bank.CreateATMManager(bank.ATMs[choice - 1].Id, notificationSender));
            }
            else
            {
                Console.WriteLine(MenuConstants.InvalidOption);
                Console.ReadKey();
            }
        }
    }
    
    private static void RunATM(ATMManager atmManager)
    {
        const string exitKey = "q";
        
        atmManager.OnATMEvent += DisplayNotifications;

        while (true)
        {
            Console.Clear();
            Console.WriteLine(MenuConstants.EnterCerdNumberOrQToBack);
            var cardNumber = Console.ReadLine()!;

            if (cardNumber!.Equals(exitKey, StringComparison.CurrentCultureIgnoreCase))
                return;

            Console.WriteLine(MenuConstants.EnterEnterPin);
            var pin = Console.ReadLine()!;

            if (atmManager.Authenticate(cardNumber, pin))
            {
                RunATMMenu(atmManager);
            }
            else
            {
                Console.WriteLine(MenuConstants.InvalidCardNumberOrPin);
                Console.ReadKey();
            }
        }
    }

    private static void RunATMMenu(ATMManager atmManager)
    {
        var actions = new Dictionary<AtmMenuOption, Action>
        {
            { AtmMenuOption.CheckBalance, () => atmManager.CheckBalance() },
            { AtmMenuOption.Withdraw, () => PerformWithdrawal(atmManager) },
            { AtmMenuOption.Deposit, () => PerformDeposit(atmManager) },
            { AtmMenuOption.Transfer, () => PerformTransfer(atmManager) },
            { AtmMenuOption.Logout, () => { return; } }
        };

        while (true)
        {
            Console.Clear();
            atmManager.GreetUser();

            GetMenuOptions<AtmMenuOption>(x => x.GetDescription());

            Console.Write(MenuConstants.ChooseOption);

            if (Enum.TryParse(Console.ReadLine(), out AtmMenuOption choice) &&
                Enum.IsDefined(typeof(AtmMenuOption), choice))
            {
                actions.TryGetValue(choice, out var action);
                action?.Invoke();
            }
            else
            {
                Console.WriteLine(MenuConstants.InvalidOption);
            }
            
            if (choice == AtmMenuOption.Logout) return;
            Console.WriteLine(MenuConstants.AnyKeyToContinue);
            Console.ReadKey();
        }
    }
    
    private static void GetATMs(Bank bank)
    {
        for (var i = 0; i < bank.ATMs.Count; i++)
        {
            var atm = bank.ATMs[i];
            Console.WriteLine(MenuConstants.CashAvailable, i + 1, atm.Location, atm.AvailableCash);
        }
    }

    private static void GetMenuOptions<T>(Func<T, string> getDescription) where T : Enum
    {
        foreach (T option in Enum.GetValues(typeof(T)))
        {
            Console.WriteLine(MenuConstants.MenuOptions, (int)(object)option, getDescription(option));
        }
    }

    private static void PerformWithdrawal(ATMManager atmManager)
    {
        atmManager.GetAvailableCash();
        var amount = GetAmountFromUser(MenuConstants.EnterAmountToWithdraw);
        atmManager.Withdraw(amount!.Value);
    }

    private static void PerformDeposit(ATMManager atmManager)
    {
        var amount = GetAmountFromUser(MenuConstants.EnterAmountToDeposit);
        atmManager.Deposit(amount!.Value);
    }

    private static void PerformTransfer(ATMManager atmManager)
    {
        Console.Write(MenuConstants.EnterRecipientCardNumber);
        var toCardNumber = Console.ReadLine()!;

        var amount = GetAmountFromUser(MenuConstants.EnterAmountToTransfer);
        atmManager.Transfer(toCardNumber, amount!.Value);
    }

    private static decimal? GetAmountFromUser(string prompt)
    {
        Console.Write(prompt);
        if (decimal.TryParse(Console.ReadLine(), out var amount))
        {
            return amount;
        }
        else
        {
            Console.WriteLine(MenuConstants.InvalidAmountEntered);
            return null;
        }
    }

    private static void DisplayNotifications(string message)
    {
        Console.WriteLine(message);
    }
}