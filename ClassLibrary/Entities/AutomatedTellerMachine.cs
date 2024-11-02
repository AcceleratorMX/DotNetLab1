using ClassLibrary.Settings;

namespace ClassLibrary.Entities;

public class AutomatedTellerMachine
{
    public string Id { get; } = string.Empty;
    public string Location { get; } = string.Empty;
    public decimal AvailableCash { get; private set; } = 0;

    public event EventHandler<ATMEventArgs> OnAuthentication = null!;
    public event EventHandler<ATMEventArgs> OnUserGreeting = null!;
    public event EventHandler<ATMEventArgs> OnGetAvailableCash = null!;
    public event EventHandler<ATMEventArgs> OnBalanceCheck = null!;
    public event EventHandler<ATMEventArgs> OnWithdrawal = null!;
    public event EventHandler<ATMEventArgs> OnDeposit = null!;
    public event EventHandler<ATMEventArgs> OnTransfer = null!;

    public AutomatedTellerMachine(string id, string location, decimal availableCash)
    {
        Id = id;
        Location = location;
        AvailableCash = availableCash;
    }

    public bool Authenticate(Account account, string pin)
    {
        var isAuthenticated = account.Pin == pin;
        RaiseEvent(OnAuthentication,
            isAuthenticated ? $"Authentication successful for card {account.CardNumber}!"
                          : $"Authentication failed for card {account.CardNumber}!");
        return isAuthenticated;
    }

    public string GreetUser(Account account)
    {
        RaiseEvent(OnUserGreeting, $"Welcome, {account.FullName}!");
        return account.FullName;
    }

    public decimal GetAvailableCash()
    {
        RaiseEvent(OnGetAvailableCash, $"Available cash in ATM: ${AvailableCash}.");
        return AvailableCash;
    }

    public decimal CheckBalance(Account account)
    {
        RaiseEvent(OnBalanceCheck, $"Current balance for card {account.CardNumber}: ${account.Balance}.");
        return account.Balance;
    }

    public bool Withdraw(Account account, decimal amount)
    {
        if (amount <= 0)
        {
            RaiseEvent(OnWithdrawal, $"Deposit failed: Invalid amount ${amount}!");
            return false;
        }

        if (amount > AvailableCash)
        {
            RaiseEvent(OnWithdrawal, $"Withdrawal failed: Insufficient funds in ATM! Available: ${AvailableCash}.");
            return false;
        }

        if (amount > account.Balance)
        {
            RaiseEvent(OnWithdrawal, $"Withdrawal failed: Insufficient funds in account! Available: ${account.Balance}.");
            return false;
        }

        if (account.UpdateBalance(-amount))
        {
            AvailableCash -= amount;
            RaiseEvent(OnWithdrawal, $"Successfully withdrawn ${amount} from card {account.CardNumber}.");
            return true;
        }

        RaiseEvent(OnWithdrawal, $"Withdrawal failed for card {account.CardNumber}!");
        return false;
    }

    public bool Deposit(Account account, decimal amount)
    {
        if (amount <= 0)
        {
            RaiseEvent(OnDeposit, $"Deposit failed: Invalid amount ${amount}!");
            return false;
        }

        if (account.UpdateBalance(amount))
        {
            AvailableCash += amount;
            RaiseEvent(OnDeposit, $"Successfully deposited ${amount} to card {account.CardNumber}.");
            return true;
        }

        RaiseEvent(OnDeposit, $"Deposit failed for card {account.CardNumber}!");
        return false;
    }

    public bool Transfer(Account fromAccount, Account toAccount, decimal amount)
    {
        if (amount <= 0)
        {
            RaiseEvent(OnTransfer, $"Transfer failed: Invalid amount ${amount}.");
            return false;
        }

        if (amount > fromAccount.Balance)
        {
            RaiseEvent(OnTransfer, $"Transfer failed: Insufficient funds! Requested: ${amount}, Available: ${fromAccount.Balance}.");
            return false;
        }

        if (fromAccount.UpdateBalance(-amount) && toAccount.UpdateBalance(amount))
        {
            RaiseEvent(OnTransfer, $"Successfully transferred {amount} from card {fromAccount.CardNumber} to card {toAccount.CardNumber}!");
            return true;
        }

        RaiseEvent(OnTransfer, $"Transfer failed between cards {fromAccount.CardNumber} and {toAccount.CardNumber}!");
        return false;
    }

    private void RaiseEvent(EventHandler<ATMEventArgs> eventHandler, string message)
    {
        eventHandler?.Invoke(this, new ATMEventArgs(message));
    }
}