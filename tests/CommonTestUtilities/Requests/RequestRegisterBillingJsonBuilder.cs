using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestUtilities.Requests;

public class RequestRegisterBillingJsonBuilder
{
    public static RequestBillingJson Build()
    {
        var faker = new Faker();

        return new RequestBillingJson
        {
            Date = faker.Date.Past(),
            BarberName = faker.Name.FullName(),
            ClientName = faker.Name.FullName(),
            ServiceName = faker.Commerce.ProductName(),
            Amount = faker.Finance.Amount(min: 1, max: 1000),
            PaymentMethod = faker.PickRandom<PaymentMethod>(),
            Status = faker.PickRandom<BillingStatus>(),
            Notes = faker.Commerce.ProductDescription()
        };
    }
}