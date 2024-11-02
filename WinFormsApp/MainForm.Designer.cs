using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp;

partial class MainForm
{
    private RichTextBox displayScreen;
    private TextBox inputDisplay;

    private void InitializeComponent()
    {
        this.Size = new Size(800, 600);
        this.Text = "ATM";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;

        displayScreen = new RichTextBox
        {
            Location = new Point(200, 20),
            Size = new Size(400, 200),
            ReadOnly = true,
            BackColor = Color.FromArgb(200, 255, 200),
            Font = new Font("Consolas", 12),
            Multiline = true
        };
        this.Controls.Add(displayScreen);

        inputDisplay = new TextBox
        {
            Location = new Point(200, 230),
            Size = new Size(400, 30),
            ReadOnly = true,
            Font = new Font("Consolas", 12),
            UseSystemPasswordChar = true
        };
        this.Controls.Add(inputDisplay);

        var keypadStartX = 300;
        var keypadStartY = 280;
        for (int i = 1; i <= 9; i++)
        {
            var button = new Button
            {
                Text = i.ToString(),
                Size = new Size(60, 60),
                Location = new Point(
                    keypadStartX + ((i - 1) % 3) * (60 + 5),
                    keypadStartY + ((i - 1) / 3) * (60 + 5)
                ),
                Font = new Font("Arial", 14, FontStyle.Bold)
            };
            button.Click += (s, e) => NumericButtonClick(button.Text);
            this.Controls.Add(button);
        }

        var zeroButton = new Button
        {
            Text = "0",
            Size = new Size(60, 60),
            Location = new Point(keypadStartX + 60 + 5, keypadStartY + 3 * (60 + 5)),
            Font = new Font("Arial", 14, FontStyle.Bold)
        };
        zeroButton.Click += (s, e) => NumericButtonClick("0");
        this.Controls.Add(zeroButton);

        var enterButton = new Button
        {
            Text = "Enter",
            Size = new Size(130, 60),
            Location = new Point(500, 475),
            Font = new Font("Arial", 14, FontStyle.Bold),
            BackColor = Color.FromArgb(50, 150, 50)
        };
        enterButton.Click += (s, e) => ProcessEnter();
        this.Controls.Add(enterButton);

        var clearButton = new Button
        {
            Text = "Clear",
            Size = new Size(130, 60),
            Location = new Point(160, 475),
            Font = new Font("Arial", 14, FontStyle.Bold),
            BackColor = Color.FromArgb(200, 50, 50)
        };
        clearButton.Click += (s, e) => ClearInput();
        this.Controls.Add(clearButton);

        var leftFunctions = new[] { "SELECT ATM", "LOGOUT", "EXIT" };
        for (int i = 0; i < leftFunctions.Length; i++)
        {
            var button = new Button
            {
                Text = leftFunctions[i],
                Size = new Size(130, 40),
                Location = new Point(30, 20 + i * 50),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            button.Click += (s, e) => FunctionButtonClick(button.Text);
            this.Controls.Add(button);
        }

        var rightFunctions = new[] { "CHECK BALANCE", "WITHDRAW", "DEPOSIT", "TRANSFER" };
        for (int i = 0; i < rightFunctions.Length; i++)
        {
            var button = new Button
            {
                Text = rightFunctions[i],
                Size = new Size(130, 40),
                Location = new Point(640, 20 + i * 50),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            button.Click += (s, e) => FunctionButtonClick(button.Text);
            this.Controls.Add(button);
        }
    }
}