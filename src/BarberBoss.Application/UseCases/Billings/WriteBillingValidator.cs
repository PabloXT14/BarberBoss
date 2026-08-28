using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;

public class WriteBillingValidator : AbstractValidator<RequestBillingJson>
{
    public WriteBillingValidator()
    {
        RuleFor(billing => billing.Date)
            .LessThanOrEqualTo(DateTime.Now).WithMessage(ResourceErrorMessages.BILLING_DATE_CANNOT_BE_FUTURE);

        RuleFor(billing => billing.BarberName)
            .NotEmpty().WithMessage(ResourceErrorMessages.BARBER_NAME_REQUIRED);

        RuleFor(billing => billing.ClientName)
            .NotEmpty().WithMessage(ResourceErrorMessages.CLIENT_NAME_REQUIRED);

        RuleFor(billing => billing.ServiceName)
            .NotEmpty().WithMessage(ResourceErrorMessages.SERVICE_NAME_REQUIRED);

        RuleFor(billing => billing.Amount)
            .GreaterThan(0).WithMessage(ResourceErrorMessages.AMOUNT_MUST_BE_GREATER_THAN_ZERO);

        RuleFor(billing => billing.PaymentMethod)
            .IsInEnum().WithMessage(ResourceErrorMessages.INVALID_PAYMENT_METHOD);

        RuleFor(billing => billing.Status)
            .IsInEnum().WithMessage(ResourceErrorMessages.INVALID_STATUS);
    }
}