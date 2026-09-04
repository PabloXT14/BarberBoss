using System.Globalization;
using BarberBoss.Application.UseCases.Billings.Reports.Excel.Colors;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using ClosedXML.Excel;
using PdfSharp.Fonts;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf;

public class GenerateBillingsReportPdfUseCase : IGenerateBillingsReportPdfUseCase
{
    private readonly IBillingsReadOnlyRepository _billingsReadOnlyRepository;

    private const int HEIGHT_ROW_TABLE = 25;

    public GenerateBillingsReportPdfUseCase(IBillingsReadOnlyRepository billingsReadOnlyRepository)
    {
        _billingsReadOnlyRepository = billingsReadOnlyRepository;

        GlobalFontSettings.FontResolver = new BillingsReportFontResolver();
    }

    public async Task<byte[]> Execute(RequestGenerateBillingsReportJson request)
    {
        if (request.StartDate > request.EndDate)
        {
            throw new ErrorOnValidationException(
                [ResourceErrorMessages.END_DATE_MUST_BE_GREATER_THAN_OR_EQUAL_TO_START_DATE]
            );
        }

        var billings = await _billingsReadOnlyRepository.FilterByDateRange(
            startDate: request.StartDate,
            endDate: request.EndDate
        );

        if (billings.Count == 0)
        {
            return [];
        }

        var culture = CultureInfo.CurrentCulture; // Get the current culture from the request context

        return [];
    }
}