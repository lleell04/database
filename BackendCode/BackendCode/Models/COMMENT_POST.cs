namespace BackendCode.Models
{
    public class COMMENT_POST
    {
        public string COMMENT_ID { get; set; }
        public string BUYER_ACCOUNT_ID { get; set; }
        public string POST_ID { get; set; }
        public DateTime EVALUATION_TIME { get; set; }
        public string EVALUATION_CONTENT { get; set; }
        public int NUMBER_OF_SUBCOMMENTS { get; set; }
        public int IS_READ { get; set; }
        public virtual BUYER BUYER { get; set; }
        public virtual POST POST { get; set; }
        public virtual ICollection<COMMENT_COMMENT> COMMENT_COMMENTS { get; set; } = new List<COMMENT_COMMENT>();
    }
}