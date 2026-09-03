using System.Globalization;

namespace BarberBoss.Api.Middlewares;

public class CultureMiddleware
{
    // Monta a lista de nomes suportados uma única vez (não a cada request) e usa HashSet pra lookup O(1)
    private static readonly HashSet<string> SupportedCultureNames =
       CultureInfo.GetCultures(CultureTypes.AllCultures)
           .Select(culture => culture.Name)
           .Where(name => !string.IsNullOrWhiteSpace(name))
           .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private readonly RequestDelegate _next;

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestedCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

        var cultureInfo = new CultureInfo("en-US"); // Default culture language (specific not neutral for currency formatting)

        // Obs: se a primeira condição for falsa, a segunda nem será checada pelo .NET (por conta do &&), evitando assim um processamento desnecessário
        if (!string.IsNullOrWhiteSpace(requestedCulture)
        && SupportedCultureNames.Contains(requestedCulture))
        {
            var matchedCulture = CultureInfo.GetCultureInfo(requestedCulture);

            // Cultura neutra (ex.: "fr", "en", "pt") não carrega símbolo de moeda/formatação de número —
            // converte pra cultura específica padrão daquele idioma (ex.: "fr-FR", "en-US", "pt-BR")
            cultureInfo = matchedCulture.IsNeutralCulture
                ? CultureInfo.CreateSpecificCulture(matchedCulture.Name)
                : matchedCulture;
        }

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo; // Set the current UI culture

        await _next(context); // Continue processing the request
    }
}