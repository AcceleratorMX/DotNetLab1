using ClassLibrary;
using ClassLibrary.Entities;
using ClassLibrary.Notifications;
using ClassLibrary.Notifications.Abstract;
using ClassLibrary.Settings.Email;
using Microsoft.Extensions.Configuration;
using ConsoleApp.Services.BankService;
using ConsoleApp.Services.MenuService;

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

MenuService.RunMainMenu(bank, multiNotificationSender);
