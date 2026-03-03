using BankMap.Application.Common;
using BankMap.Application.Features.Branches.Commands.ImportJson;
using BankMap.Domain.Entities;
using System.Text.Json;

namespace BankMap.Application.Services;

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
                var address = new Branch.AddressInfo(
                    dto.Address?.BaseCity ?? "",
                    dto.FullAddress ?? "",
                    dto.Address?.DetailedAddress ?? "",
                    dto.Address?.GeoLocation?.Lat ?? 0,
                    dto.Address?.GeoLocation?.Long ?? 0
                );

                var branchType = Enum.TryParse<Branch.BranchType>(dto.Type, true, out var type)
                    ? type
                    : Branch.BranchType.Department;

                var branch = new Branch(dto.Name, branchType, address)
                {
                    IsTemporaryClosed = dto.IsTemporaryClosed,
                    IsRegular = dto.IsRegular
                };

                // 5. Маппинг Schedules
                if (dto.Schedules != null)
                {
                    foreach (var sDto in dto.Schedules)
                    {
                        var schedule = new Branch.WorkSchedule(sDto.WorkStation);
                        foreach (var dDto in sDto.Days ?? new())
                        {
                            if (Enum.TryParse<DayOfWeek>(dDto.DayOfWeek, true, out var day))
                            {
                                var workingDay = new Branch.WorkSchedule.WorkingDay(day, dDto.From, dDto.To);
                                foreach (var bDto in dDto.Breaks ?? new())
                                    workingDay.Breaks.Add(new Branch.WorkSchedule.BreakInterval(bDto.From, bDto.To));

                                schedule.Days.Add(workingDay);
                            }
                        }
                        branch.Schedules.Add(schedule);
                    }
                }

                // 6. Маппинг Телефонов
                if (dto.Phones != null)
                {
                    foreach (var pDto in dto.Phones)
                        branch.Phones.Add(new Branch.ContactPhone(pDto.OperatorCode, pDto.Number));
                }

                // 7. Маппинг Касс
                if (dto.CashDesks != null)
                {
                    foreach (var cDto in dto.CashDesks)
                    {
                        var cashDesk = new Branch.CashDesk(cDto.ExternalId, cDto.Description);
                        foreach (var wdDto in cDto.WorkDays ?? new())
                        {
                            if (Enum.TryParse<DayOfWeek>(wdDto.DayOfWeek, true, out var day))
                            {
                                var workingDay = new Branch.CashDesk.CashWorkDay(day, wdDto.From, wdDto.To);
                                foreach (var bDto in wdDto.Breaks ?? new())
                                    workingDay.Breaks.Add(new Branch.CashDesk.BreakInterval(bDto.From, bDto.To));

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
}
