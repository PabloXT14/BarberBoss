using AutoMapper;
using BarberBoss.Communication.Requests;
using BarberBoss.Communication.Responses;
using BarberBoss.Domain.Entities;

namespace BarberBoss.Application.AutoMapper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        RequestToEntity();
        EntityToResponse();
    }

    private void RequestToEntity()
    {
        CreateMap<RequestBillingJson, Billing>();
    }

    private void EntityToResponse()
    {
        CreateMap<Billing, ResponseRegisteredBillingJson>();
        CreateMap<Billing, ResponseBillingJson>();
    }
}