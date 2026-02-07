using Microsoft.EntityFrameworkCore;
using BankMap.Api.Models;

namespace BankMap.Api.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        public DbSet<BankBranch> Branches { get; set; }
    }
}
