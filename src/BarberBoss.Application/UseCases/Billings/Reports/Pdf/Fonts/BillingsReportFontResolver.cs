using System.Reflection;
using PdfSharp.Fonts;

namespace BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts;

public class BillingsReportFontResolver : IFontResolver
{
    public byte[]? GetFont(string faceName)
    {
        var fontStream = ReadFontFile(faceName);
        fontStream ??= ReadFontFile(FontsHelper.DEFAULT_FONT);

        var length = (int)fontStream!.Length;

        var fontData = new byte[length];

        // offset = 0 because we want to read the entire font file from the beginning from the array of bytes
        _ = fontStream.Read(buffer: fontData, offset: 0, count: length);

        return fontData;
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool bold, bool italic)
    {
        return new FontResolverInfo(familyName);
    }

    private Stream? ReadFontFile(string faceName)
    {
        var assembly = Assembly.GetExecutingAssembly(); // Get the reference to the Assembly DLL of the current project (CashFlow.Application.dll) where the font files are embedded as resources

        return assembly.GetManifestResourceStream($"BarberBoss.Application.UseCases.Billings.Reports.Pdf.Fonts.{faceName}.ttf");
    }
}