using BankMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;

//Use BankBranch table as context for controllers
namespace BankMap.Infrastructure.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {}
        public DbSet<Branch> Branches => Set<Branch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
