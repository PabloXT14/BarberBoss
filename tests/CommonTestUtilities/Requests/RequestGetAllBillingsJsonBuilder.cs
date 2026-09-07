using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestUtilities.Requests;

public class RequestGetAllBillingsJsonBuilder
{
    public static RequestGetAllBillingsJson Build()
    {
        var faker = new Faker();

        return new RequestGetAllBillingsJson
        {
            Page = faker.Random.Int(1, 10),
            PageSize = faker.Random.Int(1, 100),
            SearchTerm = faker.Random.Word(),
            StartDate = faker.Date.PastDateOnly(),
            EndDate = faker.Date.FutureDateOnly(),
            MinAmount = faker.Random.Decimal(0, 1000),
            MaxAmount = faker.Random.Decimal(1000, 5000),
            Status = faker.PickRandom<BillingStatus>(),
            PaymentMethod = faker.PickRandom<PaymentMethod>()
        };
    }
}