using BarberBoss.Communication.Requests;

namespace BarberBoss.Application.UseCases.Billings.Reports.Excel;

public interface IGenerateBillingsReportExcelUseCase
{
    Task<byte[]> Execute(RequestGenerateBillingsReportJson request);
}