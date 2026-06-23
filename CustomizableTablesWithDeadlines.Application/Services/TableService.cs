using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.Common;
using CustomizableTablesWithDeadlines.Application.DTOs.Tables;
using CustomizableTablesWithDeadlines.Application.Exceptions;
using CustomizableTablesWithDeadlines.Application.Validators;
using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Services;

public class TableService : ITableService
{
    private readonly IUnitOfWork _unitOfWork;

    public TableService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TableDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tables = await _unitOfWork.Tables.GetAllAsync(cancellationToken);
        var result = new List<TableDto>();

        foreach (var table in tables)
        {
            var detailed = await _unitOfWork.Tables.GetTableWithRowsAndCellsAsync(table.Id, cancellationToken);
            if (detailed is not null)
                result.Add(detailed.ToDto());
        }

        return result;
    }

    public async Task<TableDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var table = await _unitOfWork.Tables.GetFullTableAsync(id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Table), id);

        return table.ToDetailsDto();
    }

    public async Task<int> CreateAsync(CreateTableDto dto, CancellationToken cancellationToken = default)
    {
        TableValidator.ValidateName(dto.Name);

        var table = new Table { Name = dto.Name.Trim() };
        await _unitOfWork.Tables.AddAsync(table, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return table.Id;
    }

    public async Task RenameAsync(RenameTableDto dto, CancellationToken cancellationToken = default)
    {
        TableValidator.ValidateName(dto.Name);

        var table = await _unitOfWork.Tables.GetByIdAsync(dto.Id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Table), dto.Id);

        table.Name = dto.Name.Trim();
        _unitOfWork.Tables.Update(table);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var table = await _unitOfWork.Tables.GetByIdAsync(id, cancellationToken)
                    ?? throw new NotFoundException(nameof(Table), id);

        _unitOfWork.Tables.Delete(table);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
