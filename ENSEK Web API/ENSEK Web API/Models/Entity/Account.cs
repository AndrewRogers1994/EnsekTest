using System.ComponentModel.DataAnnotations;

namespace ENSEK_Web_API.Models
{
    public class Account
    {
        [Key]
        public string AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}