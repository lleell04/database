namespace BackendCode.Models
{
    public class PRODUCT
    {
        public string PRODUCT_ID { get; set; }
        public string? PRODUCT_NAME { get; set; }
        public decimal PRODUCT_PRICE { get; set; }
        public bool SALE_OR_NOT { get; set; }
        public string? TAG { get; set; }
        public string? DESCRIBTION { get; set; }
        public string? ACCOUNT_ID { get; set; }
        public string? STORE_TAG { get; set; }
        public string? SUB_TAG { get; set; }
        public STORE STORE { get; set; }
        public virtual CATEGORY CATEGORY { get; set; }
        public virtual SUB_CATEGORY SUB_CATEGORY { get; set; }
        public virtual ICollection<PRODUCT_IMAGE> PRODUCT_IMAGES { get; set; } = new List<PRODUCT_IMAGE>();
        public virtual ICollection<PRODUCT_DETAIL> PRODUCT_DETAILS { get; set; } = new List<PRODUCT_DETAIL>();
    }
}