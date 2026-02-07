using System.ComponentModel.DataAnnotations;
//This code defines schema of BankBranch table in DB
namespace BankMap.Api.Models
{
    public class BankBranch
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Status { get; set; } = "Active";
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
