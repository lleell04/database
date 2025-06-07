namespace BackendCode.Models
{
    public class REPORT
    {
        public string REPORT_ID { get; set; }
        public DateTime? REPORT_TIME { get; set; }
        public string? REPORT_REASON { get; set; }
        public DateTime? AUDIT_TIME { get; set; }
        public string? AUDIT_RESULTS { get; set; }
        public string? ADMINISTRATOR_ACCOUNT_ID { get; set; }

        public ADMINISTRATOR ADMINISTRATOR { get; set; }
    }
}
