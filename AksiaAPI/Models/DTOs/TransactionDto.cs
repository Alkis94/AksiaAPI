namespace AksiaAPI.Models.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }

        public string? ApplicationName { get; set; }

        public string? Email { get; set; }

        public string? Filename { get; set; }

        public Uri? Url { get; set; }

        public DateTime Inception { get; set; }

        public string? Amount { get; set; }

        public decimal? Allocation { get; set; }
    }
}
