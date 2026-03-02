using BankMap.Application.Common;
using BankMap.Domain.Entities;
using BankMap.Domain.Interfaces.Persistence;
using MediatR;
using System.Text.Json;

namespace BankMap.Application.Features.Branches.Commands.ImportJson;

public class ImportBranchesHandler : IRequestHandler<ImportBranchesCommand, Result<int>>
{
    private readonly IBranchManager _branchManager;

    public ImportBranchesHandler(IBranchManager branchManager)
    {
        _branchManager = branchManager;
    }

    public async Task<Result<int>> Handle(ImportBranchesCommand request, CancellationToken ct)
    {
        if (request.JsonStream == null)
            return Result<int>.Failure("JSON file is required");

        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Десериализуем через корневой объект
            var root = await JsonSerializer.DeserializeAsync<DepartmentsImportRootDto>(
                request.JsonStream, options, ct);

            if (root?.List == null || !root.List.Any())
                return Result<int>.Failure("Invalid JSON structure");

            var branches = root.List.Select(dto =>
            {
                // 1. Создаем объект адреса (Value Object)
                var address = new Branch.AddressInfo(
                    dto.Address?.BaseCity ?? "",
                    dto.FullAddress ?? "",
                    dto.Address?.DetailedAddress ?? "",
                    dto.Address?.GeoLocation?.Lat ?? 0,
                    dto.Address?.GeoLocation?.Long ?? 0
                );

                // 2. Определяем тип отделения
                var branchType = Enum.TryParse<Branch.BranchType>(dto.Type, true, out var type)
                    ? type
                    : Branch.BranchType.Department;

                // 3. Вызываем конструктор Branch
                var branch = new Branch(dto.Name, branchType, address);

                // 4. Устанавливаем свойства с private set через Reflection
                // Это необходимо, так как в конструкторе этих полей нет
                var branchTypeObj = typeof(Branch);
                branchTypeObj.GetProperty(nameof(Branch.IsTemporaryClosed))?.SetValue(branch, dto.IsTemporaryClosed);
                branchTypeObj.GetProperty(nameof(Branch.IsRegular))?.SetValue(branch, dto.IsRegular);

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
                                // Добавляем перерывы
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

            // 8. Сохранение через менеджер
            int imported = await _branchManager.ImportBranchesAsync(branches, ct);
            return Result<int>.Success(imported);
        }
        catch (JsonException ex)
        {
            return Result<int>.Failure($"JSON format error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Import error: {ex.Message}");
        }
    }
}