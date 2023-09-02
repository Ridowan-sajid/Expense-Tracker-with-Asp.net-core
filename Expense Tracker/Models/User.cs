using System.ComponentModel.DataAnnotations;

namespace Expense_Tracker.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public ICollection<Item>? Items { get; set; }

    }
}
