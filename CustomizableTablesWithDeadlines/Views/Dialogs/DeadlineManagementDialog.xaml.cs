using System.Windows;
using CustomizableTablesWithDeadlines.ViewModels;

namespace CustomizableTablesWithDeadlines.Views.Dialogs;

public partial class DeadlineManagementDialog : Window
{
    public DeadlineManagementDialog(DeadlineManagementViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is DeadlineManagementViewModel vm)
            vm.SaveAndCloseCommand.Execute(this);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
