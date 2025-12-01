using System.ComponentModel.DataAnnotations;

namespace FitnessBookingSystem.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
