namespace BackendCode.Models
{
    public class COMMENT_COMMENT
    {
        public string COMMENT_ID { get; set; }
        public string ACCOUNT_ID { get; set; }
        public string COMMENTED_COMMENT_ID { get; set; }
        public DateTime COMMENT_TIME { get; set; }
        public string COMMENT_CONTENT { get; set; }
        public int IS_READ { get; set; }

        public virtual BUYER BUYER { get; set; }
        public virtual COMMENT_POST COMMENT_POST { get; set; }
    }
}
