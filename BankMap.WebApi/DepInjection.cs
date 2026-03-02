using BankMap.Application.Features.Branches.Queries.GetAllBranches;
using BankMap.Application.Services;
using BankMap.Domain.Interfaces.Persistence;
using BankMap.Infrastructure.Managers;

namespace BankMap.WebApi
{
    public static class DepInjection
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services){
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<GetAllBranchesHandler>();
            });
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IBranchManager, BranchManager>();

            return services;
        }
    }
}
