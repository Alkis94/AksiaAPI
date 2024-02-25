using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AksiaAPI.Models.Entities
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(200), NotNull]
        public string ApplicationName { get; set; }

        [Required, StringLength(200), NotNull]
        public string Email { get; set; }

        [StringLength(300)]
        public string? Filename { get; set; }

        public Uri? Url { get; set; }

        public DateTime Inception { get; set; }

        public double Amount { get; set; }

        [Range(0, 100)]
        public decimal? Allocation { get; set; }
    }
}
