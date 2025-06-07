namespace BackendCode.Models
{
    public class MARKET
    {
        public string MARKET_ID { get; set; }
        public string? THEME { get; set; }
        public DateTime? START_TIME { get; set; }
        public DateTime? END_TIME { get; set; }
        public string? DETAIL { get; set; }
        public byte[]? POSTERIMG { get; set; }
        public string IMAGE_ID { get; set; }
    }
}
