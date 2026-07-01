using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

    private static DateTimeEditorControl? _activeEditor;

    private bool _syncing;
    private bool _isEditing;

    public DateTimeEditorControl()
    {
        InitializeComponent();
        PopulateHourOptions();
        PopulateMinuteOptions();
        DatePart.SelectedDateChanged += (_, _) => OnEditorValueChanged();
        DataContextChanged += OnDataContextChanged;
        Loaded += (_, _) => SyncFromSource();
        Unloaded += OnUnloaded;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ResetDisplayMode();
        SyncFromSource();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_activeEditor == this)
            _activeEditor = null;

        ResetDisplayMode();
    }

    private void ResetDisplayMode()
    {
        _isEditing = false;
        DisplayText.Visibility = Visibility.Visible;
        EditorPanel.Visibility = Visibility.Collapsed;
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

    public static void CommitActiveEdit()
    {
        _activeEditor?.EndEdit();
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

    private void PopulateHourOptions()
    {
        for (var hour = 0; hour < 24; hour++)
            HourPart.Items.Add(hour.ToString("00", CultureInfo.InvariantCulture));
    }

    private void PopulateMinuteOptions()
    {
        for (var minute = 0; minute < 60; minute++)
            MinutePart.Items.Add(minute.ToString("00", CultureInfo.InvariantCulture));
    }

    private void SyncFromSource()
    {
        if (_syncing)
            return;

        var value = ReadCurrentValue();
        if (!_isEditing)
            ApplyToPickers(value);

        UpdateDisplayText(value);
    }

    private void OnDisplayClick(object sender, MouseButtonEventArgs e)
    {
        BeginEdit();
    }

    private void BeginEdit()
    {
        if (_isEditing)
            return;

        if (_activeEditor is not null && _activeEditor != this)
            _activeEditor.EndEdit();

        _activeEditor = this;
        _isEditing = true;
        DisplayText.Visibility = Visibility.Collapsed;
        EditorPanel.Visibility = Visibility.Visible;
        ApplyToPickers(ReadCurrentValue());
        Focus();
        DatePart.Focus();
    }

    public void EndEdit()
    {
        if (!_isEditing)
            return;

        CommitSelection();
        _isEditing = false;
        DisplayText.Visibility = Visibility.Visible;
        EditorPanel.Visibility = Visibility.Collapsed;
        UpdateDisplayText(ReadCurrentValue());

        if (_activeEditor == this)
            _activeEditor = null;
    }

    private void OnRootLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (!_isEditing)
            return;

        if (DatePart.IsDropDownOpen || HourPart.IsDropDownOpen || MinutePart.IsDropDownOpen)
            return;

        if (e.NewFocus is DependencyObject focus && IsDescendantOf(this, focus))
            return;

        EndEdit();
    }

    private void OnEditorValueChanged()
    {
        if (_syncing || !_isEditing)
            return;

        EnsureDefaultTimeSelections();
        CommitSelection();
        UpdateDisplayText(ReadCurrentValue());
    }

    private void EnsureDefaultTimeSelections()
    {
        if (DatePart.SelectedDate is null)
            return;

        if (HourPart.SelectedItem is null)
            HourPart.SelectedItem = "00";

        if (MinutePart.SelectedItem is null)
            MinutePart.SelectedItem = "00";
    }

    private void OnTimePartChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_syncing || !_isEditing)
            return;

        if (HourPart.SelectedItem is null || MinutePart.SelectedItem is null)
            return;

        CommitSelection();
        UpdateDisplayText(ReadCurrentValue());
    }

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
        if (DatePart.SelectedDate is null)
            return null;

        var hour = ParseSelectedNumber(HourPart) ?? 0;
        var minute = ParseSelectedNumber(MinutePart) ?? 0;
        return DatePart.SelectedDate.Value.Date.AddHours(hour).AddMinutes(minute);
    }

    private static int? ParseSelectedNumber(ComboBox comboBox)
    {
        var text = comboBox.SelectedItem as string ?? comboBox.Text;
        return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }

    private void ApplyToPickers(DateTime? value)
    {
        _syncing = true;
        try
        {
            if (value is null)
            {
                DatePart.SelectedDate = null;
                HourPart.SelectedItem = null;
                MinutePart.SelectedItem = null;
                return;
            }

            DatePart.SelectedDate = value.Value.Date;
            var hourText = value.Value.Hour.ToString("00", CultureInfo.InvariantCulture);
            var minuteText = value.Value.Minute.ToString("00", CultureInfo.InvariantCulture);
            HourPart.SelectedItem = HourPart.Items.Cast<object>().FirstOrDefault(item => Equals(item, hourText));
            MinutePart.SelectedItem = MinutePart.Items.Cast<object>().FirstOrDefault(item => Equals(item, minuteText));
        }
        finally
        {
            _syncing = false;
        }
    }

    private void UpdateDisplayText(DateTime? value = null)
    {
        value ??= ReadCurrentValue();

        if (value is null)
        {
            DisplayText.Text = string.Empty;
            DisplayText.Foreground = (Brush)FindResource("TextSecondaryBrush");
            return;
        }

        DisplayText.Text = value.Value.ToString("g", CultureInfo.CurrentCulture);
        DisplayText.Foreground = (Brush)FindResource("TextPrimaryBrush");
        ToolTip = DisplayText.Text;
    }

    private DateTime? ReadCurrentValue()
    {
        if (!string.IsNullOrEmpty(ColumnName) && DataContext is DataRowView row)
            return ReadRowValue(row, ColumnName);

        return Value;
    }

    private static DateTime? ReadRowValue(DataRowView row, string columnName)
    {
        if (string.IsNullOrEmpty(columnName) || !row.Row.Table.Columns.Contains(columnName))
            return null;

        var raw = row[columnName];
        if (raw == DBNull.Value || raw is null)
            return null;

        return raw is DateTime dt ? dt : Convert.ToDateTime(raw, CultureInfo.CurrentCulture);
    }

    private static bool IsDescendantOf(DependencyObject ancestor, DependencyObject? node)
    {
        while (node is not null)
        {
            if (node == ancestor)
                return true;

            node = node is Visual
                ? VisualTreeHelper.GetParent(node)
                : LogicalTreeHelper.GetParent(node);
        }

        return false;
    }
}
