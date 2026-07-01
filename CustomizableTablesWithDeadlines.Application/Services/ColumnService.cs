using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.DTOs.Columns;
using CustomizableTablesWithDeadlines.Application.Exceptions;
using CustomizableTablesWithDeadlines.Application.Validators;
using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Services;

public class ColumnService : IColumnService
{
    private readonly IUnitOfWork _unitOfWork;

    public ColumnService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateAsync(CreateColumnDto dto, CancellationToken cancellationToken = default)
    {
        ColumnValidator.ValidateName(dto.Name);

        var table = await _unitOfWork.Tables.GetTableWithRowsAndCellsAsync(dto.TableId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Table), dto.TableId);

        ColumnValidator.EnsureUniqueName(dto.Name, table.Columns.Select(c => c.Name));

        var orderIndex = table.Columns.Count == 0 ? 0 : table.Columns.Max(c => c.OrderIndex) + 1;
        var column = new Column
        {
            TableId = table.Id,
            Name = dto.Name.Trim(),
            DataType = dto.DataType,
            OrderIndex = orderIndex,
            IsRequired = dto.IsRequired
        };

        await _unitOfWork.Columns.AddAsync(column, cancellationToken);

        foreach (var row in table.Rows)
        {
            await _unitOfWork.CellValues.AddAsync(new CellValue
            {
                Row = row,
                Column = column
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return column.Id;
    }

    public async Task RenameAsync(RenameColumnDto dto, CancellationToken cancellationToken = default)
    {
        ColumnValidator.ValidateName(dto.Name);

        var column = await _unitOfWork.Columns.GetByIdAsync(dto.Id, cancellationToken)
                     ?? throw new NotFoundException(nameof(Column), dto.Id);

        var table = await _unitOfWork.Tables.GetTableWithColumnsAsync(column.TableId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Table), column.TableId);

        ColumnValidator.EnsureUniqueName(
            dto.Name,
            table.Columns.Where(c => c.Id != column.Id).Select(c => c.Name));

        column.Name = dto.Name.Trim();
        _unitOfWork.Columns.Update(column);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeDataTypeAsync(ChangeColumnDataTypeDto dto, CancellationToken cancellationToken = default)
    {
        var column = await _unitOfWork.Columns.GetByIdAsync(dto.Id, cancellationToken)
                     ?? throw new NotFoundException(nameof(Column), dto.Id);

        column.DataType = dto.DataType;
        _unitOfWork.Columns.Update(column);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int columnId, CancellationToken cancellationToken = default)
    {
        var column = await _unitOfWork.Columns.GetByIdAsync(columnId, cancellationToken)
                     ?? throw new NotFoundException(nameof(Column), columnId);

        _unitOfWork.Columns.Delete(column);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ReorderAsync(List<ReorderColumnDto> columns, CancellationToken cancellationToken = default)
    {
        if (columns.Count == 0)
            return;

        foreach (var item in columns)
        {
            var column = await _unitOfWork.Columns.GetByIdAsync(item.Id, cancellationToken)
                         ?? throw new NotFoundException(nameof(Column), item.Id);

            column.OrderIndex = item.OrderIndex;
            _unitOfWork.Columns.Update(column);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
