namespace BackendCode.Models
{
    public class POST
    {
        public string POST_ID { get; set; }
        public DateTime RELEASE_TIME { get; set; }
        public string POST_CONTENT { get; set; }
        public int NUMBER_OF_LIKES { get; set; }
        public int NUMBER_OF_COMMENTS { get; set; }
        public string ACCOUNT_ID { get; set; }
        public string POST_TITLE { get; set; }
        public virtual BUYER BUYER { get; set; }
        //public  BUYER BUYER { get; set; } = null!;

        public virtual ICollection<COMMENT_POST> COMMENT_POSTS { get; set; }=new List<COMMENT_POST>();
        // 导航属性，表示此帖子下的所有评论
        public virtual ICollection<POST_IMAGE> POST_IMAGES { get; set; } = new List<POST_IMAGE>();
    }
}
