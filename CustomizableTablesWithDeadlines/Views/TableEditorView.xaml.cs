using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CustomizableTablesWithDeadlines.Controls;
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
        {
            _viewModel.GridStructureChanged -= RebuildColumns;
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = e.NewValue as TableEditorViewModel;
        if (_viewModel is not null)
        {
            _viewModel.GridStructureChanged += RebuildColumns;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        RebuildColumns();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TableEditorViewModel.ShowDedicatedDeadlineColumn))
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
            IsReadOnly = true,
            Width = DataGridLength.Auto
        });

        foreach (var col in _viewModel.Columns.OrderBy(c => c.Order))
        {
            DataGridColumn column = col.Type switch
            {
                ColumnType.Boolean => new DataGridCheckBoxColumn
                {
                    Header = col.Name,
                    Binding = new Binding(col.Id.ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                    MinWidth = 110,
                    Width = DataGridLength.Auto
                },
                ColumnType.DateTime => new DataGridTemplateColumn
                {
                    Header = col.Name,
                    CellTemplate = CreateDateTimeCellTemplate(col.Id.ToString()),
                    MinWidth = 260,
                    Width = DataGridLength.Auto
                },
                _ => new DataGridTextColumn
                {
                    Header = col.Name,
                    Binding = new Binding(col.Id.ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                    MinWidth = 110,
                    Width = DataGridLength.Auto
                }
            };

            DataGrid.Columns.Add(column);
        }

        if (_viewModel.ShowDedicatedDeadlineColumn)
        {
            DataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = _viewModel.Strings.DeadlineDateTime,
                Binding = new Binding("_NextDeadline") { StringFormat = "g" },
                IsReadOnly = true,
                MinWidth = 160,
                Width = DataGridLength.Auto
            });
        }

        DataGrid.Columns.Add(new DataGridTemplateColumn
        {
            Header = _viewModel.Strings.Actions,
            CellTemplate = (DataTemplate)Resources["RowActionsTemplate"],
            MinWidth = 240,
            Width = DataGridLength.Auto
        });
    }

    private static DataTemplate CreateDateTimeCellTemplate(string columnName)
    {
        var template = new DataTemplate();
        var factory = new FrameworkElementFactory(typeof(DateTimeEditorControl));
        factory.SetValue(DateTimeEditorControl.ColumnNameProperty, columnName);
        template.VisualTree = factory;
        return template;
    }
}
