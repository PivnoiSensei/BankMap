using BankMap.Domain.Interfaces.Persistence;
using BankMap.Infrastructure.Data;
using BankMap.Infrastructure.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankMap.Infrastructure.DependencyInjection
{
    public static class IServiceCollection
    {
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddInfrastructure(
        this Microsoft.Extensions.DependencyInjection.IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IBranchManager, BranchManager>();
            return services;
        }
    }
}
