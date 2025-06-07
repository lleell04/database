using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendCode.Data;
using BackendCode.DTOs.Market;
using BackendCode.DTOs.ProductDTO;
using BackendCode.DTOs;

namespace Market.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly YourDbContext _dbContext;

        public MarketController(YourDbContext context)
        {
            _dbContext = context;
        }


        [HttpPost("GetProductsByMarket")]
        public async Task<IActionResult> GetProductsByMarketAsync([FromBody]GPBMModel model)
        {
            try
            {
                var query = from m_p in _dbContext.MARKET_PRODUCTS
                            join p in _dbContext.PRODUCTS on m_p.PRODUCT_ID equals p.PRODUCT_ID
                            where (m_p.MARKET_ID == model.MarketId&&p.SALE_OR_NOT==false)
                            select new AllProductsDTO
                            {
                                ProductId = m_p.PRODUCT_ID,
                                ProductName = p.PRODUCT_NAME,
                                ProductPrice = p.PRODUCT_PRICE,
                                ProductTag = p.TAG,
                                //修改首页图
                                ProductPics = _dbContext.PRODUCT_IMAGES
                                 .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                                  .Select(img => new ImageModel
                                  {
                                     ImageId = img.IMAGE_ID
                                          })
                                 .ToList(),

                                ProductDes = _dbContext.PRODUCT_DETAILS
                                .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                                .Select(img => new ShowPicDes { Description = img.DESCRIPTION,
                                    DetailPic = new ImageModel
                                    {
                                        ImageId = img.IMAGE_ID
                                    }
                                })
                                .ToList(),//修改图文详情
                                SaleOrNot = p.SALE_OR_NOT,
                                StoreId = p.ACCOUNT_ID,
                                StoreTag = p.STORE_TAG,
                                Describtion = p.DESCRIBTION,
                                Discount = m_p.DISCOUNT_PRICE,

                            };
                var res = await query.ToListAsync();

                if (res == null || !res.Any())
                {
                    return NotFound("该市集没有对应的商品");
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"访问失败: {ex.Message}");
            }
            
        }


    }
}

