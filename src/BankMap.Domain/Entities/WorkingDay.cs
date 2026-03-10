namespace BankMap.Domain.Entities
{
    public class WorkingDay
    {
        public DayOfWeek Day { get; private set; }
        public string From { get; private set; } = null!;
        public string To { get; private set; } = null!;

        public List<BreakInterval> Breaks { get; private set; } = new();

        private WorkingDay() { }

        public WorkingDay(DayOfWeek day, string from, string to)
        {
            Day = day;
            From = from;
            To = to;
        }
    }
}
