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

    public async Task<int> ImportBranchesAsync(List<Branch> branches, CancellationToken ct)
    {
        var allBranches = await _dbContext.Branches
            .Include(b => b.Schedules)
                .ThenInclude(s => s.Days)
                    .ThenInclude(d => d.Breaks)
            .Include(b => b.Phones)
            .Include(b => b.CashDesks)
                .ThenInclude(c => c.WorkDays)
            .ToListAsync(ct);

        _dbContext.Branches.RemoveRange(allBranches);
        await _dbContext.SaveChangesAsync(ct);

        await _dbContext.Branches.AddRangeAsync(branches, ct);
        await _dbContext.SaveChangesAsync(ct);

        return branches.Count;
    }

    public async Task<bool> UpdateBranchAsync(int id, bool isTemporaryClosed, CancellationToken ct) {
        var branch = await _dbContext.Branches.FindAsync([id], ct);

        if (branch == null) return false;

        branch.IsTemporaryClosed = isTemporaryClosed;

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}