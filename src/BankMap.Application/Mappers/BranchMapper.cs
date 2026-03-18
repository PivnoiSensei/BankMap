using BankMap.Application.Common;
using BankMap.Application.Features.Branches.Commands.ImportJson.Dto;
using BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto;
using BankMap.Domain.Entities;
using System.Text.Json;

namespace BankMap.Application.Mappers;

public class BranchMapper : IBranchMapper
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<Result<List<Branch>>> MapFromStreamAsync(Stream jsonStream, CancellationToken ct)
    {
        try
        {
            var root = await JsonSerializer.DeserializeAsync<DepartmentsImportRootDto>(
                jsonStream, _jsonOptions, ct);

            if (root?.List == null || !root.List.Any())
                return Result<List<Branch>>.Failure("Invalid JSON structure");

            var branches = root.List
                .Select(MapBranch)
                .ToList();

            return Result<List<Branch>>.Success(branches);
        }
        catch (JsonException ex)
        {
            return Result<List<Branch>>.Failure($"JSON format error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<List<Branch>>.Failure($"Mapping error: {ex.Message}");
        }
    }

    private Branch MapBranch(ImportDto dto)
    {
        var address = MapAddress(dto.Address, dto.FullAddress);

        var branchType = Enum.TryParse<BranchType>(dto.Type, true, out var type)
            ? type
            : BranchType.Department;

        var branch = new Branch(dto.Name, branchType, address)
        {
            IsTemporaryClosed = dto.IsTemporaryClosed,
            IsRegular = dto.IsRegular
        };
        foreach (var schedule in MapSchedules(dto.Schedules))
            branch.Schedules.Add(schedule);

        foreach (var phone in MapPhones(dto.Phones))
            branch.Phones.Add(phone);

        foreach (var cash in MapCashDesks(dto.CashDesks))
            branch.CashDesks.Add(cash);

        return branch;
    }

    private AddressInfo MapAddress(ImportAddressDto? dto, string? fullAddress)
    {
        return new AddressInfo(
            dto?.BaseCity ?? "",
            fullAddress ?? "",
            dto?.DetailedAddress ?? "",
            dto?.GeoLocation?.Lat ?? 0,
            dto?.GeoLocation?.Long ?? 0
        );
    }

    private List<WorkSchedule> MapSchedules(List<ImportWorkScheduleDto>? schedules)
    {
        if (schedules == null) return [];

        var result = new List<WorkSchedule>();

        foreach (var sDto in schedules)
        {
            var schedule = new WorkSchedule(sDto.WorkStation);

            foreach (var dDto in sDto.Days ?? [])
            {
                var workingDay = MapWorkingDay(dDto);
                if (workingDay != null)
                    schedule.Days.Add(workingDay);
            }

            result.Add(schedule);
        }

        return result;
    }

    private WorkingDay? MapWorkingDay(ImportWorkingDayDto dto)
    {
        if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek, true, out var day))
            return null;

        var workingDay = new WorkingDay(day, dto.From, dto.To);

        foreach (var bDto in dto.Breaks ?? new())
            workingDay.Breaks.Add(MapBreak(bDto));

        return workingDay;
    }

    private BreakInterval MapBreak(ImportBreakIntervalDto dto)
    {
        return new BreakInterval(dto.From, dto.To);
    }

    private List<ContactPhone> MapPhones(List<ImportContactPhoneDto>? phones)
    {
        if (phones == null) return new();

        return phones
            .Select(p => new ContactPhone(p.OperatorCode, p.Number))
            .ToList();
    }

    private List<CashDesk> MapCashDesks(List<ImportCashDeskDto>? cashDesks)
    {
        if (cashDesks == null) return new();

        var result = new List<CashDesk>();

        foreach (var cDto in cashDesks)
        {
            var cashDesk = new CashDesk(cDto.ExternalId, cDto.Description);

            foreach (var wdDto in cDto.WorkDays ?? new())
            {
                var workDay = MapCashWorkDay(wdDto);
                if (workDay != null)
                    cashDesk.WorkDays.Add(workDay);
            }

            result.Add(cashDesk);
        }

        return result;
    }

    private CashWorkDay? MapCashWorkDay(ImportCashWorkDayDto dto)
    {
        if (!Enum.TryParse<DayOfWeek>(dto.DayOfWeek, true, out var day))
            return null;

        var workDay = new CashWorkDay(day, dto.From, dto.To);

        foreach (var bDto in dto.Breaks ?? new())
            workDay.Breaks.Add(MapBreak(bDto));

        return workDay;
    }

    //Interface method
    public List<BranchDto> MapToDto(List<Branch> branches)
    {
        return branches.Select(MapBranchToDto).ToList();
    }

    private BranchDto MapBranchToDto(Branch branch)
    {
        return new BranchDto(
            branch.Id,
            branch.Name,
            branch.Type.ToString(),
            branch.IsTemporaryClosed,
            branch.IsRegular,
            MapAddressToDto(branch.Address),
            MapSchedulesToDto(branch.Schedules),
            MapPhonesToDto(branch.Phones),
            MapCashDesksToDto(branch.CashDesks)
        );
    }

    private AddressDto MapAddressToDto(AddressInfo address)
    {
        return new AddressDto(
            address.City,
            address.FullAddress,
            address.DetailedAddress,
            address.Latitude,
            address.Longitude
        );
    }

    private List<WorkScheduleDto> MapSchedulesToDto(IEnumerable<WorkSchedule> schedules)
    {
        return schedules.Select(s => new WorkScheduleDto(
            s.WorkStation,
            s.Days.Select(d => new WorkingDayDto(
                d.Day.ToString(),
                d.From,
                d.To,
                d.Breaks.Select(b => new BreakIntervalDto(b.From, b.To)).ToList()
            )).ToList()
        )).ToList();
    }

    private List<ContactPhoneDto> MapPhonesToDto(IEnumerable<ContactPhone> phones)
    {
        return phones.Select(p => new ContactPhoneDto(
            p.OperatorCode,
            p.Number,
            p.FullNumber
        )).ToList();
    }

    private List<CashDeskDto> MapCashDesksToDto(IEnumerable<CashDesk> cashDesks)
    {
        return cashDesks.Select(c => new CashDeskDto(
            c.ExternalId,
            c.Description,
            c.WorkDays.Select(w => new CashWorkDayDto(
                w.Day.ToString(),
                w.From,
                w.To,
                w.Breaks.Select(b => new BreakIntervalDto(b.From, b.To)).ToList()
            )).ToList()
        )).ToList();
    }
}
