using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace FitnessBookingSystem.Models
{
    public class Class
    {
        public int ClassId { get; set; }

        [Required]
        [MaxLength(35, ErrorMessage = "Title cannot exceed 35 characters.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string? Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Duration cannot be negative.")]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Capacity cannot be negative.")]
        public int Capacity { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [ForeignKey("TrainerId")]
        public Trainer? Trainer { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; } = string.Empty;


    }
}

