namespace BankMap.Domain.Entities
{
    public class WorkSchedule
    {
        public string WorkStation { get; private set; } = null!;
        public List<WorkingDay> Days { get; private set; } = new();
        private WorkSchedule() { }
        public WorkSchedule(string workstation)
        {
            WorkStation = workstation;
        }
    }
}
