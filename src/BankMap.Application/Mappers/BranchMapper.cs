using BankMap.Application.Common;
using BankMap.Domain.Entities;
using System.Text.Json;
using BankMap.Application.Features.Branches.Commands.ImportJson.Dto;
using BankMap.Application.Features.Branches.Queries.GetAllBranches.Dto;

namespace BankMap.Application.Mappers;

public class BranchMapper : IBranchMapper
{
    public async Task<Result<List<Branch>>> MapFromStreamAsync(Stream jsonStream, CancellationToken ct)
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            //Start JSON Deserialization through root object (Root in Dto)
            var root = await JsonSerializer.DeserializeAsync<DepartmentsImportRootDto>(
                jsonStream, options, ct);

            if (root?.List == null || !root.List.Any())
                return Result<List<Branch>>.Failure("Invalid JSON structure");

            var branches = root.List.Select(dto =>
            {
                var address = new AddressInfo(
                    dto.Address?.BaseCity ?? "",
                    dto.FullAddress ?? "",
                    dto.Address?.DetailedAddress ?? "",
                    dto.Address?.GeoLocation?.Lat ?? 0,
                    dto.Address?.GeoLocation?.Long ?? 0
                );

                var branchType = Enum.TryParse<BranchType>(dto.Type, true, out var type)
                    ? type
                    : BranchType.Department;

                var branch = new Branch(dto.Name, branchType, address)
                {
                    IsTemporaryClosed = dto.IsTemporaryClosed,
                    IsRegular = dto.IsRegular
                };

                if (dto.Schedules != null)
                {
                    foreach (var sDto in dto.Schedules)
                    {
                        var schedule = new WorkSchedule(sDto.WorkStation);
                        foreach (var dDto in sDto.Days ?? new())
                        {
                            if (Enum.TryParse<DayOfWeek>(dDto.DayOfWeek, true, out var day))
                            {
                                var workingDay = new WorkingDay(day, dDto.From, dDto.To);
                                foreach (var bDto in dDto.Breaks ?? new())
                                    workingDay.Breaks.Add(new BreakInterval(bDto.From, bDto.To));

                                schedule.Days.Add(workingDay);
                            }
                        }
                        branch.Schedules.Add(schedule);
                    }
                }

                if (dto.Phones != null)
                {
                    foreach (var pDto in dto.Phones)
                        branch.Phones.Add(new ContactPhone(pDto.OperatorCode, pDto.Number));
                }

                if (dto.CashDesks != null)
                {
                    foreach (var cDto in dto.CashDesks)
                    {
                        var cashDesk = new CashDesk(cDto.ExternalId, cDto.Description);
                        foreach (var wdDto in cDto.WorkDays ?? new())
                        {
                            if (Enum.TryParse<DayOfWeek>(wdDto.DayOfWeek, true, out var day))
                            {
                                var workingDay = new CashWorkDay(day, wdDto.From, wdDto.To);
                                foreach (var bDto in wdDto.Breaks ?? new())
                                    workingDay.Breaks.Add(new BreakInterval(bDto.From, bDto.To));

                                cashDesk.WorkDays.Add(workingDay);
                            }
                        }
                        branch.CashDesks.Add(cashDesk);
                    }
                }

                return branch;
            }).ToList();

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

    public List<BranchDto> MapToDto(List<Branch> branches)
    {
        return branches.Select(branch => new BranchDto(
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
                    d.Breaks.Select(b => new BreakIntervalDto(b.From, b.To)).ToList()
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
                    w.Breaks.Select(b => new BreakIntervalDto(b.From, b.To)).ToList()
                )).ToList()
            )).ToList()
        )).ToList();
    }
}
