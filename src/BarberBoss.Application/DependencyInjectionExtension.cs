using BarberBoss.Application.AutoMapper;
using BarberBoss.Application.UseCases.Billings.Register;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Application;



public static class DependencyInjectionExtension
{
    public static void AddApplication(this IServiceCollection services)
    {
        AddUseCases(services);
        AddAutoMapper(services);
    }

    public static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<IRegisterBillingUseCase, RegisterBillingUseCase>();
    }

    public static void AddAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AutoMapping>();
        });
    }
}