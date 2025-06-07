//有关收藏的DTO
namespace BackendCode.DTOs.Favourite
{
    //返回所有用户收藏商品的DTO
    public class FavouriteProductsDTO
    {
        public string? BuyerId { get; set; }
        public string? StoreId { get; set; }
        public string? ProductId { get; set; }
        public decimal? ProductPrice { get; set; }
        public bool? SaleOrNot { get; set; }
        public string? Tag { get; set; }
        public ImageModel? ProductPic { get; set; }//改成传图片ID
        public string? ProductName { get; set; }
        
    }
    public class ProductDTO
    {
        public string? ProductId { get; set; }
        public decimal? ProductPrice { get; set; }
        public ImageModel? ProductPic { get; set; }
        public string? ProductName { get; set; }
    }
    //返回所有用户收藏店铺的DTO
    public class FavouriteStoresDTO
    {
        public string? BuyerId { get; set; }
        public string? StoreId { get; set; }
        public string? StoreName { get; set; }
        public decimal? StoreScore { get; set; }
        public string? StorePic { get; set; }
        public List<ProductDTO>? Products { get; set; }

    }

    public class GFPModel
    {
        public string? userId { get; set; }
    }
}
