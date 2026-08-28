using AutoMapper;
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
    private readonly IMapper _mapper;

    public RegisterBillingUseCase(
        IBillingsWriteOnlyRepository billingRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _billingRepository = billingRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ResponseRegisteredBillingJson> Execute(RequestBillingJson request)
    {
        Validate(request);

        var entity = _mapper.Map<Billing>(request);

        await _billingRepository.Add(entity);
        await _unitOfWork.Commit();

        var response = _mapper.Map<ResponseRegisteredBillingJson>(entity);

        return response;
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