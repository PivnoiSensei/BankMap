namespace BankMap.Domain.Entities
{
    public class ContactPhone
    {
        public string OperatorCode { get; private set; } = null!;
        public string Number { get; private set; } = null!;

        public string FullNumber => $"{OperatorCode} {Number}";

        private ContactPhone() { }

        public ContactPhone(string code, string number)
        {
            OperatorCode = code;
            Number = number;
        }
    }
}
