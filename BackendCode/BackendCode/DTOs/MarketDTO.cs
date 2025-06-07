using BackendCode.DTOs.ProductDTO;

namespace BackendCode.DTOs.Market
{
    public class AllProductsDTO
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public string? ProductTag { get; set; }
        public string? StoreId { get; set; }
        public bool? SaleOrNot { get; set; }
        public string? Describtion { get; set; }
        public List<ImageModel> ProductPics { get; set; }//新增首页图ID
        public List<ShowPicDes> ProductDes { get; set; }//新增详情图文
        public string? StoreTag { get; set; }
        public decimal? Discount { get; set; }

    }

    public class GPBMModel
    {
        public string? MarketId { get; set; }
    }
}
