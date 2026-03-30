using MediatR;
using FluentValidation;
using BusinessCentral.Application.Common.Behaviors;
using System.Reflection;

namespace BusinessCentral.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.Load("BusinessCentral.Application"));

        services.AddValidatorsFromAssembly(Assembly.Load("BusinessCentral.Application"));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}