namespace BarberBoss.Communication.Requests;

public class RequestGenerateBillingsReportJson
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}