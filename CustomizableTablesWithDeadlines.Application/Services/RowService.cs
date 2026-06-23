using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.DTOs.Rows;
using CustomizableTablesWithDeadlines.Application.Exceptions;
using CustomizableTablesWithDeadlines.Application.Validators;
using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Services;

public class RowService : IRowService
{
    private readonly IUnitOfWork _unitOfWork;

    public RowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateAsync(CreateRowDto dto, CancellationToken cancellationToken = default)
    {
        var table = await _unitOfWork.Tables.GetTableWithRowsAndCellsAsync(dto.TableId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Table), dto.TableId);

        var orderNumber = table.Rows.Count == 0 ? 1 : table.Rows.Max(r => r.OrderNumber) + 1;
        var row = new Row
        {
            TableId = table.Id,
            OrderNumber = orderNumber
        };

        await _unitOfWork.Rows.AddAsync(row, cancellationToken);

        foreach (var column in table.Columns)
        {
            await _unitOfWork.CellValues.AddAsync(new CellValue
            {
                Row = row,
                ColumnId = column.Id
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return row.Id;
    }

    public async Task DeleteAsync(int rowId, CancellationToken cancellationToken = default)
    {
        var row = await _unitOfWork.Rows.GetByIdAsync(rowId, cancellationToken)
                  ?? throw new NotFoundException(nameof(Row), rowId);

        _unitOfWork.Rows.Delete(row);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCellValueAsync(UpdateCellValueDto dto, CancellationToken cancellationToken = default)
    {
        var column = await _unitOfWork.Columns.GetByIdAsync(dto.ColumnId, cancellationToken)
                     ?? throw new NotFoundException(nameof(Column), dto.ColumnId);

        var cells = await _unitOfWork.CellValues.FindAsync(
            c => c.RowId == dto.RowId && c.ColumnId == dto.ColumnId,
            cancellationToken);

        var cell = cells.FirstOrDefault();
        if (cell is null)
        {
            cell = new CellValue
            {
                RowId = dto.RowId,
                ColumnId = dto.ColumnId
            };
            CellValueValidator.ApplyValue(cell, column.DataType, dto);
            await _unitOfWork.CellValues.AddAsync(cell, cancellationToken);
        }
        else
        {
            CellValueValidator.ApplyValue(cell, column.DataType, dto);
            _unitOfWork.CellValues.Update(cell);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
