namespace ClassLibrary.Settings;

public class ATMEventArgs(string message) : EventArgs
{
    public string Message { get; set; } = message;
}
