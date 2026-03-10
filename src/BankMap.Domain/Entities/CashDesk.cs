namespace BankMap.Domain.Entities
{
    public class CashDesk
    {
        public int ExternalId { get; private set; }
        public string Description { get; private set; } = null!;
        public List<CashWorkDay> WorkDays { get; private set; } = new();
        private CashDesk() { }
        public CashDesk(int externalId, string description)
        {
            ExternalId = externalId;
            Description = description;
        }
    }
}
