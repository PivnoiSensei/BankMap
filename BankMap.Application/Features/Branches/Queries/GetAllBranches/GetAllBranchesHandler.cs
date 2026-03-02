using MediatR;
using BankMap.Application.Common;
using BankMap.Application.Services;

namespace BankMap.Application.Features.Branches.Queries.GetAllBranches
{
    public class GetAllBranchesHandler : IRequestHandler<GetAllBranchesQuery, Result<List<BranchDto>>>
    {
        private readonly IBranchService _branchService;
        public GetAllBranchesHandler(IBranchService branchService)
        {
            _branchService = branchService;
        }
        public async Task<Result<List<BranchDto>>> Handle(
            GetAllBranchesQuery req, CancellationToken ct
        )
        {
            var res = await _branchService.GetAllAsync(ct);

            if (!res.IsSuccess)
                if(res.Error != null)
                    return Result<List<BranchDto>>.Failure(res.Error);
            var dto = res.Value!
                .Select(branch => new BranchDto(
                 branch.Id,
                        branch.Name,
                        branch.Type.ToString(),
                        branch.IsTemporaryClosed,
                        branch.IsRegular,
                        new AddressDto(
                            branch.Address.City,
                            branch.Address.FullAddress,
                            branch.Address.DetailedAddress,
                            branch.Address.Latitude,
                            branch.Address.Longitude
                        ),
                        branch.Schedules.Select(s => new WorkScheduleDto(
                            s.WorkStation,
                            s.Days.Select(d => new WorkingDayDto(
                                d.Day.ToString(),
                                d.From,
                                d.To,
                                d.Breaks.Select(b => new BreakIntervalDto(
                                    b.From,
                                    b.To
                                )).ToList()
                            )).ToList()
                        )).ToList(),
                        branch.Phones.Select(p => new ContactPhoneDto(
                            p.OperatorCode,
                            p.Number,
                            p.FullNumber
                        )).ToList(),
                        branch.CashDesks.Select(c => new CashDeskDto(
                            c.ExternalId,
                            c.Description,
                            c.WorkDays.Select(w => new CashWorkDayDto(
                                w.Day.ToString(),
                                w.From,
                                w.To,
                                w.Breaks.Select(b => new BreakIntervalDto(
                                    b.From,
                                    b.To
                                )).ToList()
                            )).ToList()
                        )).ToList()
                )).ToList();

            return Result<List<BranchDto>>
                .Success(dto);
        }
    }
}
