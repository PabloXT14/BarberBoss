using BarberBoss.Communication.Requests;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public interface IGenerateBillingsReportUseCase
{
    Task<byte[]> Execute(RequestGenerateBillingsReportJson request);
}