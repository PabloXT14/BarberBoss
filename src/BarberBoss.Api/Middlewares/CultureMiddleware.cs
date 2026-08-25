using System.Globalization;

namespace BarberBoss.Api.Middlewares;

public class CultureMiddleware
{
    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var supportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).ToList();

        var requestedCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

        var cultureInfo = new CultureInfo("en"); // Default culture language

        // Obs: se a primeira condição for falsa, a segunda nem será checada pelo .NET (por conta do &&), evitando assim um processamento desnecessário
        if (!string.IsNullOrWhiteSpace(requestedCulture)
        && supportedCultures.Exists(language => language.Name.Equals(requestedCulture, StringComparison.OrdinalIgnoreCase)))
        {
            cultureInfo = new CultureInfo(requestedCulture);
        }

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo; // Set the current UI culture

        await _next(context); // Continue processing the request
    }
}