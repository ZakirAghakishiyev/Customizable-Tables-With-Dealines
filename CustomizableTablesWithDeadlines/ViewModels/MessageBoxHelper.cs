using System.Windows;

namespace CustomizableTablesWithDeadlines.ViewModels;

public static class MessageBoxHelper
{
    public static bool Confirm(string message) =>
        MessageBox.Show(message, string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public static void ShowError(string message) =>
        MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);

    public static void ShowWarning(string message) =>
        MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Warning);

    public static void ShowInfo(string message) =>
        MessageBox.Show(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
}
