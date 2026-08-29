using BarberBoss.Communication.Enums;

namespace BarberBoss.Communication.Requests;

public class RequestGetAllBillingsJson
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public BillingStatus? Status { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}