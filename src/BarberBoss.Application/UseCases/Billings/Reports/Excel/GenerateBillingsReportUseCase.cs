using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories.Billings;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public class GenerateBillingsReportUseCase : IGenerateBillingsReportUseCase
{
    private readonly IBillingsReadOnlyRepository _billingsReadOnlyRepository;

    public GenerateBillingsReportUseCase(IBillingsReadOnlyRepository billingsReadOnlyRepository)
    {
        _billingsReadOnlyRepository = billingsReadOnlyRepository;
    }

    public async Task<byte[]> Execute(RequestGenerateBillingsReportJson request)
    {
        var billings = await _billingsReadOnlyRepository.FilterByDateRange(
            startDate: request.StartDate,
            endDate: request.EndDate
        );

        if (billings.Count == 0)
        {
            return [];
        }

        return [];
    }
}