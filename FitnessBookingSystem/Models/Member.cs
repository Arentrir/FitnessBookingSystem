using System.ComponentModel.DataAnnotations;

namespace FitnessBookingSystem.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(80)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime JoinDate { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Booking> Bookings { get; set; }
    }
}

