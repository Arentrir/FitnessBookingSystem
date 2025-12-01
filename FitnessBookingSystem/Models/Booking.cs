using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessBookingSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

