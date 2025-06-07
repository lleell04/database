namespace BackendCode.Models
{
    public class BUYER_STORE_BOOKMARK
    {
        public string BUYER_ACCOUNT_ID { get; set; }
        public string STORE_ACCOUNT_ID { get; set; }

        public virtual BUYER BUYER { get; set; }
        public virtual STORE STORE { get; set; }
    }
}
