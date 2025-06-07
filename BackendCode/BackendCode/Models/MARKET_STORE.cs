namespace BackendCode.Models
{
    public class MARKET_STORE
    {
        public string MARKET_ID { get; set; }
        public string STORE_ACCOUNT_ID { get; set; }
        public bool IN_OR_NOT { get; set; }

        public STORE STORE { get; set; }
        public MARKET MARKET { get; set; }
    }
}
