using BackendCode.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using BackendCode.DTOs.Store;
using BackendCode.Models;
using Alipay.AopSdk.Core.Domain;
using BackendCode.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;

namespace ClassificationController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassificationController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly ILogger<ClassificationController> _logger;

        public ClassificationController(YourDbContext dbContext, ILogger<ClassificationController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        //获取大分类图文详情
        [HttpGet("GetCategoryByName")]
        public async Task<IActionResult> GetCategoryByName(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Category name is required.");
            }

            try
            {
                var category = await _dbContext.CATEGORYS
                    .Where(c => c.CATEGORY_NAME == categoryName)
                    .Select(c => new CategoryDetailDTO
                    {
                        CategoryDescription=c.CATEGORY_DESCRIPTION,
                        CategoryPhoto=new CategoryImageModel { ImageId= categoryName }
                    })
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return NotFound("Category not found.");
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by name.");
                return StatusCode(500, "Internal server error.");
            }
        }

        //新接口！获取有哪些大类（以及大类下的小类）
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategoriesWithSubcategories()
        {

            // 查询所有大分类
            var largeCategories = await _dbContext.CATEGORYS.ToListAsync();

            // 为每个大分类查询其子分类
            var result = largeCategories.Select(lc => new
            {
                LargeCategoryName = lc.CATEGORY_NAME,
                SubCategories = _dbContext.SUB_CATEGORYS
                    .Where(sc => sc.CATEGORY_NAME == lc.CATEGORY_NAME)
                    .OrderBy(sc => sc.SUBCATEGORY_ID) // 按ID排序
                    .Select(sc => new CategoryDTO{ SubCategoryName=sc.SUBCATEGORY_NAME ,SubCategoryId=sc.SUBCATEGORY_ID})
                    .ToList()
            }).ToList();
            if (!result.Any())
            { return NotFound(new { Message = "当前不存在任何分类！" }); }

                return Ok(new { Message = "获取大类名称及对应小类成功！", Categories = result });
        }

        //新接口！获取有哪些大类（相当于上个接口的轻量版）
        [HttpGet("GetBigCategories")]
        public async Task<IActionResult> GetBigCategoriesWithSubcategories()
        {
            // 查询所有大分类（没有ParentID的分类）
            var result = await _dbContext.CATEGORYS
                .Select(c => new { CategoryName = c.CATEGORY_NAME })
                .ToListAsync();
            if (!result.Any())
            { return NotFound(new { Message = "当前不存在任何分类！" }); }
            return Ok(new {Message="获取大类名称成功！",Categories= result });
        }

        //新接口！获取大类下有什么小类
        [HttpGet("GetSubCategoryByName")]
        public async Task<IActionResult> GetSubCategoryByName(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Category name is required.");
            }

            try
            {
                var category = await _dbContext.CATEGORYS
                   .Where(c => c.CATEGORY_NAME == categoryName)
                   .FirstOrDefaultAsync();
                if (category == null) { return NotFound(new { Message = "大分类名称填写错误！" }); }
                var subcategories = await _dbContext.SUB_CATEGORYS
                    .Where(c => c.CATEGORY_NAME == categoryName)
                    .OrderBy(sc => sc.SUBCATEGORY_ID) // 按ID排序
                    .Select(sc => new CategoryDTO{ SubCategoryId=sc.SUBCATEGORY_ID,SubCategoryName= sc.SUBCATEGORY_NAME })
                    .ToListAsync();
                if (!subcategories.Any())
                { return NotFound(new { Message = "当前不存在任何子分类！" }); }
                return Ok(new { Message=$"成功找到{categoryName}下的子分类！",Subcategories= subcategories });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by name.");
                return StatusCode(500, "Internal server error.");
            }
        }

        //重大修改 ：通过小类id获取商品
        [HttpGet("getProductsBySubTagId")]
        //[Authorize]
        public async Task<IActionResult> GetProductsBySubTagId(string subTagId)
        {
            try
            {
                var subcategory = await _dbContext.SUB_CATEGORYS
                            .Where(c => c.SUBCATEGORY_ID == subTagId)
                            .FirstOrDefaultAsync();
                if (subcategory == null)
                    return NotFound(new { Message=$"子分类ID{subTagId}不存在！"});
                if (subcategory.SUBCATEGORY_NAME == "全部")
                {
                    var products = await _dbContext.PRODUCTS
                        .Where(p => p.TAG == subcategory.CATEGORY_NAME)
                        .Select(p => new
                        {
                            ProductId = p.PRODUCT_ID,
                            ProductName = p.PRODUCT_NAME,
                            ProductPrice = p.PRODUCT_PRICE,
                            SaleOrNot=p.SALE_OR_NOT,//新增
                            SubCategoryName = _dbContext.SUB_CATEGORYS
                                .Where(sc => sc.SUBCATEGORY_ID == p.TAG)
                                .Select(sc => sc.SUBCATEGORY_NAME)
                                .FirstOrDefault(),
                            Images = _dbContext.PRODUCT_IMAGES
                                .Where(pi => pi.PRODUCT_ID == p.PRODUCT_ID)
                                .Select(pi => new ImageModel { ImageId = pi.IMAGE_ID })
                                .ToList()
                        })
                        .ToListAsync();

/*                    if (products == null || !products.Any())
                    {
                        return NotFound("No products found for the given tag.");
                    }*/

                    return Ok(products);
                }
                else
                {
                    var products = await _dbContext.PRODUCTS
                        .Where(p => p.SUB_TAG == subTagId)
                        .Select(p => new
                        {
                            ProductId = p.PRODUCT_ID,
                            ProductName = p.PRODUCT_NAME,
                            ProductPrice = p.PRODUCT_PRICE,
                            SaleOrNot = p.SALE_OR_NOT,//新增
                            SubCategoryName = _dbContext.SUB_CATEGORYS
                                .Where(sc => sc.SUBCATEGORY_ID == p.TAG)
                                .Select(sc => sc.SUBCATEGORY_NAME)
                                .FirstOrDefault(),
                            Images = _dbContext.PRODUCT_IMAGES
                                .Where(pi => pi.PRODUCT_ID == p.PRODUCT_ID)
                                .Select(pi => new ImageModel { ImageId = pi.IMAGE_ID })
                                .ToList()
                        })
                        .ToListAsync();

/*                    if (products == null || !products.Any())
                    {
                        return NotFound("No products found for the given tag.");
                    }*/

                    return Ok(products);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //新接口！获取小类的名称（相当于上个接口的轻量版）
        [HttpGet("GetSubCategoryName")]
        public async Task<IActionResult> GetSubCategoriesName(string subTagId)
        {
            var result = await _dbContext.SUB_CATEGORYS
                .Where(c=>c.SUBCATEGORY_ID== subTagId)
                .Select(c => new { CategoryName = c.SUBCATEGORY_NAME })
                .FirstOrDefaultAsync();
            if(result == null)
                return NotFound(new { Message = "未找到此ID对应的子分类！请检查！", SubCategoryName = result });

            return Ok(new { Message = "获取小类名称成功！", SubCategoryName = result });
        }


        //自用接口：上传大类的图片
        [HttpPost("SetCategoryPic")]
        public async Task<IActionResult> SetCatrgoryPic([FromForm]UploadCategoryPic model)
        {
            try
            {
                var category = await _dbContext.CATEGORYS
                    .Where(c => c.CATEGORY_NAME == model.CategoryName)
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    return NotFound("Category not found.");
                }
                var ms = new MemoryStream();
                await model.CategoryPhoto.CopyToAsync(ms);
                var imageData = ms.ToArray();
                category.CATEGORY_PIC = imageData;  
                await _dbContext.SaveChangesAsync();
                return Ok(new {Message="上传分类图片成功！"});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by name.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
