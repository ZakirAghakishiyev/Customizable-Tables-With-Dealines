using System.Windows;
using System.Windows.Controls;
using CustomizableTablesWithDeadlines.ViewModels;

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
        InputTextBox.Focus();
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
