using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        DataGrid.PreviewMouseDown += OnDataGridPreviewMouseDown;
    }

    private void OnDataGridPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not DependencyObject source)
            return;

        var node = source;
        while (node is not null)
        {
            if (node is DateTimeEditorControl)
                return;

            node = System.Windows.Media.VisualTreeHelper.GetParent(node)
                   ?? System.Windows.LogicalTreeHelper.GetParent(node);
        }

        DateTimeEditorControl.CommitActiveEdit();
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

        var cellTextStyle = (Style)FindResource("DataGridCellText");

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
                    MinWidth = 80,
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    ElementStyle = CreateCheckBoxCellStyle()
                },
                ColumnType.DateTime => new DataGridTemplateColumn
                {
                    Header = col.Name,
                    CellTemplate = CreateDateTimeCellTemplate(col.Id.ToString()),
                    MinWidth = 180,
                    Width = new DataGridLength(1.2, DataGridLengthUnitType.Star)
                },
                _ => new DataGridTextColumn
                {
                    Header = col.Name,
                    Binding = new Binding(col.Id.ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged },
                    MinWidth = 100,
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    ElementStyle = cellTextStyle
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
                MinWidth = 140,
                Width = new DataGridLength(1.2, DataGridLengthUnitType.Star),
                ElementStyle = cellTextStyle
            });
        }

        DataGrid.Columns.Add(new DataGridTemplateColumn
        {
            Header = _viewModel.Strings.Actions,
            CellTemplate = (DataTemplate)Resources["RowActionsTemplate"],
            MinWidth = 96,
            Width = 96,
            CanUserResize = false
        });
    }

    private static DataTemplate CreateDateTimeCellTemplate(string columnName)
    {
        var template = new DataTemplate();
        var factory = new FrameworkElementFactory(typeof(DateTimeEditorControl));
        factory.SetValue(DateTimeEditorControl.ColumnNameProperty, columnName);
        factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
        template.VisualTree = factory;
        return template;
    }

    private static Style CreateCheckBoxCellStyle()
    {
        var style = new Style(typeof(CheckBox));
        style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));
        style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
        style.Setters.Add(new Setter(MarginProperty, new Thickness(0)));
        return style;
    }
}
