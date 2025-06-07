namespace BackendCode.Models
{
    public class SET_UP_MARKETPLACE
    {
        public string? MARKET_ID { get; set; }
        public string? ADMINISTRATOR_ACCOUNT_ID { get; set; }

        public MARKET MARKET { get; set; }
        public ADMINISTRATOR ADMINISTRATOR { get; set; }
    }
}
