using CustomizableTablesWithDeadlines.Application.Abstractions.Persistence;
using CustomizableTablesWithDeadlines.Application.Abstractions.Services;
using CustomizableTablesWithDeadlines.Application.Common;
using CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;
using CustomizableTablesWithDeadlines.Application.Exceptions;
using CustomizableTablesWithDeadlines.Application.Validators;
using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Services;

public class DeadlineService : IDeadlineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationScheduler _notificationScheduler;

    public DeadlineService(IUnitOfWork unitOfWork, INotificationScheduler notificationScheduler)
    {
        _unitOfWork = unitOfWork;
        _notificationScheduler = notificationScheduler;
    }

    public async Task<List<DeadlineDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var deadlines = await _unitOfWork.Deadlines.GetAllWithRowAsync(cancellationToken);
        return deadlines.Select(d => d.ToDto()).ToList();
    }

    public async Task<List<DeadlineDto>> GetUpcomingAsync(CancellationToken cancellationToken = default)
    {
        var deadlines = await _unitOfWork.Deadlines.GetUpcomingDeadlinesAsync(cancellationToken);
        return deadlines.Select(d => d.ToDto()).ToList();
    }

    public async Task<List<DeadlineDto>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var deadlines = await _unitOfWork.Deadlines.GetOverdueDeadlinesAsync(cancellationToken);
        return deadlines.Select(d => d.ToDto()).ToList();
    }

    public async Task<List<DeadlineDto>> GetByRowIdAsync(int rowId, CancellationToken cancellationToken = default)
    {
        var deadlines = await _unitOfWork.Deadlines.GetDeadlinesByRowIdAsync(rowId, cancellationToken);
        return deadlines.Select(d => d.ToDto()).ToList();
    }

    public async Task<int> CreateAsync(CreateDeadlineDto dto, CancellationToken cancellationToken = default)
    {
        DeadlineValidator.ValidateTitle(dto.Title);
        DeadlineValidator.ValidateDateTime(dto.DeadlineDateTime);

        var row = await _unitOfWork.Rows.GetByIdAsync(dto.RowId, cancellationToken)
                  ?? throw new NotFoundException(nameof(Row), dto.RowId);

        var deadline = new Deadline
        {
            RowId = row.Id,
            Title = dto.Title.Trim(),
            DeadlineDateTime = dto.DeadlineDateTime
        };

        await _unitOfWork.Deadlines.AddAsync(deadline, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationScheduler.ScheduleDeadlineNotificationsAsync(deadline.Id, cancellationToken);
        return deadline.Id;
    }

    public async Task UpdateAsync(UpdateDeadlineDto dto, CancellationToken cancellationToken = default)
    {
        DeadlineValidator.ValidateTitle(dto.Title);
        DeadlineValidator.ValidateDateTime(dto.DeadlineDateTime);

        var deadline = await _unitOfWork.Deadlines.GetByIdAsync(dto.Id, cancellationToken)
                       ?? throw new NotFoundException(nameof(Deadline), dto.Id);

        deadline.Title = dto.Title.Trim();
        deadline.DeadlineDateTime = dto.DeadlineDateTime;
        _unitOfWork.Deadlines.Update(deadline);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _notificationScheduler.RescheduleDeadlineNotificationsAsync(deadline.Id, cancellationToken);
    }

    public async Task DeleteAsync(int deadlineId, CancellationToken cancellationToken = default)
    {
        var deadline = await _unitOfWork.Deadlines.GetByIdAsync(deadlineId, cancellationToken)
                       ?? throw new NotFoundException(nameof(Deadline), deadlineId);

        await _notificationScheduler.CancelDeadlineNotificationsAsync(deadlineId, cancellationToken);
        _unitOfWork.Deadlines.Delete(deadline);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
