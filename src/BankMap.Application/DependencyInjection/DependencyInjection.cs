using BankMap.Application.Common.Behaviors;
using BankMap.Application.Mappers;
using BankMap.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BankMap.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IBranchMapper, BranchMapper>();
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
