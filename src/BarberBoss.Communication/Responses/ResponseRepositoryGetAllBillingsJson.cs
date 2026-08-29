namespace BarberBoss.Communication.Responses;

public class ResponseRepositoryGetAllBillingsJson<T>
{
    public List<T> Billings { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}