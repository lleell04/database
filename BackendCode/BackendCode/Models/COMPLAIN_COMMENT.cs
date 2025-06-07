namespace BackendCode.Models
{
    public class COMPLAIN_COMMENT
    {
        public string ACCOUNT_ID { get; set; }
        public string COMMENT_ID { get; set; }
        public string REPORT_ID { get; set; }

        public virtual BUYER BUYER { get; set; }
        public virtual COMMENT_POST COMMENT_POST { get; set; }
        public virtual REPORT REPORT { get; set; }
    }
}
