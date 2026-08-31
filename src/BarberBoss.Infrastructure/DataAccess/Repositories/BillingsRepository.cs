using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Repositories.Billings;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.DataAccess.Repositories;

internal class BillingsRepository : IBillingsWriteOnlyRepository, IBillingsReadOnlyRepository, IBillingsUpdateOnlyRepository
{
    private readonly BarberBossDbContext _dbContext;

    public BillingsRepository(BarberBossDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Billing billing)
    {
        await _dbContext.Billings.AddAsync(billing);
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await _dbContext.Billings.FirstOrDefaultAsync(billing => billing.Id == id);

        if (entity is null)
        {
            return false;
        }

        _dbContext.Billings.Remove(entity);

        return true;
    }

    public async Task<ResponseRepositoryGetAllBillingsJson<Billing>> GetAll(RequestGetAllBillingsJson request)
    {
        var skip = (request.Page - 1) * request.PageSize;

        var query = _dbContext.Billings.AsQueryable().AsNoTracking();


        // FILTER BY SEARCH
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(billing =>
                EF.Functions.Like(billing.ServiceName, $"%{request.SearchTerm}%") ||
                EF.Functions.Like(billing.ClientName, $"%{request.SearchTerm}%") ||
                EF.Functions.Like(billing.BarberName, $"%{request.SearchTerm}%") ||
                EF.Functions.Like(billing.Notes, $"%{request.SearchTerm}%"));
        }

        // FILTER BY DATE
        if (request.StartDate.HasValue && request.EndDate.HasValue)
        {
            var startDate = new DateTime(
                year: request.StartDate.Value.Year,
                month: request.StartDate.Value.Month,
                day: request.StartDate.Value.Day
            ).Date; // Date -> return a date with time set to 00:00:00 (midnight)

            var endDate = new DateTime(
                year: request.EndDate.Value.Year,
                month: request.EndDate.Value.Month,
                day: request.EndDate.Value.Day
            ).Date; // Date -> return a date with time set to 00:00:00 (midnight)

            endDate = endDate.AddDays(1).AddTicks(-1); // Get the end of the day

            query = query.Where(billing => billing.Date >= startDate && billing.Date <= endDate);
        }

        // FILTER BY AMOUNT
        if (request.MinAmount.HasValue)
        {
            query = query.Where(billing => billing.Amount >= request.MinAmount.Value);
        }

        if (request.MaxAmount.HasValue)
        {
            query = query.Where(billing => billing.Amount <= request.MaxAmount.Value);
        }

        // FILTER BY STATUS
        if (request.Status.HasValue)
        {
            query = query.Where(billing => billing.Status == (Domain.Enums.BillingStatus)request.Status.Value);
        }

        // FILTER BY PAYMENT METHOD
        if (request.PaymentMethod.HasValue)
        {
            query = query.Where(billing => billing.PaymentMethod == (Domain.Enums.PaymentMethod)request.PaymentMethod.Value);
        }

        var totalCount = await query.CountAsync();

        var billings = await query
            .OrderBy(billing => billing.Date)
            .ThenBy(billing => billing.ServiceName)
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync();

        return new ResponseRepositoryGetAllBillingsJson<Billing>
        {
            Billings = billings,
            TotalCount = totalCount
        };
    }

    async Task<Billing?> IBillingsReadOnlyRepository.GetById(Guid id)
    {
        return await _dbContext.Billings
            .AsNoTracking()
            .FirstOrDefaultAsync(billing => billing.Id == id);
    }

    async Task<Billing?> IBillingsUpdateOnlyRepository.GetById(Guid id)
    {
        return await _dbContext.Billings
            .AsNoTracking()
            .FirstOrDefaultAsync(billing => billing.Id == id);
    }

    public void Update(Billing billing)
    {
        _dbContext.Billings.Update(billing);
    }

    public async Task<List<Billing>> FilterByMonth(DateOnly date)
    {
        var startDate = new DateTime(year: date.Year, month: date.Month, day: 1).Date; // Date -> return a date with time set to 00:00:00 (midnight)

        var daysInMonth = DateTime.DaysInMonth(year: date.Year, month: date.Month);

        var endDate = new DateTime(
            year: date.Year,
            month: date.Month,
            day: daysInMonth,
            hour: 23,
            minute: 59,
            second: 59
        );

        return await _dbContext.Billings
            .AsNoTracking()
            .Where(billing => billing.Date >= startDate && billing.Date <= endDate)
            .OrderBy(billing => billing.Date)
            .ThenBy(billing => billing.ServiceName)
            .ToListAsync();
    }

    public async Task<List<Billing>> FilterByDateRange(DateOnly startDate, DateOnly endDate)
    {
        var startDateTime = new DateTime(year: startDate.Year, month: startDate.Month, day: startDate.Day).Date; // Date -> return a date with time set to 00:00:00 (midnight)

        var endDateTime = new DateTime(year: endDate.Year, month: endDate.Month, day: endDate.Day, hour: 23, minute: 59, second: 59); // Set the time to the end of the day

        return await _dbContext.Billings
            .AsNoTracking()
            .Where(billing => billing.Date >= startDateTime && billing.Date <= endDateTime)
            .Where(billing => billing.Status == BillingStatus.Paid)
            .OrderBy(billing => billing.Date)
            .ThenBy(billing => billing.ServiceName)
            .ToListAsync();
    }
}