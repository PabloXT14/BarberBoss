using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;

namespace BarberBoss.Domain.Repositories.Billings;

public interface IBillingsReadOnlyRepository
{
    Task<ResponseRepositoryGetAllBillingsJson<Billing>> GetAll(RequestGetAllBillingsJson request);
    Task<Billing?> GetById(Guid id);
    Task<List<Billing>> FilterByMonth(DateOnly month);
}