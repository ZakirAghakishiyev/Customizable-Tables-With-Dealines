using System.Windows;

namespace CustomizableTablesWithDeadlines;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.GetService<ViewModels.MainViewModel>();
    }
}
