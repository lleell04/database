namespace BackendCode.Models
{
    public class MARKET_PRODUCT
    {
        public string MARKET_ID { get; set; }
        public string PRODUCT_ID { get; set; }
        public decimal DISCOUNT_PRICE { get; set; }

        public virtual MARKET MARKET { get; set; }
        public virtual PRODUCT PRODUCT { get; set; }
    }
}
