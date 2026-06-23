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
}
