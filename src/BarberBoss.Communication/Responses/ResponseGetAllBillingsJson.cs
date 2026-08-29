namespace BarberBoss.Communication.Responses;

public class ResponseGetAllBillingsJson
{
    public List<ResponseShortBillingJson> Billings { get; set; } = [];
    public ResponsePaginationJson Pagination { get; set; } = new ResponsePaginationJson();
}