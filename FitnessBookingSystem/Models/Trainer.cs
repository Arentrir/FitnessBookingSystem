using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FitnessBookingSystem.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }

        [MaxLength(100)]
        public required string Specialization { get; set; }

        [Required]
        public int ExperienceYears { get; set; }

        public double? Rating { get; set; }

        [MaxLength(200)]
        public string? PhotoPath { get; set; }


        public DateTime CreatedAt { get; set; }

        public ICollection<Class>? Classes { get; set; }


    }
}
