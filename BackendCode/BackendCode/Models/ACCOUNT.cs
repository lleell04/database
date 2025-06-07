namespace BackendCode.Models
{
    public class ACCOUNT
    {
        public string ACCOUNT_ID { get; set; }
        public string? USER_NAME { get; set; }
        public string? EMAIL { get; set; }
        public string PASSWORD { get; set; }
      /*  public virtual WALLET WALLET { get; set; } // 添加导航属性*/

        public string? DESCRIBTION { get; set; }
        public byte[]? PHOTO { get; set; }

    }
}