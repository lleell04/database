namespace BackendCode.Models
{
    public class COMPLAIN_POST
    {
        public string BUYER_ACCOUNT_ID { get; set; }
        public string POST_ID { get; set; }
        public string REPORT_ID { get; set; }

        public virtual BUYER BUYER { get; set; }
        public virtual POST POST { get; set; }
        public virtual REPORT REPORT { get; set; }
    }
}
