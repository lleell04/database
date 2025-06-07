using BackendCode.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BackendCode.DTOs.ProductDTO;
using BackendCode.DTOs;
using BackendCode.Models;

namespace NaviSearchController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NaviSearchController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly ILogger<NaviSearchController> _logger;

        public NaviSearchController(YourDbContext dbContext, ILogger<NaviSearchController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword, int type)
        {
            if (string.IsNullOrEmpty(keyword) || (type != 0 && type != 1))
            {
                return BadRequest("Keyword and valid type (0 or 1) are required.");
            }

            // 分解关键词，用于包含字符的模糊匹配
            var keywordArray = keyword.ToCharArray();
            var likePattern = string.Join("%", keywordArray);

            if (type == 0)
            {
                // 搜索商品
                var products = await _dbContext.PRODUCTS
                    .Where(p => EF.Functions.Like(p.PRODUCT_NAME, $"%{keyword}%") ||
                                EF.Functions.Like(p.PRODUCT_NAME, $"%{likePattern}%"))
                    .ToListAsync();

                var productDtos = products.Select(p => new
                {
                    ProductId = p.PRODUCT_ID,
                    ProductName = p.PRODUCT_NAME,
                    ProductPrice = p.PRODUCT_PRICE,
                    SaleOrNot = p.SALE_OR_NOT,
                    Tag = p.TAG,
                    Description = p.DESCRIBTION,
                    ProductPics = _dbContext.PRODUCT_IMAGES
                                .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                                .Select(img => new ImageModel
                                {
                                    ImageId = img.IMAGE_ID
                                })
                                .ToList(),//修改首页图
                    ProductDes = _dbContext.PRODUCT_DETAILS
                                .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                                .Select(img => new ShowPicDes { Description = img.DESCRIPTION, DetailPic = new ImageModel
                                {
                                    ImageId = img.IMAGE_ID
                                }
                                })
                                .ToList(),//修改图文详情
                    StoreId = p.ACCOUNT_ID,
                    StoreName = _dbContext.STORES.FirstOrDefault(s => s.ACCOUNT_ID == p.ACCOUNT_ID)?.STORE_NAME,
                    StoreScore = _dbContext.STORES.FirstOrDefault(s => s.ACCOUNT_ID == p.ACCOUNT_ID)?.STORE_SCORE
                })
                .OrderByDescending(p => p.ProductName == keyword)
                .ThenByDescending(p => p.ProductName.StartsWith(keyword))
                .ThenBy(p => p.ProductName.Contains(keyword))
                .ThenBy(p => p.ProductName.IndexOf(keyword))
                .ToList();

                if (!productDtos.Any())
                {
                    return NotFound("No products found for the given keyword.");
                }

                return Ok(productDtos);
            }
            else if (type == 1)
            {
                // 搜索商家
                var stores = await _dbContext.STORES
                    .Where(s => EF.Functions.Like(s.STORE_NAME, $"%{keyword}%") ||
                                EF.Functions.Like(s.STORE_NAME, $"%{likePattern}%"))
                    .ToListAsync();

                var storeDtos = stores
                    .AsEnumerable() // 将查询结果加载到内存中
                    .Select(s => new
                    {
                        StoreId = s.ACCOUNT_ID,
                        StoreName = s.STORE_NAME,
                        StoreScore = s.STORE_SCORE,
                        StorePhoto = new StoreInfoImageModel { ImageId = s.ACCOUNT_ID },
                            HomeProducts = _dbContext.PRODUCTS
                            .Where(p => p.ACCOUNT_ID == s.ACCOUNT_ID && !p.SALE_OR_NOT)
                            .Take(4)
                            .Select(p => new
                            {
                                ProductId = p.PRODUCT_ID,
                                ProductName = p.PRODUCT_NAME,
                                ProductPrice = p.PRODUCT_PRICE,
                                ProductPics = _dbContext.PRODUCT_IMAGES
                                    .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                                    .Select(img => new ImageModel
                                    {
                                        ImageId = img.IMAGE_ID
                                    })
                                    .ToList(), // 修改首页图
                                ProductDes = _dbContext.PRODUCT_DETAILS
                                    .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                                    .Select(img => new ShowPicDes
                                    {
                                        Description = img.DESCRIPTION,
                                        DetailPic = new ImageModel
                                        {
                                            ImageId = img.IMAGE_ID
                                        }
                                    })
                                    .ToList(), // 修改图文详情
                            })
                            .ToList()
                    })
                    .OrderByDescending(s => s.StoreName == keyword)
                    .ThenByDescending(s => s.StoreName.StartsWith(keyword))
                    .ThenBy(s => s.StoreName.Contains(keyword))
                    .ThenBy(s => s.StoreName.IndexOf(keyword))
                    .ToList();

                if (!storeDtos.Any())
                {
                    return NotFound("No stores found for the given keyword.");
                }

                return Ok(storeDtos);
            }

            return BadRequest("Invalid search type.");
        }
    }
}

