using ConsoleApp.Enums;

namespace ConsoleApp.Extensions;

public static class AtmMenuOptionExtensions
{
    public static string GetDescription(this AtmMenuOption option)
    {
        return option switch
        {
            AtmMenuOption.CheckBalance => "Check Balance",
            AtmMenuOption.Withdraw => "Withdraw",
            AtmMenuOption.Deposit => "Deposit",
            AtmMenuOption.Transfer => "Transfer",
            AtmMenuOption.Logout => "Logout",
            _ => "Unknown Option"
        };
    }
}