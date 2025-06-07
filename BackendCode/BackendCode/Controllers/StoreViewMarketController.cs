using BackendCode.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using BackendCode.DTOs.Store;
using BackendCode.Models;
using BackendCode.DTOs;

namespace StoreViewMarket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreViewMarketController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly ILogger<StoreViewMarketController> _logger;

        public StoreViewMarketController(YourDbContext dbContext, ILogger<StoreViewMarketController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        //Get接口，传入店铺id，传回所有市集信息及该店铺参与情况
        [HttpGet]
        [Route("GetMarketsByStoreId")]
        public async Task<ActionResult<List<MarketDTO>>> GetMarketsByStoreId(string storeID)
        {
            if (string.IsNullOrEmpty(storeID))
            {
                return BadRequest("Store ID is required.");
            }

            try
            {
                var markets = await _dbContext.MARKETS.ToListAsync();
                var marketDTOs = new List<MarketDTO>();

                foreach (var market in markets)
                {
                    var marketStore = await _dbContext.MARKET_STORES
                        .Where(ms => ms.MARKET_ID == market.MARKET_ID && ms.STORE_ACCOUNT_ID == storeID)
                        .FirstOrDefaultAsync();
                    if (marketStore==null)
                    {
                        continue;
                    }
                    var marketDTO = new MarketDTO
                    {
                        MarketId = market.MARKET_ID,
                        Theme = market.THEME,
                        StartTime = market.START_TIME,
                        EndTime = market.END_TIME,
                        Detail = market.DETAIL,
                        PosterImg= _dbContext.MARKETS
                         .Where(img => img.MARKET_ID == market.MARKET_ID)
                         .Select(img => new MarketImageModel { ImageId = img.IMAGE_ID })
                         .ToList(),
                        IsStoreParticipating = marketStore.IN_OR_NOT
                    };

                    marketDTOs.Add(marketDTO);
                }

                return Ok(marketDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting markets by store ID.");
                return StatusCode(500, "Internal server error.");
            }
        }

        //Put接口，更改店铺参与市集情况
        [HttpPut]
        [Route("UpdateMarketStoreParticipation")]
        public async Task<IActionResult> UpdateMarketStoreParticipation(UpdateMarketStoreDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.MarketId) || string.IsNullOrEmpty(request.StoreId))
            {
                return BadRequest("Market ID and Store ID are required.");
            }

            try
            {
                var marketStore = await _dbContext.MARKET_STORES
                    .FirstOrDefaultAsync(ms => ms.MARKET_ID == request.MarketId && ms.STORE_ACCOUNT_ID == request.StoreId);

                if (marketStore == null)
                {
/*                    // 如果未找到匹配项，则创建一个新条目
                    marketStore = new MARKET_STORE
                    {
                        MARKET_ID = request.MarketId,
                        STORE_ACCOUNT_ID = request.StoreId,
                        IN_OR_NOT = request.InOrOut
                    };
                    _dbContext.MARKET_STORES.Add(marketStore);*/
                }
                else
                {
                    marketStore.IN_OR_NOT = request.InOrOut;
                    if (request.InOrOut)
                    {
                        var products = await _dbContext.PRODUCTS
                         .Where(st => st.ACCOUNT_ID==request.StoreId)
                         .Select(st => st.PRODUCT_ID)
                         .Distinct()
                         .ToListAsync();

                        foreach (var product in products)
                        {
                            _dbContext.MARKET_PRODUCTS.Add(new BackendCode.Models.MARKET_PRODUCT()
                            {
                                MARKET_ID = request.MarketId,
                                PRODUCT_ID = product,
                                DISCOUNT_PRICE=0.8M//写死
                            });
                        }
                    }
                    else
                    {
                        // 获取要删除的 PRODUCT_ID 列表
                        var productIds = await _dbContext.PRODUCTS
                            .Where(st => st.ACCOUNT_ID == request.StoreId)
                            .Select(st => st.PRODUCT_ID)
                            .Distinct()
                            .ToListAsync();

                        // 查询要删除的 MARKET_PRODUCTS 实体
                        var marketProductsToRemove = _dbContext.MARKET_PRODUCTS
                            .Where(mp => productIds.Contains(mp.PRODUCT_ID));

                        // 从上下文中删除这些实体
                        _dbContext.MARKET_PRODUCTS.RemoveRange(marketProductsToRemove);

                    }
                }

                await _dbContext.SaveChangesAsync();

                return Ok("Update successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating market store participation.");
                return StatusCode(500, "Internal server error.");
            }
        }

        //Get接口，传入店铺id，市集theme,传回市集信息及该店铺参与情况
        // 搜索 market
        [HttpGet("searchMarket")]
        public async Task<IActionResult> SearchMarket(string theme, string storeID)
        {
            if (string.IsNullOrEmpty(theme))
            {
                return BadRequest("Theme is required.");
            }

            try
            {
                // 分解关键词，用于包含字符的模糊匹配
                var keywordArray = theme.ToCharArray();
                var likePattern = string.Join("%", keywordArray);

                var markets = await _dbContext.MARKETS
                    .Where(m => EF.Functions.Like(m.THEME, $"%{theme}%") ||
                                EF.Functions.Like(m.THEME, $"%{likePattern}%"))
                    .ToListAsync();

                var marketDTOs = markets.Select(async m =>
                {
                    var marketStore = await _dbContext.MARKET_STORES
                        .FirstOrDefaultAsync(ms => ms.MARKET_ID == m.MARKET_ID && ms.STORE_ACCOUNT_ID == storeID);

                    if (marketStore == null)
                    {
                        // 如果未找到匹配项，则创建一个新条目，且不参与市集
                        marketStore = new MARKET_STORE
                        {
                            MARKET_ID = m.MARKET_ID,
                            STORE_ACCOUNT_ID = storeID,
                            IN_OR_NOT = false // 默认为不参与市集
                        };
                        _dbContext.MARKET_STORES.Add(marketStore);
                        await _dbContext.SaveChangesAsync();
                    }

                    return new MarketDTO
                    {
                        MarketId = m.MARKET_ID,
                        Theme = m.THEME,
                        StartTime = m.START_TIME,
                        EndTime = m.END_TIME,
                        Detail = m.DETAIL,
                        PosterImg = _dbContext.MARKETS
                         .Where(img => img.MARKET_ID == m.MARKET_ID)
                         .Select(img => new MarketImageModel { ImageId = img.IMAGE_ID })
                         .ToList(),
                        IsStoreParticipating = marketStore.IN_OR_NOT
                    };
                }).ToList();

                var marketDTOsResolved = await Task.WhenAll(marketDTOs);

                var orderedMarkets = marketDTOsResolved.OrderByDescending(m => m.Theme == theme)  // 完全匹配优先
                                                         .ThenByDescending(m => m.Theme.Contains(theme))  // 部分匹配次优
                                                         .ThenBy(m => m.Theme.StartsWith(theme))  // 前缀匹配再次
                                                         .ThenBy(m => m.Theme.IndexOf(theme))  // 名称包含关键字次之
                                                         .ToList();

                if (!orderedMarkets.Any())
                {
                    return NotFound("No markets found for the given theme.");
                }

                return Ok(orderedMarkets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching markets by theme.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
