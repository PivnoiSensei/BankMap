namespace BankMap.Domain.Entities
{
    public class CashWorkDay
    {
        public DayOfWeek Day { get; private set; }
        public string From { get; private set; } = null!;
        public string To { get; private set; } = null!;

        public List<BreakInterval> Breaks { get; private set; } = new();

        private CashWorkDay() { }

        public CashWorkDay(DayOfWeek day, string from, string to)
        {
            Day = day;
            From = from;
            To = to;
        }
    }
}
