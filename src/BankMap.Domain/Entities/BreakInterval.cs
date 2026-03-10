namespace BankMap.Domain.Entities
{

    public class BreakInterval
    {
        public string From { get; private set; } = null!;
        public string To { get; private set; } = null!;

        private BreakInterval() { }

        public BreakInterval(string from, string to)
        {
            From = from;
            To = to;
        }
    }
}
