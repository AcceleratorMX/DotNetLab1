﻿using ClassLibrary.Entities;
using ClassLibrary.Notifications;
using ClassLibrary.Notifications.Abstract;

namespace ClassLibrary;

public class ATMManager
{
    private readonly Bank bank = null!;
    private readonly AutomatedTellerMachine atm = null!;
    private Account currentAccount = null!;
    private readonly INotificationSender notificationSender = null!;

    public delegate void ATMEventHandler(string message);
    public event ATMEventHandler OnATMEvent = null!;

    public ATMManager(Bank bank, AutomatedTellerMachine atm, INotificationSender notificationSender)
    {
        this.bank = bank;
        this.atm = atm;
        this.notificationSender = notificationSender;
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        atm.OnAuthentication += (sender, args) => HandleATMEvent(args.Message, false);
        atm.OnUserGreeting += (sender, args) => HandleATMEvent(args.Message, false);
        atm.OnGetAvailableCash += (sender, args) => HandleATMEvent(args.Message, false);
        atm.OnBalanceCheck += (sender, args) => HandleATMEvent(args.Message, false);

        atm.OnWithdrawal += (sender, args) => HandleATMEvent(args.Message, true);
        atm.OnDeposit += (sender, args) => HandleATMEvent(args.Message, true);
        atm.OnTransfer += (sender, args) => HandleATMEvent(args.Message, true);
    }

    public bool Authenticate(string cardNumber, string pin)
    {
        var account = bank.GetAccount(cardNumber);
        if (account == null) return false;

        currentAccount = account;
        return atm.Authenticate(currentAccount, pin);
    }

    public string GreetUser() =>
        currentAccount != null ? atm.GreetUser(currentAccount) : throw new InvalidOperationException("Користувач не автентифікований.");

    public decimal CheckBalance() =>
        currentAccount != null ? atm.CheckBalance(currentAccount) : throw new InvalidOperationException("Користувач не автентифікований.");

    public bool Withdraw(decimal amount) =>
        currentAccount != null ? atm.Withdraw(currentAccount, amount) : throw new InvalidOperationException("Користувач не автентифікований.");

    public bool Deposit(decimal amount) =>
        currentAccount != null ? atm.Deposit(currentAccount, amount) : throw new InvalidOperationException("Користувач не автентифікований.");

    public bool Transfer(string toCardNumber, decimal amount)
    {
        if (currentAccount == null)
            throw new InvalidOperationException("Користувач не автентифікований.");

        var toAccount = bank.GetAccount(toCardNumber);
        if (toAccount == null) return false;

        return atm.Transfer(currentAccount, toAccount, amount);
    }


    private void HandleATMEvent(string message, bool sendEmail)
    {
        OnATMEvent?.Invoke(message);
        if (currentAccount?.Email != null)
        {
            notificationSender.Send(new ATMNotification(message, currentAccount.Email, sendEmail));
        }
    }
}