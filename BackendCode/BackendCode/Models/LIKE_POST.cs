namespace BackendCode.Models
{
    public class LIKE_POST
    {
        public string BUYER_ACCOUNT_ID { get; set; }
        public string POST_ID { get; set; }

        public virtual BUYER BUYER { get; set; }
        public virtual POST POST { get; set; }
    }
}
