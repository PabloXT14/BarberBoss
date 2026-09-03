using System.Globalization;
using BarberBoss.Application.UseCases.Billings.Reports.Excel.Colors;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Extensions;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using ClosedXML.Excel;

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

        using var workbook = new XLWorkbook();

        workbook.Author = "BarberBoss";
        workbook.Style.Font.FontSize = 12;
        workbook.Style.Font.FontName = "Roboto";

        // Obs: Excel don't allow special characters in sheet names, so we need to format the period string accordingly
        var period = $"{request.StartDate:dd-MM-yyyy} | {request.EndDate:dd-MM-yyyy}";

        var worksheet = workbook.Worksheets.Add(period);

        InsertHeader(worksheet);

        var row = 2;
        foreach (var billing in billings)
        {
            InsertBilling(worksheet, billing, row, culture);

            row++;
        }

        worksheet.Columns().AdjustToContents(); // Adjust column widths to fit content

        var file = new MemoryStream();

        workbook.SaveAs(file);

        return file.ToArray();
    }

    private void InsertHeader(IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = ResourceReportGenerationMessages.TITLE;
        worksheet.Cell("B1").Value = ResourceReportGenerationMessages.DATE;
        worksheet.Cell("C1").Value = ResourceReportGenerationMessages.PAYMENT_METHOD;
        worksheet.Cell("D1").Value = ResourceReportGenerationMessages.AMOUNT;
        worksheet.Cell("E1").Value = ResourceReportGenerationMessages.DESCRIPTION;

        worksheet.Range("A1:E1").Style.Font.SetBold();
        worksheet.Range("A1:E1").Style.Font.SetFontColor(XLColor.FromHtml(ColorsHelper.WHITE));
        worksheet.Range("A1:E1").Style.Fill.SetBackgroundColor(XLColor.FromHtml(ColorsHelper.GREEN_DARK));
        // worksheet.Row(1).Height = 24; // Set row height for header

        // ALIGNMENTS
        worksheet.Range("A1:E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Range("A1:E1").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

    }

    private void InsertBilling(IXLWorksheet worksheet, Billing billing, int row, CultureInfo culture)
    {
        worksheet.Cell($"A{row}").Value = billing.ServiceName;

        worksheet.Cell($"B{row}").Value = billing.Date;
        worksheet.Cell($"B{row}").Style.DateFormat.Format = culture.DateTimeFormat.ShortDatePattern; // Use the short date pattern of the current culture

        worksheet.Cell($"C{row}").Value = billing.PaymentMethod.PaymentMethodToString();

        worksheet.Cell($"D{row}").Value = billing.Amount;
        worksheet.Cell($"D{row}").Style.NumberFormat.Format = BuildCurrencyFormat(culture);

        worksheet.Cell($"E{row}").Value = billing.Notes;

        // ALIGNMENTS
        worksheet.Cell($"A{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        worksheet.Cell($"B{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell($"C{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        worksheet.Cell($"D{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        worksheet.Cell($"E{row}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
    }

    private static string BuildCurrencyFormat(CultureInfo culture)
    {
        var symbol = culture.NumberFormat.CurrencySymbol;
        var lcidHex = culture.LCID.ToString("X", CultureInfo.InvariantCulture);

        // pt-BR -> [$R$-416]#,##0.00  |  en-US -> [$$-409]#,##0.00
        return $"[${symbol}-{lcidHex}] #,##0.00";
    }
}