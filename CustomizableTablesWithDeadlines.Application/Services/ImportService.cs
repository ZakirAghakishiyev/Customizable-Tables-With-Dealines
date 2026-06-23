using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.Common;
using CustomizableTablesWithDeadlines.Application.DTOs.Import;
using CustomizableTablesWithDeadlines.Application.DTOs.Settings;
using CustomizableTablesWithDeadlines.Application.Exceptions;
using CustomizableTablesWithDeadlines.Application.Validators;
using CustomizableTablesWithDeadlines.Domain.Entities;
using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Application.Services;

public class ImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWordImportService _wordImportService;
    private readonly ISettingsService _settingsService;
    private readonly INotificationScheduler _notificationScheduler;

    public ImportService(
        IUnitOfWork unitOfWork,
        IWordImportService wordImportService,
        ISettingsService settingsService,
        INotificationScheduler notificationScheduler)
    {
        _unitOfWork = unitOfWork;
        _wordImportService = wordImportService;
        _settingsService = settingsService;
        _notificationScheduler = notificationScheduler;
    }

    public async Task<List<DetectedWordTableDto>> DetectWordTablesAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var tables = await _wordImportService.DetectTablesAsync(filePath, cancellationToken);
        return tables.ToList();
    }

    public async Task<int> ImportWordTableAsync(
        ImportTableRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var parsed = await _wordImportService.ImportTableAsync(
            dto.FilePath,
            dto.TableIndex,
            dto.FirstRowAsHeader,
            cancellationToken);

        var tableName = string.IsNullOrWhiteSpace(dto.TableName)
            ? parsed.SuggestedTableName
            : dto.TableName;
        TableValidator.ValidateName(tableName);

        var table = new Table { Name = tableName.Trim() };
        await _unitOfWork.Tables.AddAsync(table, cancellationToken);

        var columns = new List<Column>();
        for (var i = 0; i < parsed.ColumnNames.Count; i++)
        {
            var columnValues = parsed.Rows
                .Where(r => i < r.Count)
                .Select(r => r[i])
                .ToList();

            var column = new Column
            {
                Table = table,
                Name = parsed.ColumnNames[i],
                DataType = CellValueValidator.DetectDataType(columnValues),
                OrderIndex = i
            };
            columns.Add(column);
            await _unitOfWork.Columns.AddAsync(column, cancellationToken);
        }

        var settings = await _settingsService.GetAsync(cancellationToken);
        var deadlineColumnSet = new HashSet<int>(dto.DateTimeColumnIndices);

        for (var rowIndex = 0; rowIndex < parsed.Rows.Count; rowIndex++)
        {
            var rowData = parsed.Rows[rowIndex];
            var row = new Row
            {
                Table = table,
                OrderNumber = rowIndex + 1
            };
            await _unitOfWork.Rows.AddAsync(row, cancellationToken);

            for (var colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                var rawValue = colIndex < rowData.Count ? rowData[colIndex] : string.Empty;
                var cell = new CellValue
                {
                    Row = row,
                    Column = columns[colIndex]
                };

                try
                {
                    CellValueValidator.SetFromString(cell, columns[colIndex].DataType, rawValue);
                }
                catch (InvalidCellValueException)
                {
                    cell.ValueText = rawValue;
                    columns[colIndex].DataType = ColumnDataType.Text;
                }

                await _unitOfWork.CellValues.AddAsync(cell, cancellationToken);

                if (dto.CreateDeadlinesFromDateColumns
                    && deadlineColumnSet.Contains(colIndex)
                    && columns[colIndex].DataType == ColumnDataType.DateTime
                    && cell.ValueDateTime.HasValue)
                {
                    var deadline = new Deadline
                    {
                        Row = row,
                        Title = columns[colIndex].Name,
                        DeadlineDateTime = cell.ValueDateTime.Value
                    };
                    deadline.NotificationRules.Add(new NotificationRule
                    {
                        NotifyBeforeMinutes = settings.DefaultNotifyBeforeMinutes,
                        IsEnabled = true
                    });
                    await _unitOfWork.Deadlines.AddAsync(deadline, cancellationToken);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (dto.CreateDeadlinesFromDateColumns)
            await _notificationScheduler.ScheduleAllFutureNotificationsAsync(cancellationToken);

        return table.Id;
    }
}
