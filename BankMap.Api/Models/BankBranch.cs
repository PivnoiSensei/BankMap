using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankMap.Api.Models
{
    // Основная таблица для карты и фильтров
    public class BankBranch
    {
        [Key]
        public int Id { get; set; }

        // ID из внешнего JSON
        public int ExternalDepartmentId { get; set; }

        // department | atm | terminal
        [Required]
        [MaxLength(32)]
        public string DepartmentType { get; set; } = "department";

        [MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(128)]
        public string BaseCity { get; set; } = string.Empty;

        [MaxLength(512)]
        public string FullAddress { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsTemporaryClosed { get; set; }

        // Полный JSON отделения (timeTables, phones, services и т.д.)
        [Column(TypeName = "nvarchar(max)")] 
        public string DataJson { get; set; } = "{}";

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }


    // DTO для импорта JSON
    public class DepartmentsImportRootDto
    {
        public List<BranchImportDto> List { get; set; } = new();
    }

    public class BranchImportDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentType { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public bool IsTemporaryClosed { get; set; }

        public string FullAddress { get; set; } = "";

        public AddressDto Address { get; set; } = null!;

        public List<string> ExtraServices { get; set; } = new();
        public List<TimeTableDto> TimeTables { get; set; } = new();

        public List<CashDepartmentDto> CashDepartments { get; set; } = new();
    }

    public class AddressDto
    {
        public string BaseCity { get; set; } = "";
        public string City { get; set; } = "";
        public string? DetailedAddress { get; set; }
        public GeoLocationDto GeoLocation { get; set; } = null!;
    }

    public class GeoLocationDto
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }

    public class TimeTableDto
    {
        public string Workstation { get; set; } = ""; // department, cashDepartment
        public List<WorkDayDto> WorkDays { get; set; } = new();
    }

    public class WorkDayDto
    {
        public string WorkingDay { get; set; } = ""; 
        public string WorkFrom { get; set; } = "";
        public string WorkTo { get; set; } = "";
        public List<BreakDto> Breaks { get; set; } = new();
    }

    public class BreakDto
    {
        public string BreakFrom { get; set; } = "";
        public string BreakTo { get; set; } = "";
    }

    //Separate cash
    public class CashDepartmentDto
    {
        public int CashId { get; set; }

        public string CashDescription { get; set; } = "";

        public List<CashWorkDayDto> WorkDays { get; set; } = new();
    }

    public class CashWorkDayDto
    {
        public string DayOfWeek { get; set; } = ""; 

        public string WorkFrom { get; set; } = "";
        public string WorkTo { get; set; } = "";

        public List<BreakDto> Breaks { get; set; } = new();
    }
}