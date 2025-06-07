namespace BackendCode.Models
{
    public class SUBMIT_AUTHENTICATION
    {
        public string STORE_ACCOUNT_ID { get; set; }
        public string ADMINISTRATOR_ACCOUNT_ID { get; set; }
        public string? AUTHENTICATION { get; set; }
        public string STATUS { get; set; }
        public byte[]? PHOTO { get; set; }
        public STORE STORE { get; set; }
        public ADMINISTRATOR ADMINISTRATOR { get; set; }
    }
}
