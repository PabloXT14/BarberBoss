using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Repositories.Billings;
using BarberBoss.Exception.ExceptionsBase;

namespace BarberBoss.Application.UseCases.Billings.GetAll;

public class GetAllBillingsUseCase : IGetAllBillingsUseCase
{
    private readonly IBillingsReadOnlyRepository _billingReadOnlyRepository;
    private readonly IMapper _mapper;

    public GetAllBillingsUseCase(IBillingsReadOnlyRepository billingReadOnlyRepository, IMapper mapper)
    {
        _billingReadOnlyRepository = billingReadOnlyRepository;
        _mapper = mapper;
    }

    public async Task<ResponseGetAllBillingsJson> Execute(RequestGetAllBillingsJson request)
    {
        Validate(request);

        var result = await _billingReadOnlyRepository.GetAll(request);

        var response = new ResponseGetAllBillingsJson
        {
            Billings = _mapper.Map<List<ResponseShortBillingJson>>(result.Billings),
            Pagination = new ResponsePaginationJson
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = result.TotalCount
            }
        };

        return response;
    }

    private void Validate(RequestGetAllBillingsJson request)
    {
        var validator = new GetAllBillingsValidator();

        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}