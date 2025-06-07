namespace BackendCode.Models
{
    public class BUYER_PRODUCT_BOOKMARK
    {
        public string BUYER_ACCOUNT_ID { get; set; }
        public string PRODUCT_ID { get; set; }

        public virtual BUYER BUYER { get; set; }
        public virtual PRODUCT PRODUCT { get; set; }
    }
}

