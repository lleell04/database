using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendCode.Data;
using BackendCode.DTOs.Favourite;
using BackendCode.Models;
using BackendCode.DTOs;

namespace Favourite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavouriteController : ControllerBase
    {
        private readonly YourDbContext _dbContext;

        public FavouriteController(YourDbContext context)
        {
            _dbContext = context;
        }

        [HttpPost("GetFavoriteProducts")]
        public async Task<IActionResult> GetFavoriteProductsAsync([FromBody] GFPModel model)
        {
            // 获取用户喜欢的商品ID
            var favoriteProductIds = await _dbContext.BUYER_PRODUCT_BOOKMARKS
                .Where(bp => bp.BUYER_ACCOUNT_ID == model.userId)
                .Select(bp => bp.PRODUCT_ID)
                .ToListAsync();

            // 通过商品ID获取商品详细信息
            var favoriteProducts = await _dbContext.PRODUCTS
                .Where(p => favoriteProductIds.Contains(p.PRODUCT_ID))
                .ToListAsync();

            if (favoriteProducts == null || !favoriteProducts.Any())
            {
                return NotFound("No products found for the given user ID.");
            }

            var productDtos = favoriteProducts.Select(p => new FavouriteProductsDTO
            {
                BuyerId = model.userId,
                ProductName=p.PRODUCT_NAME,
                StoreId = p.ACCOUNT_ID,
                ProductId=p.PRODUCT_ID,
                ProductPrice=p.PRODUCT_PRICE,
                SaleOrNot=p.SALE_OR_NOT,
                Tag=p.TAG,
                ProductPic = new ImageModel
                {
                       ImageId = _dbContext.PRODUCT_IMAGES
                             .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                             .Select(img => img.IMAGE_ID)
                            .FirstOrDefault()
                }
            }) .ToList();
            // 返回商品信息
            return Ok(productDtos);
        }


        [HttpPost("GetFavoriteStores")]
        public async Task<IActionResult> GetFavoriteStoresAsync([FromBody] GFPModel model)
        {
            // 获取用户喜欢的商店ID
            var favoriteStoreIds = await _dbContext.BUYER_STORE_BOOKMARKS
                .Where(bp => bp.BUYER_ACCOUNT_ID == model.userId)
                .Select(bp => bp.STORE_ACCOUNT_ID)
                .ToListAsync();

            // 通过商店ID获取商店详细信息
            var favoriteStores = await _dbContext.STORES
                .Where(p => favoriteStoreIds.Contains(p.ACCOUNT_ID))
                .ToListAsync();

            if (favoriteStores == null || !favoriteStores.Any())
            {
                return NotFound("No stores found for the given user ID.");
            }

            var productDtos = await Task.WhenAll(favoriteStores.Select(async store =>
            {
                var products = await _dbContext.PRODUCTS
                    .Where(mp => mp.ACCOUNT_ID == store.ACCOUNT_ID)
                    .OrderBy(mp => mp.PRODUCT_ID) // Optionally order by price or any other criteria
                    .Take(4) // Get the top 4 products
                    .Select(mp => new ProductDTO
                    {
                        ProductId = mp.PRODUCT_ID,
                        ProductPrice = mp.PRODUCT_PRICE,
                        ProductName=mp.PRODUCT_NAME,
                        ProductPic = new ImageModel
                        {
                            ImageId = _dbContext.PRODUCT_IMAGES
                             .Where(img => img.PRODUCT_ID == mp.PRODUCT_ID)
                             .Select(img => img.IMAGE_ID)
                            .FirstOrDefault()
                        }

                    })
                    .ToListAsync();

                return new FavouriteStoresDTO
                {
                    BuyerId = model.userId,
                    StoreId = store.ACCOUNT_ID,
                    StoreName = store.STORE_NAME,
                    StoreScore = store.STORE_SCORE,
                    Products = products,
                    StorePic = store.PHOTO != null ? Convert.ToBase64String(store.PHOTO) : null,
                };
            }));

            // 返回商品信息
            return Ok(productDtos);
        }

        /***************************************/
        /* 收藏店铺接口                        */
        /* 传入{userid,storeid}                */
        /* 在数据库中添加收藏信息              */
        /***************************************/
        [HttpPost("BookmarkStore")]
        public async Task<IActionResult> BookmarkStoreAsync(string userId, string storeId)
        {
            /* 查询商家信息 */
            var store = await _dbContext.STORES.FirstOrDefaultAsync(s => s.ACCOUNT_ID == storeId);
            if (store == null) //商家ID不存在
            {
                return NotFound("未找到该商家信息");
            }

            /* 查询买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(b => b.ACCOUNT_ID == userId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到该买家信息");
            }

            /* 查询是否已经收藏了该店铺 */
            var isBookmarked = await _dbContext.BUYER_STORE_BOOKMARKS
                .AnyAsync(b => b.BUYER_ACCOUNT_ID == buyer.ACCOUNT_ID && b.STORE_ACCOUNT_ID == store.ACCOUNT_ID);
            if (isBookmarked)
            {
                return BadRequest("已收藏该店铺");
            }

            /* 添加收藏信息 */
            var bookmark = new BUYER_STORE_BOOKMARK
            {
                BUYER_ACCOUNT_ID = buyer.ACCOUNT_ID,
                STORE_ACCOUNT_ID = store.ACCOUNT_ID
            };
            _dbContext.BUYER_STORE_BOOKMARKS.Add(bookmark);

            await _dbContext.SaveChangesAsync(); //保存更改到数据库

            return Ok("收藏店铺成功"); //返回收藏成功
        }

        /***************************************/
        /* 收藏商品接口                        */
        /* 传入{userid,productid}              */
        /* 在数据库中添加收藏信息              */
        /***************************************/
        [HttpPost("BookmarkProduct")]
        public async Task<IActionResult> BookmarkProductAsync(string userId, string productId)
        {
            /* 查询商品信息 */
            var product = await _dbContext.PRODUCTS.FirstOrDefaultAsync(s => s.PRODUCT_ID == productId);
            if (product == null) //商品ID不存在
            {
                return NotFound("未找到该商品信息");
            }

            /* 查询买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(b => b.ACCOUNT_ID == userId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到该买家信息");
            }

            /* 查询是否已经收藏了该商品 */
            var isBookmarked = await _dbContext.BUYER_PRODUCT_BOOKMARKS
                .AnyAsync(b => b.BUYER_ACCOUNT_ID == buyer.ACCOUNT_ID && b.PRODUCT_ID == product.PRODUCT_ID);
            if (isBookmarked)
            {
                return BadRequest("已收藏该商品");
            }

            /* 添加收藏信息 */
            var bookmark = new BUYER_PRODUCT_BOOKMARK
            {
                BUYER_ACCOUNT_ID = buyer.ACCOUNT_ID,
                PRODUCT_ID = product.PRODUCT_ID
            };
            _dbContext.BUYER_PRODUCT_BOOKMARKS.Add(bookmark);

            await _dbContext.SaveChangesAsync(); //保存更改到数据库

            return Ok("收藏商品成功"); //返回收藏成功
        }

        /***************************************/
        /* 取消收藏店铺接口                    */
        /* 传入{userid,storeid}                */
        /* 在数据库中删除收藏信息              */
        /***************************************/
        [HttpDelete("UnbookmarkStore")]
        public async Task<IActionResult> UnbookmarkStoreAsync(string userId, string storeId)
        {
            /* 查询商家信息 */
            var store = await _dbContext.STORES.FirstOrDefaultAsync(s => s.ACCOUNT_ID == storeId);
            if (store == null) //商家ID不存在
            {
                return NotFound("未找到该商家信息");
            }

            /* 查询买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(b => b.ACCOUNT_ID == userId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到该买家信息");
            }

            /* 查询是否已经收藏了该店铺 */
            var bookmark = await _dbContext.BUYER_STORE_BOOKMARKS
                .FirstOrDefaultAsync(b => b.BUYER_ACCOUNT_ID == buyer.ACCOUNT_ID && b.STORE_ACCOUNT_ID == store.ACCOUNT_ID);
            if (bookmark == null)
            {
                return BadRequest("未收藏该店铺");
            }

            _dbContext.BUYER_STORE_BOOKMARKS.Remove(bookmark); //删除收藏信息

            await _dbContext.SaveChangesAsync(); //保存更改到数据库

            return Ok("取消收藏店铺成功"); //返回取消收藏成功
        }

        /***************************************/
        /* 取消收藏商品接口                    */
        /* 传入{userid,productid}              */
        /* 在数据库中取消收藏信息              */
        /***************************************/
        [HttpDelete("UnbookmarkProduct")]
        public async Task<IActionResult> UnbookmarkProductAsync(string userId, string productId)
        {
            /* 查询商品信息 */
            var product = await _dbContext.PRODUCTS.FirstOrDefaultAsync(s => s.PRODUCT_ID == productId);
            if (product == null) //商品ID不存在
            {
                return NotFound("未找到该商品信息");
            }

            /* 查询买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(b => b.ACCOUNT_ID == userId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到该买家信息");
            }

            /* 查询是否已经收藏了该商品 */
            var bookmark = await _dbContext.BUYER_PRODUCT_BOOKMARKS
                .FirstOrDefaultAsync(b => b.BUYER_ACCOUNT_ID == buyer.ACCOUNT_ID && b.PRODUCT_ID == product.PRODUCT_ID);
            if (bookmark == null)
            {
                return BadRequest("未收藏该商品");
            }

            _dbContext.BUYER_PRODUCT_BOOKMARKS.Remove(bookmark); //删除收藏信息

            await _dbContext.SaveChangesAsync(); //保存更改到数据库

            return Ok("取消收藏商品成功"); //返回取消收藏成功
        }

        /***************************************/
        /* 检查买家是否收藏商品                */
        /* 传入{userid,productid}              */
        /* 返回买家是否收藏该商品              */
        /***************************************/
        [HttpGet("IsProductBookmarked")]
        public async Task<IActionResult> IsProductBookmarkedAsync(string userId, string productId)
        {
            /* 查询商品信息 */
            var product = await _dbContext.PRODUCTS.FirstOrDefaultAsync(s => s.PRODUCT_ID == productId);
            if (product == null) //商品ID不存在
            {
                return NotFound("未找到该商品信息");
            }

            /* 查询买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(b => b.ACCOUNT_ID == userId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到该买家信息");
            }

            /* 查询是否已经收藏了该商品 */
            var isBookmarked = await _dbContext.BUYER_PRODUCT_BOOKMARKS
                .AnyAsync(b => b.BUYER_ACCOUNT_ID == userId && b.PRODUCT_ID == productId);

            return Ok(isBookmarked);
        }

        /***************************************/
        /* 检查买家是否收藏店铺                */
        /* 传入{userid,storeid}                */
        /* 返回买家是否收藏该店铺              */
        /***************************************/
        [HttpGet("IsStoreBookmarked")]
        public async Task<IActionResult> IsStoreBookmarkedAsync(string userId, string storeId)
        {
            /* 查询商家信息 */
            var store = await _dbContext.STORES.FirstOrDefaultAsync(s => s.ACCOUNT_ID == storeId);
            if (store == null) //商家ID不存在
            {
                return NotFound("未找到该商家信息");
            }

            /* 查询买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(b => b.ACCOUNT_ID == userId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到该买家信息");
            }

            /* 查询是否已经收藏了该店铺 */
            var isBookmarked = await _dbContext.BUYER_STORE_BOOKMARKS
                .AnyAsync(b => b.BUYER_ACCOUNT_ID == userId && b.STORE_ACCOUNT_ID == storeId);

            return Ok(isBookmarked);
        }
    }
}