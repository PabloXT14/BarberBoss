using BarberBoss.Domain.Enums;
using BarberBoss.Domain.Reports;

namespace BarberBoss.Domain.Extensions;

public static class PaymentMethodExtensions
{
    public static string PaymentMethodToString(this PaymentMethod paymentMethod)
    {
        return paymentMethod switch
        {
            PaymentMethod.Cash => ResourceReportGenerationMessages.CASH,
            PaymentMethod.CreditCard => ResourceReportGenerationMessages.CREDIT_CARD,
            PaymentMethod.DebitCard => ResourceReportGenerationMessages.DEBIT_CARD,
            PaymentMethod.Pix => ResourceReportGenerationMessages.PIX,
            PaymentMethod.Other => ResourceReportGenerationMessages.OTHER,
            _ => string.Empty
        };
    }
}