namespace BankMap.Api.Models
{
    public class DepartmentsImportRootDto
    {
        public List<BranchImportDto> List { get; set; } = new();
    }
}
