using BarberBoss.Communication.Requests;
using BarberBoss.Exception;
using FluentValidation;

namespace BarberBoss.Application.UseCases.Billings;

public class GetAllBillingsValidator : AbstractValidator<RequestGetAllBillingsJson>
{
    public GetAllBillingsValidator()
    {
        // Pagination
        RuleFor(request => request.Page)
            .GreaterThan(0)
            .WithMessage(ResourceErrorMessages.PAGE_MUST_BE_GREATER_THAN_ZERO);

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .WithMessage(ResourceErrorMessages.PAGE_SIZE_MUST_BE_GREATER_THAN_ZERO);

        // Date filters
        RuleFor(request => request.EndDate)
            .NotNull()
            .When(request => request.StartDate is not null)
            .WithMessage(ResourceErrorMessages.END_DATE_IS_REQUIRED_WHEN_START_DATE_IS_PROVIDED);

        RuleFor(request => request.StartDate)
            .NotNull()
            .When(request => request.EndDate is not null)
            .WithMessage(ResourceErrorMessages.START_DATE_IS_REQUIRED_WHEN_END_DATE_IS_PROVIDED);

        RuleFor(request => request.EndDate)
            .GreaterThanOrEqualTo(request => request.StartDate)
            .When(request => request.StartDate is not null && request.EndDate is not null)
            .WithMessage(ResourceErrorMessages.END_DATE_MUST_BE_GREATER_THAN_OR_EQUAL_TO_START_DATE);

        // Amount filters
        RuleFor(request => request.MaxAmount)
            .NotNull()
            .When(request => request.MinAmount is not null)
            .WithMessage(ResourceErrorMessages.MAX_AMOUNT_IS_REQUIRED_WHEN_MIN_AMOUNT_IS_PROVIDED);

        RuleFor(request => request.MinAmount)
            .NotNull()
            .When(request => request.MaxAmount is not null)
            .WithMessage(ResourceErrorMessages.MIN_AMOUNT_IS_REQUIRED_WHEN_MAX_AMOUNT_IS_PROVIDED);

        RuleFor(request => request.MinAmount)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MinAmount is not null)
            .WithMessage(ResourceErrorMessages.MIN_AMOUNT_MUST_BE_GREATER_THAN_OR_EQUAL_TO_ZERO);

        RuleFor(request => request.MaxAmount)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MaxAmount is not null)
            .WithMessage(ResourceErrorMessages.MAX_AMOUNT_MUST_BE_GREATER_THAN_OR_EQUAL_TO_ZERO);

        RuleFor(request => request.MaxAmount)
            .GreaterThanOrEqualTo(request => request.MinAmount)
            .When(request => request.MinAmount is not null && request.MaxAmount is not null)
            .WithMessage(ResourceErrorMessages.MAX_AMOUNT_MUST_BE_GREATER_THAN_OR_EQUAL_TO_MIN_AMOUNT);

        // Enums
        RuleFor(request => request.Status)
            .IsInEnum()
            .When(request => request.Status is not null)
            .WithMessage(ResourceErrorMessages.INVALID_STATUS);

        RuleFor(request => request.PaymentMethod)
            .IsInEnum()
            .When(request => request.PaymentMethod is not null)
            .WithMessage(ResourceErrorMessages.INVALID_PAYMENT_METHOD);
    }
}
