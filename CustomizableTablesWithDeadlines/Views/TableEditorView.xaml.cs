using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CustomizableTablesWithDeadlines.Models.Enums;
using CustomizableTablesWithDeadlines.ViewModels;

namespace CustomizableTablesWithDeadlines.Views;

public partial class TableEditorView : UserControl
{
    private TableEditorViewModel? _viewModel;

    public TableEditorView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_viewModel is not null)
            _viewModel.GridStructureChanged -= RebuildColumns;

        _viewModel = e.NewValue as TableEditorViewModel;
        if (_viewModel is not null)
            _viewModel.GridStructureChanged += RebuildColumns;

        RebuildColumns();
    }

    private void RebuildColumns()
    {
        if (_viewModel is null)
            return;

        DataGrid.Columns.Clear();

        DataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "ID",
            Binding = new Binding("_RowId"),
            Visibility = Visibility.Collapsed,
            IsReadOnly = true
        });

        foreach (var col in _viewModel.Columns.OrderBy(c => c.Order))
        {
            DataGridColumn column = col.Type switch
            {
                ColumnType.Boolean => new DataGridCheckBoxColumn
                {
                    Header = col.Name,
                    Binding = new Binding(col.Id.ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                },
                ColumnType.DateTime => new DataGridTextColumn
                {
                    Header = col.Name,
                    Binding = new Binding(col.Id.ToString())
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        StringFormat = "g"
                    }
                },
                _ => new DataGridTextColumn
                {
                    Header = col.Name,
                    Binding = new Binding(col.Id.ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                }
            };

            DataGrid.Columns.Add(column);
        }

        var actionsColumn = new DataGridTemplateColumn
        {
            Header = _viewModel.Strings.Actions,
            Width = 220,
            CellTemplate = CreateActionsTemplate()
        };
        DataGrid.Columns.Add(actionsColumn);
    }

    private DataTemplate CreateActionsTemplate()
    {
        var template = new DataTemplate();
        var panel = new FrameworkElementFactory(typeof(StackPanel));
        panel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

        var manageBtn = new FrameworkElementFactory(typeof(Button));
        manageBtn.SetValue(Button.ContentProperty, _viewModel?.Strings.ManageDeadlines ?? "Manage");
        manageBtn.SetResourceReference(Button.StyleProperty, "SecondaryButton");
        manageBtn.SetValue(Button.MarginProperty, new Thickness(0, 0, 8, 0));
        manageBtn.SetBinding(Button.CommandProperty, new Binding(nameof(TableEditorViewModel.ManageDeadlinesCommand)));
        manageBtn.SetBinding(Button.CommandParameterProperty, new Binding("_RowId"));

        var deleteBtn = new FrameworkElementFactory(typeof(Button));
        deleteBtn.SetValue(Button.ContentProperty, _viewModel?.Strings.DeleteRow ?? "Delete");
        deleteBtn.SetResourceReference(Button.StyleProperty, "DangerButton");
        deleteBtn.SetBinding(Button.CommandProperty, new Binding(nameof(TableEditorViewModel.DeleteRowCommand)));
        deleteBtn.SetBinding(Button.CommandParameterProperty, new Binding("_RowId"));

        panel.AppendChild(manageBtn);
        panel.AppendChild(deleteBtn);
        template.VisualTree = panel;
        return template;
    }
}
