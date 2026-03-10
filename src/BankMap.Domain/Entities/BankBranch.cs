using BankMap.Domain.Entities.Common;
namespace BankMap.Domain.Entities;

public class Branch : AuditableEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public BranchType Type { get; private set; }
    public bool IsTemporaryClosed { get; set; }
    public bool IsRegular { get; init; }

    public AddressInfo Address { get; private set; } = null!;
    public ICollection<WorkSchedule> Schedules { get; private set; }
        = new List<WorkSchedule>();
    public ICollection<ContactPhone> Phones { get; private set; }
        = new List<ContactPhone>();
    public ICollection<CashDesk> CashDesks { get; private set; }
        = new List<CashDesk>();

    private Branch() { } //Parameterless constructor for EF Core

    public Branch(string name, BranchType type, AddressInfo address)
    {
        Name = name;
        Type = type;
        Address = address;
    }
}
      
