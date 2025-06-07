namespace BackendCode.Models
{
    public class STORE_BUSINESS_DIRECTION
    {
        public string STORE_ID { get; set; }
        public string BUSINESS_TAG { get; set; }
        public int LINK_COUNT { get; set; }
        public virtual STORE STORE { get; set; }
    }
}
