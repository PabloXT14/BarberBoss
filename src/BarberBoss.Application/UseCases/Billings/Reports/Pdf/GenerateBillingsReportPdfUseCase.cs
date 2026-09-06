using System.Globalization;
using System.Reflection;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Colors;
using BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Reports;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
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
        var document = CreateDocument(startDate: request.StartDate, endDate: request.EndDate, culture: culture);
        var page = CreatePage(document);

        CreateHeaderWithLogoAndName(page);

        var totalBillings = billings.Sum(billing => billing.Amount);
        CreateTotalBillingsSection(page, request, totalBillings, culture);

        return RenderDocument(document);
    }

    private Document CreateDocument(DateOnly startDate, DateOnly endDate, CultureInfo culture)
    {
        var document = new Document();

        document.Info.Title = $"{ResourceReportGenerationMessages.BILLINGS_FOR} {startDate.ToString("d", culture)} - {endDate.ToString("d", culture)}";
        document.Info.Author = "BarberBoss";

        var styles = document.Styles["Normal"];
        styles!.Font.Name = FontsHelper.ROBOTO_REGULAR;

        return document;
    }

    private Section CreatePage(Document document)
    {
        var section = document.AddSection();

        section.PageSetup = document.DefaultPageSetup.Clone();

        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.TopMargin = 50;
        section.PageSetup.BottomMargin = 50;
        section.PageSetup.LeftMargin = 40;
        section.PageSetup.RightMargin = 40;

        return section;
    }

    private void CreateHeaderWithLogoAndName(Section page)
    {
        var table = page.AddTable();

        // Add 2 columns
        table.AddColumn();
        table.AddColumn(300); // Set the width of the second column to 300 pixels

        var row = table.AddRow();

        var assembly = Assembly.GetExecutingAssembly();
        var directoryName = Path.GetDirectoryName(assembly.Location);
        var filePath = Path.Combine(directoryName!, "Assets", "logo.png");

        var image = row.Cells[0].AddImage(filePath);
        image.Width = 62;

        row.Cells[1].Format.LeftIndent = 10;
        row.Cells[1].AddParagraph("Barbearia do João");
        row.Cells[1].Format.Font = new Font
        {
            Name = FontsHelper.BEBASNEUE_REGULAR,
            Size = 25
        };
        row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
    }

    private void CreateTotalBillingsSection(Section page, RequestGenerateBillingsReportJson request, decimal totalBillings, CultureInfo culture)
    {
        var paragraph = page.AddParagraph();
        paragraph.Format.SpaceBefore = 38;
        paragraph.Format.SpaceAfter = 64;

        var title = $"{ResourceReportGenerationMessages.BILLINGS_FOR} {request.StartDate.ToString("d", culture)} - {request.EndDate.ToString("d", culture)}";
        paragraph.AddFormattedText(title, new Font
        {
            Name = FontsHelper.ROBOTO_MEDIUM,
            Size = 15
        });

        paragraph.AddLineBreak();

        var totalCurrencyText = totalBillings.ToString("C", culture);
        paragraph.AddFormattedText(totalCurrencyText, new Font
        {
            Name = FontsHelper.BEBASNEUE_REGULAR,
            Size = 50
        });
    }

    private byte[] RenderDocument(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document
        };

        renderer.RenderDocument();

        using var fileStream = new MemoryStream();

        renderer.PdfDocument.Save(fileStream);

        return fileStream.ToArray();
    }
}