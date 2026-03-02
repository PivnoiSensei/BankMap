using BankMap.Domain.Entities;
using BankMap.Domain.Interfaces.Persistence;
using BankMap.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMap.Infrastructure.Managers;

public class BranchManager : IBranchManager
{
    private readonly ApplicationDbContext _dbContext;

    public BranchManager(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Branch>> GetAllAsync(CancellationToken ct)
    {
        return await _dbContext.Branches
            .Include(b => b.Schedules)
                .ThenInclude(s => s.Days)
                    .ThenInclude(d => d.Breaks)
            .Include(b => b.Phones)
            .Include(b => b.CashDesks)
                .ThenInclude(c => c.WorkDays)
            .ToListAsync(ct);
    }

    public async Task<int> ImportBranchesAsync(List<Branch> branches, CancellationToken cancellationToken)
    {
        var allBranches = await _dbContext.Branches
            .Include(b => b.Schedules)
                .ThenInclude(s => s.Days)
                    .ThenInclude(d => d.Breaks)
            .Include(b => b.Phones)
            .Include(b => b.CashDesks)
                .ThenInclude(c => c.WorkDays)
            .ToListAsync(cancellationToken);

        _dbContext.Branches.RemoveRange(allBranches);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Branches.AddRangeAsync(branches, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return branches.Count;
    }
}