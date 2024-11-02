using ClassLibrary;
using ClassLibrary.Entities;
using ClassLibrary.Notifications;
using ClassLibrary.Notifications.Abstract;
using ClassLibrary.Settings.Email;
using Microsoft.Extensions.Configuration;

namespace WinFormsApp;

public partial class MainForm : Form
{
    private readonly Bank _bank;
    private readonly INotificationSender _notificationSender;
    private ATMManager? _currentATMManager;
    private string _currentInput = string.Empty;
    private bool _isAuthenticated = false;
    private string currentState = "SELECT_ATM";
    private string? tempCardNumber;

    public MainForm()
    {
        InitializeComponent();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var emailSettingsProvider = new EmailSettingsProvider(configuration);
        var emailNotificationSender = new EmailNotificationSender(emailSettingsProvider);
        var displayNotificationSender = new DisplayNotificationSender();

        _notificationSender = new MultiNotificationSender(displayNotificationSender, emailNotificationSender);

        _bank = new Bank("StereoBank");
        InitializeBank(_bank, _notificationSender);

        UpdateDisplay($"Welcome to {_bank.Name} ATM System!\nPlease select ATM to continue.");
    }

    private static void InitializeBank(Bank bank, INotificationSender notificationSender)
    {
        InitializeATMs(bank);
        InitializeAccounts(bank, notificationSender);
    }

    private static void InitializeATMs(Bank bank)
    {
        bank.AddATM(new AutomatedTellerMachine("ATM001", "Main Street", 500));
        bank.AddATM(new AutomatedTellerMachine("ATM002", "Park Avenue", 150000));
    }

    private static void InitializeAccounts(Bank bank, INotificationSender notificationSender)
    {
        bank.AddAccount(new Account("John Doe", "mail@gmail.com", "1234", "1234", 1000), notificationSender);
        bank.AddAccount(new Account("Jane Smith", "mail@gmail.com", "4321", "4321", 2000), notificationSender);
        bank.AddAccount(new Account("Bob Johnson", "mail@gmail.com", "1111", "1111", 1500), notificationSender);
    }

    private void ProcessEnter()
    {
        switch (currentState)
        {
            case "SELECT_ATM":
                SelectATM();
                break;

            case "ENTER_CARD":
                tempCardNumber = _currentInput;
                currentState = "ENTER_PIN";
                ClearDisplay();
                UpdateDisplay("Please enter your PIN:");
                break;

            case "ENTER_PIN":
                Authenticate();
                break;

            case "WITHDRAW":
                WithdrawAmount();
                break;

            case "DEPOSIT":
                DepositAmount();
                break;

            case "TRANSFER_AMOUNT":
                TransferAmount();
                break;

            case "TRANSFER_CARD":
                tempCardNumber = _currentInput;
                currentState = "TRANSFER_AMOUNT";
                ClearDisplay();
                UpdateDisplay("Enter amount to transfer:");
                break;
        }

        ClearInput();
    }

    private void SelectATM()
    {
        if (_currentInput == "1")
        {
            _currentATMManager = _bank.CreateATMManager("ATM001", _notificationSender);
        }
        else if (_currentInput == "2")
        {
            _currentATMManager = _bank.CreateATMManager("ATM002", _notificationSender);
        }

        if (_currentATMManager != null)
        {
            _currentATMManager.OnATMEvent += HandleATMEvent;
            currentState = "ENTER_CARD";
            ClearDisplay();
            UpdateDisplay("Please enter your card number:");
        }
    }

    private void ShowMainMenu()
    {
        currentState = "MAIN_MENU";
        if (_currentATMManager != null)
        {
            ClearDisplay();

            _currentATMManager.GreetUser();
            _currentATMManager.GetAvailableCash();

            displayScreen.Text += "\nPlease, choose any option...";
        }
    }

    private void Authenticate()
    {
        if (_currentATMManager!.Authenticate(tempCardNumber!, _currentInput))
        {
            _isAuthenticated = true;
            currentState = "MAIN_MENU";

            ShowMainMenu();
        }
        else
        {
            ClearDisplay();
            UpdateDisplay("Invalid card number or PIN. Please try again.\nEnter card number:");
            currentState = "ENTER_CARD";
        }
    }

    private void Logout()
    {
        if (MessageBox.Show("Are you sure you want to logout?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            ResetATM();
            UpdateDisplay("You have been logged out.\nPlease select ATM to continue.");
        }
    }

    private void FunctionButtonClick(string function)
    {
        if (!_isAuthenticated && function != "SELECT ATM" && function != "EXIT")
        {
            UpdateDisplay("Please select ATM and login first!");
            return;
        }

        switch (function)
        {
            case "SELECT ATM":
                ResetATM();
                break;
            case "LOGOUT":
                Logout();
                break;
            case "CHECK BALANCE":
                ClearDisplay();
                _currentATMManager!.CheckBalance();
                break;
            case "WITHDRAW":
                currentState = "WITHDRAW";
                ClearDisplay();
                UpdateDisplay("Enter amount to withdraw:");
                break;
            case "DEPOSIT":
                currentState = "DEPOSIT";
                ClearDisplay();
                UpdateDisplay("Enter amount to deposit:");
                break;
            case "TRANSFER":
                currentState = "TRANSFER_CARD";
                ClearDisplay();
                UpdateDisplay("Enter recipient's card number:");
                break;
            case "EXIT":
                if (MessageBox.Show("Are you sure you want to exit?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Exit();
                }
                break;
        }
    }

    private void ResetATM()
    {
        currentState = "SELECT_ATM";
        _isAuthenticated = false;
        _currentATMManager = null;
        ClearDisplay();
        UpdateDisplay("Select ATM:\n1. ATM001 - Main Street\n2. ATM002 - Park Avenue");
    }

    private void WithdrawAmount()
    {
        if (decimal.TryParse(_currentInput, out decimal withdrawAmount))
        {
            _currentATMManager!.Withdraw(withdrawAmount);

        }
        currentState = "MAIN_MENU";
    }

    private void DepositAmount()
    {
        if (decimal.TryParse(_currentInput, out decimal depositAmount))
        {
            _currentATMManager!.Deposit(depositAmount);

        }
        currentState = "MAIN_MENU";
    }

    private void TransferAmount()
    {
        if (decimal.TryParse(_currentInput, out decimal transferAmount))
        {
            _currentATMManager!.Transfer(tempCardNumber!, transferAmount);

        }
        currentState = "MAIN_MENU";
    }

    private void NumericButtonClick(string number)
    {
        _currentInput += number;

        if (currentState == "ENTER_PIN")
        {
            inputDisplay.UseSystemPasswordChar = true;
            inputDisplay.Text = new string('*', _currentInput.Length);
        }
        else
        {
            inputDisplay.UseSystemPasswordChar = false;
            inputDisplay.Text = _currentInput;
        }
    }

    private void HandleATMEvent(string message)
    {
        if (displayScreen.InvokeRequired)
        {
            displayScreen.Invoke(new Action(() => UpdateDisplay(message)));
        }
        else
        {
            UpdateDisplay(message);
        }
    }

    private void UpdateDisplay(string message)
    {
        if (displayScreen.Text == string.Empty)
        {
            displayScreen.Text = message;
        }
        else
        {
            displayScreen.Text += "\n" + message;
        }
    }

    private void ClearDisplay()
    {
        displayScreen.Text = string.Empty;
    }

    private void ClearInput()
    {
        _currentInput = string.Empty;
        inputDisplay.Text = string.Empty;
    }
}
