namespace AksiaAPI.Models.DTOs
{
    public class TransactionInsertDto
    {
        public string? ApplicationName { get; set; }

        public string? Email { get; set; }

        public string? Filename { get; set; }

        public string? Url { get; set; }

        public DateTime? Inception { get; set; }

        public string? Amount { get; set; }

        public decimal? Allocation { get; set; }
    }
}
