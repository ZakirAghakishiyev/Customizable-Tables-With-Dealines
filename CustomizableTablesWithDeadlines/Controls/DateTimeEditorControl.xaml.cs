using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace CustomizableTablesWithDeadlines.Controls;

public partial class DateTimeEditorControl : UserControl
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(DateTime?),
            typeof(DateTimeEditorControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    public static readonly DependencyProperty ColumnNameProperty =
        DependencyProperty.Register(
            nameof(ColumnName),
            typeof(string),
            typeof(DateTimeEditorControl),
            new PropertyMetadata(null, OnColumnNameChanged));

    private bool _syncing;

    public DateTimeEditorControl()
    {
        InitializeComponent();
        PopulateTimeOptions();
        DatePart.SelectedDateChanged += (_, _) => CommitSelection();
        DataContextChanged += (_, _) => SyncFromSource();
    }

    public DateTime? Value
    {
        get => (DateTime?)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string? ColumnName
    {
        get => (string?)GetValue(ColumnNameProperty);
        set => SetValue(ColumnNameProperty, value);
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DateTimeEditorControl editor && !editor._syncing && string.IsNullOrEmpty(editor.ColumnName))
            editor.ApplyToPickers(editor.Value);
    }

    private static void OnColumnNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DateTimeEditorControl editor)
            editor.SyncFromSource();
    }

    private void PopulateTimeOptions()
    {
        for (var hour = 0; hour < 24; hour++)
        {
            for (var minute = 0; minute < 60; minute += 15)
            {
                var time = new DateTime(1, 1, 1, hour, minute, 0);
                TimePart.Items.Add(time.ToString("t", CultureInfo.CurrentCulture));
            }
        }
    }

    private void SyncFromSource()
    {
        if (_syncing)
            return;

        if (!string.IsNullOrEmpty(ColumnName) && DataContext is DataRowView row)
        {
            ApplyToPickers(ReadRowValue(row, ColumnName));
            return;
        }

        if (string.IsNullOrEmpty(ColumnName))
            ApplyToPickers(Value);
    }

    private void OnTimeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_syncing || TimePart.SelectedItem is null)
            return;

        CommitSelection();
    }

    private void OnTimeLostFocus(object sender, RoutedEventArgs e) =>
        CommitSelection();

    private void CommitSelection()
    {
        if (_syncing)
            return;

        var combined = CombineSelected();

        if (!string.IsNullOrEmpty(ColumnName) && DataContext is DataRowView row)
        {
            _syncing = true;
            try
            {
                row[ColumnName] = combined.HasValue ? combined.Value : DBNull.Value;
            }
            finally
            {
                _syncing = false;
            }

            return;
        }

        Value = combined;
    }

    private DateTime? CombineSelected()
    {
        if (DatePart.SelectedDate is null && string.IsNullOrWhiteSpace(GetTimeText()))
            return null;

        var date = DatePart.SelectedDate ?? DateTime.Today;
        var time = TryParseTime(GetTimeText()) ?? TimeSpan.Zero;
        return date.Date + time;
    }

    private string GetTimeText() =>
        TimePart.SelectedItem as string ?? TimePart.Text;

    private static TimeSpan? TryParseTime(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.NoCurrentDateDefault, out var parsed))
            return parsed.TimeOfDay;

        if (TimeSpan.TryParse(text, CultureInfo.CurrentCulture, out var timeSpan))
            return timeSpan;

        return null;
    }

    private void ApplyToPickers(DateTime? value)
    {
        _syncing = true;
        try
        {
            if (value is null)
            {
                DatePart.SelectedDate = null;
                TimePart.SelectedItem = null;
                TimePart.Text = string.Empty;
                return;
            }

            DatePart.SelectedDate = value.Value.Date;
            var timeText = value.Value.ToString("t", CultureInfo.CurrentCulture);
            var match = TimePart.Items.Cast<object>().FirstOrDefault(item => Equals(item, timeText));
            if (match is not null)
                TimePart.SelectedItem = match;
            else
                TimePart.Text = timeText;
        }
        finally
        {
            _syncing = false;
        }
    }

    private static DateTime? ReadRowValue(DataRowView row, string columnName)
    {
        if (string.IsNullOrEmpty(columnName) || !row.Row.Table.Columns.Contains(columnName))
            return null;

        var raw = row[columnName];
        if (raw == DBNull.Value || raw is null)
            return null;

        return raw is DateTime dt ? dt : Convert.ToDateTime(raw);
    }
}
