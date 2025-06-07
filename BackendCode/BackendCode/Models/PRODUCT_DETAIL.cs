namespace BackendCode.Models
{
    public class PRODUCT_DETAIL
    {
        public string ? IMAGE_ID { get; set; }
        public string ? PRODUCT_ID { get; set; }
        public byte[]?  IMAGE { get; set; }
        public string ? DESCRIPTION { get; set; }
        public virtual PRODUCT PRODUCT { get; set; }

    }
}