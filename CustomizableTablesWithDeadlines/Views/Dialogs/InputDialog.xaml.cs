using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomizableTablesWithDeadlines.Views.Dialogs;

public partial class InputDialog : Window
{
    public string InputText => InputTextBox.Text;

    public InputDialog(string title, string prompt, string defaultValue = "")
    {
        InitializeComponent();
        Title = title;
        DataContext = this;
        TitleText.Text = title;
        PromptText.Text = prompt;
        InputTextBox.Text = defaultValue;
        InputTextBox.SelectAll();
        Loaded += (_, _) =>
        {
            InputTextBox.Focus();
            Keyboard.Focus(InputTextBox);
        };
    }

    public static bool TryPrompt(string title, string prompt, out string value, string defaultValue = "")
    {
        value = string.Empty;
        var dialog = new InputDialog(title, prompt, defaultValue)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(dialog.InputText))
            return false;

        value = dialog.InputText.Trim();
        return true;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
