using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Domain.Repositories;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.Update;

public class UpdateBillingUseCase : IUpdateBillingUseCase
{
    private readonly IBillingsUpdateOnlyRepository _billingsUpdateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateBillingUseCase(
        IBillingsUpdateOnlyRepository billingsUpdateRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _billingsUpdateRepository = billingsUpdateRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task Execute(Guid id, RequestBillingJson request)
    {
        Validate(request);

        var entity = await _billingsUpdateRepository.GetById(id);

        if (entity == null)
        {
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);
        }

        _mapper.Map(request, entity);

        entity.UpdatedAt = DateTime.Now;

        _billingsUpdateRepository.Update(entity);

        await _unitOfWork.Commit();
    }

    private void Validate(RequestBillingJson request)
    {
        var validator = new WriteBillingValidator();

        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}