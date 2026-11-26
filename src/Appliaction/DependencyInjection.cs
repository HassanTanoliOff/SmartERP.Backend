using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Later, when you have services:
        // services.AddScoped<IProductService, ProductService>();

        services.AddValidatorsFromAssembly(assembly);
        // services.AddAutoMapper(assembly); // when profiles exist

        return services;
    }
}
