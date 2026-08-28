using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Register;

public class RegisterBillingUseCase : IRegisterBillingUseCase
{
    private readonly IBillingsWriteOnlyRepository _billingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterBillingUseCase(IBillingsWriteOnlyRepository billingRepository, IUnitOfWork unitOfWork)
    {
        _billingRepository = billingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredBillingJson> Execute(RequestBillingJson requestBillingJson)
    {
        Validate(requestBillingJson);

        var entity = new Billing
        {
            Date = requestBillingJson.Date,
            BarberName = requestBillingJson.BarberName,
            ClientName = requestBillingJson.ClientName,
            ServiceName = requestBillingJson.ServiceName,
            Amount = requestBillingJson.Amount,
            PaymentMethod = (BarberBoss.Domain.Enums.PaymentMethod)requestBillingJson.PaymentMethod,
            Status = (BarberBoss.Domain.Enums.BillingStatus)requestBillingJson.Status,
            Notes = requestBillingJson.Notes,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _billingRepository.Add(entity);
        await _unitOfWork.Commit();

        return new ResponseRegisteredBillingJson
        {
            Id = entity.Id,
        };
    }

    private void Validate(RequestBillingJson requestBillingJson)
    {
        var validator = new WriteBillingValidator();

        var validationResult = validator.Validate(requestBillingJson);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}