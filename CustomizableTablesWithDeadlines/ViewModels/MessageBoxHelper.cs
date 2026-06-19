using System.Windows;

namespace CustomizableTablesWithDeadlines.ViewModels;

public static class MessageBoxHelper
{
    public static bool Confirm(string message) =>
        MessageBox.Show(message, string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
}
