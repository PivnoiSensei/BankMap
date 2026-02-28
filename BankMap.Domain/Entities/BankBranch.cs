namespace BankMap.Domain.Entities;

public class Branch
{
    public int Id { get; private set; }

    public string Name { get; private set; } = null!;

    public BranchType Type { get; private set; }

    public bool IsTemporaryClosed { get; private set; }

    public bool IsRegular { get; private set; }

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

    //Створені вкладені об'єкти є value objects і не матимуть окремого id

    public class AddressInfo
    {
        public string City { get; private set; } = null!;
        public string FullAddress { get; private set; } = null!;
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        private AddressInfo() { }

        public AddressInfo(string city, string fullAddress,
            double lat, double lng)
        {
            City = city;
            FullAddress = fullAddress;
            Latitude = lat;
            Longitude = lng;
        }
    }

    public class WorkSchedule
    {
        public string WorkStation { get; private set; } = null!;

        public List<WorkingDay> Days { get; private set; } = new();

        private WorkSchedule() { }

        public WorkSchedule(string workstation)
        {
            WorkStation = workstation;
        }

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

        public class CashWorkDay
        {
            public DayOfWeek Day { get; private set; }
            public string From { get; private set; } = null!;
            public string To { get; private set; } = null!;

            private CashWorkDay() { }

            public CashWorkDay(DayOfWeek day, string from, string to)
            {
                Day = day;
                From = from;
                To = to;
            }
        }
    }

    public enum BranchType
    {
        Department,
        Atm,
        Terminal
    }
}

