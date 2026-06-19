using System.Windows;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.ViewModels;

namespace CustomizableTablesWithDeadlines.Views.Dialogs;

public partial class ImportWordPreviewDialog : Window
{
    private readonly ImportWordPreviewViewModel _viewModel;

    public CustomTable? Result => _viewModel.Result;

    public ImportWordPreviewDialog(ImportWordPreviewViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void Import_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.TableName))
            return;

        _viewModel.ImportCommand.Execute(null);
        if (_viewModel.Result is not null)
        {
            DialogResult = true;
            Close();
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
