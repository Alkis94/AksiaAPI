namespace AksiaAPI.Models.Business
{
    public class BusinessException : Exception
    {
        public string Type { get; set; }
        public string Detail { get; set; }
        public int Status { get; set; }
 
        public BusinessException(string detail, int status = StatusCodes.Status400BadRequest )
        {
            Type = "buisness-exception";
            Detail = detail;
            Status = status;
        }
    }
}
