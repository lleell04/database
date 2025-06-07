using BackendCode.DTOs;

namespace BackendCode.Models
{
    public class CATEGORY
    {
        public string CATEGORY_NAME { get; set; }
        public byte[]? CATEGORY_PIC { get; set; }
        //public ImageModel CATEGORY_PIC { get; set; }
        public string? CATEGORY_DESCRIPTION { get; set; }
        public virtual ICollection<SUB_CATEGORY> SUB_CATEGORYS { get; set; } = new List<SUB_CATEGORY>();/////新加入
        public virtual ICollection<PRODUCT> PRODUCTS { get; set; } = new List<PRODUCT>();/////新加入
    }
}