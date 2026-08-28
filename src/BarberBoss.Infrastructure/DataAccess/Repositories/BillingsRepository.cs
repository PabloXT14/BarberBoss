using BarberBoss.Domain.Entities;
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

    public async Task<List<Billing>> GetAll()
    {
        return await _dbContext.Billings.AsNoTracking().ToListAsync();
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
}