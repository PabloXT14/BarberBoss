using AutoMapper;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.GetById;

public class GetBillingByIdUseCase : IGetBillingByIdUseCase
{
    private readonly IBillingsReadOnlyRepository _billingRepository;
    private readonly IMapper _mapper;

    public GetBillingByIdUseCase(
        IBillingsReadOnlyRepository billingRepository,
        IMapper mapper)
    {
        _billingRepository = billingRepository;
        _mapper = mapper;
    }

    public async Task<ResponseBillingJson> Execute(Guid billingId)
    {
        var entity = await _billingRepository.GetById(billingId);

        if (entity == null)
        {
            throw new NotFoundException(ResourceErrorMessages.BILLING_NOT_FOUND);
        }

        var response = _mapper.Map<ResponseBillingJson>(entity);

        return response;
    }
}