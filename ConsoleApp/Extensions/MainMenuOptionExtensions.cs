using ConsoleApp.Enums;

namespace ConsoleApp.Extensions;

public static class MainMenuOptionExtensions
{
    public static string GetDescription(this MainMenuOption option)
    {
        return option switch
        {
            MainMenuOption.SelectAtm => "Select ATM",
            MainMenuOption.Exit => "Exit",
            _ => "Unknown Option"
        };
    }
}