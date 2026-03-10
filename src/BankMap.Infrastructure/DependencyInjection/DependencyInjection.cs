using BankMap.Domain.Interfaces.Persistence;
using BankMap.Infrastructure.Data;
using BankMap.Infrastructure.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMap.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"), o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            services.AddScoped<IBranchManager, BranchManager>();
            return services;
        }
    }
}
