using BackendCode.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using BackendCode.DTOs.ProductDTO;
using BackendCode.Models;
using Yitter.IdGenerator;
using Alipay.AopSdk.Core.Domain;
using BackendCode.DTOs.PostModel;
using BackendCode.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Hosting;

namespace StoreViewProductController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreViewProductController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly ILogger<StoreViewProductController> _logger;

        public StoreViewProductController(YourDbContext dbContext, ILogger<StoreViewProductController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        //Get接口，传入store_id, view_type，view_type1 2 3分别代表全部，售出，未售
        [HttpGet("GetProductsByStoreIdAndViewType")]
        public async Task<IActionResult> GetProductsByStoreIdAndViewType(string StoreId, int ViewType)
        {
            if (string.IsNullOrEmpty(StoreId))
            {
                return BadRequest("Store ID is required.");
            }

            if (ViewType < 1 || ViewType > 3)
            {
                return BadRequest("Invalid view type. Allowed values are 1, 2, or 3.");
            }

            try
            {
                IQueryable<PRODUCT> query = _dbContext.PRODUCTS.Where(p => p.ACCOUNT_ID == StoreId);

                if (ViewType == 2)
                {
                    query = query.Where(p => p.SALE_OR_NOT == true);
                }
                else if (ViewType == 3)
                {
                    query = query.Where(p => p.SALE_OR_NOT == false);
                }

                var products = await query.ToListAsync();

                var productDTOs = products.Select(p => new ShowProductDTO
                {
                    ProductId = p.PRODUCT_ID,
                    ProductName = p.PRODUCT_NAME,
                    ProductPrice = p.PRODUCT_PRICE,
                    SaleOrNot = p.SALE_OR_NOT,
                    Tag = p.TAG,
                    SubTag = p.SUB_TAG,//新增！
                    Description = p.DESCRIBTION,
                    StoreTag = p.STORE_TAG,
                   ProductPics = _dbContext.PRODUCT_IMAGES
                 .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                 .Select(img => new ImageModel { ImageId=img.IMAGE_ID })
                 .ToList(),
                   ProductDes= _dbContext.PRODUCT_DETAILS
                 .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                 .Select(img =>new ShowPicDes { Description = img.DESCRIPTION, DetailPic = new ImageModel { ImageId = img.IMAGE_ID } } )
                 .ToList()
                }).ToList();

                return Ok(productDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products by store ID and view type.");
                return StatusCode(500, "Internal server error.");
            }
        }

        //Get接口传入storeid keyword匹配对应商品
        [HttpGet("search")]
        public async Task<IActionResult> Search(string storeId, string keyword)
        {
            if (string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(keyword))
            {
                return BadRequest("Store ID and Keyword are required.");
            }

            // 分解关键词，用于包含字符的模糊匹配
            var keywordArray = keyword.ToCharArray();
            var likePattern = string.Join("%", keywordArray);

            var products = await _dbContext.PRODUCTS
                .Where(p => p.ACCOUNT_ID == storeId &&
                            (EF.Functions.Like(p.PRODUCT_NAME, $"%{keyword}%") ||
                             EF.Functions.Like(p.PRODUCT_NAME, $"%{likePattern}%")))
                .ToListAsync();

            var productDtos = products.Select(p => new ShowProductDTO
            {
                ProductId = p.PRODUCT_ID,
                ProductName = p.PRODUCT_NAME,
                ProductPrice = p.PRODUCT_PRICE,
                SaleOrNot = p.SALE_OR_NOT,
                Tag = p.TAG,
                SubTag = p.SUB_TAG,//新增！
                Description = p.DESCRIBTION,
                StoreTag = p.STORE_TAG,
                ProductPics = _dbContext.PRODUCT_IMAGES
                 .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                .Select(img => new ImageModel { ImageId = img.IMAGE_ID })
                 .ToList(),
                ProductDes = _dbContext.PRODUCT_DETAILS
                 .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                 .Select(img => new ShowPicDes { Description = img.DESCRIPTION, DetailPic = new ImageModel { ImageId = img.IMAGE_ID } })
                 .ToList()
            })
            .OrderByDescending(p => p.ProductName == keyword)  // 完全匹配优先
            .ThenByDescending(p => p.ProductName.StartsWith(keyword))  // 前缀匹配次优
            .ThenBy(p => p.ProductName.Contains(keyword))             // 部分匹配再次
            .ThenBy(p => p.ProductName.IndexOf(keyword))              // 名称包含关键字次之
            .ToList();

            if (!productDtos.Any())
            {
                return NotFound("No products found for the given store ID and keyword.");
            }

            return Ok(productDtos);
        }

        //Get接口传入storeid storetag匹配对应商品
        [HttpGet("searchByStoreTag")]
        public async Task<IActionResult> SearchByTag(string storeId, string storeTag)
        {
            if (string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(storeTag))
            {
                return BadRequest("Store ID and Store Tag are required.");
            }

            var products = await _dbContext.PRODUCTS
                .Where(p => p.ACCOUNT_ID == storeId && p.STORE_TAG == storeTag)
                .Select(p => new ShowProductDTO
                {
                    ProductId = p.PRODUCT_ID,
                    ProductName = p.PRODUCT_NAME,
                    ProductPrice = p.PRODUCT_PRICE,
                    SaleOrNot = p.SALE_OR_NOT,
                    Tag = p.TAG,
                    SubTag = p.SUB_TAG,//新增！
                    Description = p.DESCRIBTION,
                    StoreTag = p.STORE_TAG,
                    ProductPics = _dbContext.PRODUCT_IMAGES
                 .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
               .Select(img => new ImageModel { ImageId = img.IMAGE_ID })
                 .ToList(),
                    ProductDes = _dbContext.PRODUCT_DETAILS
                 .Where(img => img.PRODUCT_ID == p.PRODUCT_ID)
                 .Select(img => new ShowPicDes { Description = img.DESCRIPTION, DetailPic = new ImageModel { ImageId = img.IMAGE_ID } })
                 .ToList()
                })
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound("No products found for the given store ID and store tag.");
            }

            return Ok(products);
        }

        //Put接口传入product_id和store_id和更改属性及值
        [HttpPut("editProduct")]
        public async Task<IActionResult> EditProduct([FromBody] Product2DTO updatedProduct)
        {
            var product = await _dbContext.PRODUCTS.FirstOrDefaultAsync(p => p.PRODUCT_ID == updatedProduct.ProductId && p.ACCOUNT_ID == updatedProduct.storeId);
            if (product == null)
            {
                return NotFound("Product not found or does not belong to the store.");
            }

            if (!string.IsNullOrEmpty(updatedProduct.ProductName)) product.PRODUCT_NAME = updatedProduct.ProductName;
            //if (updatedProduct.ProductPrice.HasValue) 
                product.PRODUCT_PRICE = updatedProduct.ProductPrice;
            if (!string.IsNullOrEmpty(updatedProduct.Tag)&& !string.IsNullOrEmpty(updatedProduct.SubTagId))
            {
                //更改商家的运营方向：
                var result = await _dbContext.SUB_CATEGORYS
               .Where(c => c.SUBCATEGORY_ID == product.SUB_TAG)
               .Select(c => c.SUBCATEGORY_NAME)
               .FirstOrDefaultAsync();
                if (result == null) { return NotFound(new { Message = "子分类不存在！" }); }
                // 检查该商家的TAG是否已经存在
                var existingTag = await _dbContext.STORE_BUSINESS_DIRECTIONS
                    .FirstOrDefaultAsync(st => st.STORE_ID == updatedProduct.storeId && st.BUSINESS_TAG == product.TAG + result);
                if (existingTag == null)
                {
                    ;
                }
                else
                {
                    existingTag.LINK_COUNT--;
                    if (existingTag.LINK_COUNT <= 0)
                    {
                        _dbContext.STORE_BUSINESS_DIRECTIONS.Remove(existingTag);
                    }
                }
                product.TAG = updatedProduct.Tag;
                product.SUB_TAG = updatedProduct.SubTagId;//新增加
                //增加新的TAG
                var newResult = await _dbContext.SUB_CATEGORYS
              .Where(c => c.SUBCATEGORY_ID == product.SUB_TAG)
              .Select(c => c.SUBCATEGORY_NAME)
              .FirstOrDefaultAsync();
                if (result == null) { return NotFound(new { Message = "子分类不存在！" }); }
                // 检查该商家的TAG是否已经存在
                var newTag = await _dbContext.STORE_BUSINESS_DIRECTIONS
                    .FirstOrDefaultAsync(st => st.STORE_ID == updatedProduct.storeId && st.BUSINESS_TAG == product.TAG + result);
                if (existingTag == null)
                {
                    // 如果TAG不存在，添加新记录到STORE_TAG表
                    var newStoreTag = new STORE_BUSINESS_DIRECTION
                    {
                        STORE_ID = updatedProduct.storeId,
                        BUSINESS_TAG = product.TAG + result,
                        LINK_COUNT = 1,
                    };

                    _dbContext.STORE_BUSINESS_DIRECTIONS.Add(newStoreTag);
                }
                else
                {
                    existingTag.LINK_COUNT++;
                }
            }
              

            if (!string.IsNullOrEmpty(updatedProduct.Description)) product.DESCRIBTION = updatedProduct.Description;


            if (!string.IsNullOrEmpty(updatedProduct.StoreTag)) product.STORE_TAG = updatedProduct.StoreTag;

            _dbContext.PRODUCTS.Update(product);
            await _dbContext.SaveChangesAsync();

            return Ok("Product updated successfully.");
        }

        //delete接口，确认是未删除商品后删除该商品
        [HttpDelete("deleteProducts")]
        public async Task<IActionResult> DeleteProducts([FromBody] List<string> productIds, string storeId)
        {
            if (productIds == null || productIds.Count == 0)
            {
                return BadRequest("Product IDs are required.");
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var products = await _dbContext.PRODUCTS
                        .Where(p => productIds.Contains(p.PRODUCT_ID) && p.ACCOUNT_ID == storeId)
                        .ToListAsync();

                    if (products.Count != productIds.Count)
                    {
                        return NotFound("One or more products not found or do not belong to the store.");
                    }

                    var soldProducts = products.Where(p => p.SALE_OR_NOT == true).ToList();
                    if (soldProducts.Any())
                    {
                        return BadRequest("One or more products have been sold and cannot be deleted.");
                    }

                    foreach (var product in products)
                    {
                        // 删除 MARKET_PRODUCTS 表中对应 productId 的记录
                        var marketProducts = await _dbContext.MARKET_PRODUCTS
                            .Where(ms => ms.PRODUCT_ID == product.PRODUCT_ID)
                            .ToListAsync();
                        _dbContext.MARKET_PRODUCTS.RemoveRange(marketProducts);

                        // 删除 BUYER_PRODUCT_BOOKMARK 表中对应 productId 的记录
                        var bUYER_PRODUCT_BOOKMARK = await _dbContext.BUYER_PRODUCT_BOOKMARKS
                            .Where(ms => ms.PRODUCT_ID == product.PRODUCT_ID)
                            .ToListAsync();
                        _dbContext.BUYER_PRODUCT_BOOKMARKS.RemoveRange(bUYER_PRODUCT_BOOKMARK);

                        // 查找 ORDERS 表中对应 productId 的订单
                        var orders = await _dbContext.ORDERS
                            .Where(o => o.PRODUCT_ID == product.PRODUCT_ID)
                            .ToListAsync();

                        foreach (var order in orders)
                        {
                            // 删除 RETURN 表中对应 orderId 的记录
                            var returns = await _dbContext.RETURNS
                                .Where(r => r.ORDER_ID == order.ORDER_ID)
                                .ToListAsync();
                            _dbContext.RETURNS.RemoveRange(returns);

                            // 删除 ORDERS 表中的记录
                            _dbContext.ORDERS.Remove(order);
                        }

                        //更改商家的运营方向：
                        var result = await _dbContext.SUB_CATEGORYS
                       .Where(c => c.SUBCATEGORY_ID == product.SUB_TAG)
                       .Select(c => c.SUBCATEGORY_NAME)
                       .FirstOrDefaultAsync();
                        if (result == null) { return NotFound(new { Message = "子分类不存在！" }); }
                        // 检查该商家的TAG是否已经存在
                        var existingTag = await _dbContext.STORE_BUSINESS_DIRECTIONS
                            .FirstOrDefaultAsync(st => st.STORE_ID == storeId && st.BUSINESS_TAG == product.TAG + result);
                        if (existingTag == null)
                        {
                            ;
                        }
                        else
                        {
                            existingTag.LINK_COUNT--;
                            if (existingTag.LINK_COUNT <= 0)
                            {
                                _dbContext.STORE_BUSINESS_DIRECTIONS.Remove(existingTag);
                            }
                        }
                        // 删除 PRODUCTS 表中的记录
                        _dbContext.PRODUCTS.Remove(product);
                    }

                    // 保存更改并提交事务
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok("Products deleted successfully.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }
        }

        //post接口，新建商品
        [HttpPost("addProduct")]
        public async Task<IActionResult> AddProduct([FromForm] Product1DTO newProduct)
        {
/*            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) { return NotFound("请先登录"); }
            var userRole = User.FindFirst("UserRole")?.Value;
            if (userRole != "商家") { return Unauthorized("必须为商家才能发布商品！"); }*/
            try
            {
                // 生成唯一的 PRODUCT_ID
                string productId = "p"+YitIdHelper.NextId().ToString();//修改商品id生成方法
                var product = new PRODUCT
                {
                    PRODUCT_ID = productId,
                    PRODUCT_NAME = newProduct.ProductName,
                    PRODUCT_PRICE = newProduct.ProductPrice,
                    SALE_OR_NOT = false, // 默认值
                    TAG = newProduct.Tag,
                    SUB_TAG = newProduct.SubTag,//新增加
                    DESCRIBTION = newProduct.Description,
                    ACCOUNT_ID = newProduct.storeId,
                    STORE_TAG = newProduct.StoreTag,
                    
                };
                _dbContext.PRODUCTS.Add(product);
                //await _dbContext.SaveChangesAsync();

                // 处理图片上传
                //.Any检查集合中是否有任何元素（防止是空集）
                if (newProduct.ProductImages != null && newProduct.ProductImages.Any())
                {
                    foreach (var image in newProduct.ProductImages)
                    {
                        using (var ms = new MemoryStream())
                        {
                            string uidb = YitIdHelper.NextId().ToString();
                            await image.CopyToAsync(ms);
                            var imageData = ms.ToArray();
                            var productImage = new PRODUCT_IMAGE
                            {
                                PRODUCT_ID = productId,
                                IMAGE_ID = uidb,
                                IMAGE = imageData
                            };
                            _dbContext.PRODUCT_IMAGES.Add(productImage);
                        }
                    }
                }
                // 处理商品详情
                //.Any检查集合中是否有任何元素（防止是空集）
                if (newProduct.PicDes != null && newProduct.PicDes.Any())
                {
                    foreach (var picdes in newProduct.PicDes)
                    {
                        using (var ms = new MemoryStream())
                        {
                            string uidb = YitIdHelper.NextId().ToString();
                            await picdes.DetailPic.CopyToAsync(ms);
                            var imageData = ms.ToArray();
                            var productDetail = new PRODUCT_DETAIL
                            {
                                PRODUCT_ID = productId,
                                IMAGE_ID = uidb,
                                IMAGE = imageData,
                                DESCRIPTION = picdes.Description != null ? picdes.Description : null,
                            };
                            _dbContext.PRODUCT_DETAILS.Add(productDetail);
                        }
                    }
                }

                //记录商家的运营方向：
                var result = await _dbContext.SUB_CATEGORYS
               .Where(c => c.SUBCATEGORY_ID == newProduct.SubTag)
               .Select(c => c.SUBCATEGORY_NAME )
               .FirstOrDefaultAsync();
                if ( result == null) { return NotFound(new {Message="子分类不存在！"}); }
                // 检查该商家的TAG是否已经存在
                var existingTag = await _dbContext.STORE_BUSINESS_DIRECTIONS
                    .FirstOrDefaultAsync(st => st.STORE_ID == newProduct.storeId && st.BUSINESS_TAG == newProduct.Tag+ result);
                if (existingTag == null)
                {
                    // 如果TAG不存在，添加新记录到STORE_TAG表
                    var newStoreTag = new STORE_BUSINESS_DIRECTION
                    {
                        STORE_ID = newProduct.storeId,
                        BUSINESS_TAG = newProduct.Tag + result,
                        LINK_COUNT = 1,
                    };

                    _dbContext.STORE_BUSINESS_DIRECTIONS.Add(newStoreTag);
                }
                else
                {
                    existingTag.LINK_COUNT++;
                }

                await _dbContext.SaveChangesAsync();
                return Ok(productId);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //首页图
        [HttpGet("getProductImages/{productId}")]
        public async Task<IActionResult> GetProductImages(string productId)
        {
            try
            {
                var images = await _dbContext.PRODUCT_IMAGES
                    .Where(p => p.PRODUCT_ID == productId)
                    .Select(p => new ImageModel
                    {
                        ImageId = p.IMAGE_ID
                    })
                    .ToListAsync();

                if (images == null || !images.Any())
                {
                    return NotFound("No images found for the given product ID.");
                }

                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("deleteProductImage/{productId}/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(string productId, string imageId)
        {
            try
            {
                // 查找要删除的图片
                var image = await _dbContext.PRODUCT_IMAGES
                    .FirstOrDefaultAsync(p => p.IMAGE_ID == imageId);

                if (image == null)
                {
                    return NotFound("Image not found for the given product ID and image ID.");
                }

                // 删除图片
                _dbContext.PRODUCT_IMAGES.Remove(image);

                // 检查该 productId 是否还有其他图片
                var hasMoreImages = await _dbContext.PRODUCT_IMAGES
                    .AnyAsync(p => p.PRODUCT_ID == productId);

                if (!hasMoreImages)
                {
                    // 如果没有剩余图片，返回401状态码
                    return Unauthorized("Image deleted successfully. No more images left for this product.");
                }
                await _dbContext.SaveChangesAsync();
                return Ok("Image deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //添加商品图片
        [HttpPost("addProductImage")]
        public async Task<IActionResult> AddProductImage([FromForm] ProductImageDto model)
        {
            try
            {
                if (model.ProductImages == null || !model.ProductImages.Any())
                {
                    return BadRequest("No images uploaded.");
                }

                foreach (var image in model.ProductImages)
                {
                    using (var ms = new MemoryStream())
                    {
                        string imageId = YitIdHelper.NextId().ToString(); // 生成唯一的 IMAGE_ID
                        await image.CopyToAsync(ms); // 将图片拷贝到内存流中
                        var imageData = ms.ToArray(); // 转换为字节数组

                        // 新增到 PRODUCT_IMAGES 表
                        var productImage = new PRODUCT_IMAGE
                        {
                            PRODUCT_ID = model.ProductId,
                            IMAGE_ID = imageId,
                            IMAGE = imageData
                        };
                        _dbContext.PRODUCT_IMAGES.Add(productImage);
                    }
                }

                await _dbContext.SaveChangesAsync(); // 保存更改到数据库中

                return Ok("Images added and details updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //新接口！新建立一个商品，同时添加商品的展示图片（相当于之前添加商品的轻量版）
        [HttpPost("addNewProduct")]
        public async Task<IActionResult> NewAddProduct([FromForm] AddProductDTO newProduct)
        {
            /*            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if(userId == null) { return NotFound("请先登录"); }
                        var userRole = User.FindFirst("UserRole")?.Value;
                        if (userRole != "商家") { return Unauthorized("必须为商家才能发布商品！"); }*/
            try
            {
                // 生成唯一的 PRODUCT_ID
                string productId = "p" + YitIdHelper.NextId().ToString();//修改商品id生成方法
                var product = new PRODUCT
                {
                    PRODUCT_ID = productId,
                    PRODUCT_NAME = newProduct.ProductName,
                    PRODUCT_PRICE = newProduct.ProductPrice,
                    SALE_OR_NOT = false, // 默认值
                    TAG = newProduct.Tag,
                    SUB_TAG = newProduct.SubTag,//新增加
                    DESCRIBTION = newProduct.Description,
                    ACCOUNT_ID = newProduct.storeId,
                    STORE_TAG = newProduct.StoreTag,

                };
                _dbContext.PRODUCTS.Add(product);
                // 处理图片上传
                //.Any检查集合中是否有任何元素（防止是空集）
                if (newProduct.ProductImages != null && newProduct.ProductImages.Any())
                {
                    foreach (var image in newProduct.ProductImages)
                    {
                        using (var ms = new MemoryStream())
                        {
                            string uidb = YitIdHelper.NextId().ToString();
                            await image.CopyToAsync(ms);
                            var imageData = ms.ToArray();
                            var productImage = new PRODUCT_IMAGE
                            {
                                PRODUCT_ID = productId,
                                IMAGE_ID = uidb,
                                IMAGE = imageData
                            };
                            _dbContext.PRODUCT_IMAGES.Add(productImage);
                        }
                    }
                }
                //记录商家的运营方向：
                var result = await _dbContext.SUB_CATEGORYS
               .Where(c => c.SUBCATEGORY_ID == newProduct.SubTag)
               .Select(c => c.SUBCATEGORY_NAME)
               .FirstOrDefaultAsync();
                if (result == null) { return NotFound(new { Message = "子分类不存在！" }); }
                // 检查该商家的TAG是否已经存在
                var existingTag = await _dbContext.STORE_BUSINESS_DIRECTIONS
                    .FirstOrDefaultAsync(st => st.STORE_ID == newProduct.storeId && st.BUSINESS_TAG == newProduct.Tag + result);
                if (existingTag == null)
                {
                    // 如果TAG不存在，添加新记录到STORE_TAG表
                    var newStoreTag = new STORE_BUSINESS_DIRECTION
                    {
                        STORE_ID = newProduct.storeId,
                        BUSINESS_TAG = newProduct.Tag + result,
                        LINK_COUNT = 1,
                    };

                    _dbContext.STORE_BUSINESS_DIRECTIONS.Add(newStoreTag);
                }
                else
                {
                    existingTag.LINK_COUNT++;
                }

                await _dbContext.SaveChangesAsync();
                return Ok(new { Message = "成功添加商品！", ProductId = productId });

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //修改接口！获取某商品的全部图文详情
        [HttpGet("getProductDetails/{productId}")]
        public async Task<IActionResult> GetDetailImages(string productId)
        {
            try
            {
                var images = await _dbContext.PRODUCT_DETAILS
                    .Where(p => p.PRODUCT_ID == productId)
                    .Select(p => new ProductDetailDTO
                    {
                        Description = p.DESCRIPTION,
                        Image = new ImageModel { ImageId = p.IMAGE_ID }
                    })
                    .ToListAsync();

/*                if (images == null || !images.Any())
                {
                    return NotFound("No images found for the given product ID.");
                }*/

                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //新接口！为某商品增加一个图文详情
        [HttpPost("addProductDetail")]
        public async Task<IActionResult> NewAddProductDetail([FromForm] AddProductDetailDTO newDetail)
        {
            try
            {
                var ms = new MemoryStream();

                string uidb = YitIdHelper.NextId().ToString();
                await newDetail.Image.CopyToAsync(ms);
                var imageData = ms.ToArray();
                var productDetail = new PRODUCT_DETAIL
                {
                    PRODUCT_ID = newDetail.productId,
                    IMAGE_ID = uidb,
                    IMAGE = imageData,
                    DESCRIPTION =newDetail.Description,
                };
                _dbContext.PRODUCT_DETAILS.Add(productDetail);


                await _dbContext.SaveChangesAsync();
                return Ok(new {Message=$"成功为{newDetail.productId}添加一个图文详情！", ImageId = uidb });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //新接口！为某商品删除一个图文详情
        [HttpPost("deleteProductDetail/{imageId}")]
        public async Task<IActionResult> NewDeleteProductDetail(string imageId)
        {
            try
            {
                var result = await _dbContext.PRODUCT_DETAILS
                            .Where(c => c.IMAGE_ID == imageId)
                            .FirstOrDefaultAsync();
                if(result == null)
                {
                    return NotFound(new {Message="要删除的图文详情不存在！"});
                }
                _dbContext.PRODUCT_DETAILS.Remove(result);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = $"成功为{result.PRODUCT_ID}删除一个图文详情！" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        //修改某个图文详情中的文字描述
        [HttpPost("updateProductDescription")]
        public async Task<IActionResult> UpdateProductDescription([FromBody] UpdateDescriptionRequest request)
        {
            try
            {
                var detail = await _dbContext.PRODUCT_DETAILS
                    .FirstOrDefaultAsync(p => p.IMAGE_ID == request.ImageId);

                if (detail == null)
                {
                    return NotFound("No details found for the given image ID.");
                }

                detail.DESCRIPTION = request.Description;

                await _dbContext.SaveChangesAsync();

                return Ok("Product description updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //给一个商品添加详情图片和描述文字
        /*        [HttpPost("AddDetailedPicDes")]
                public async Task<IActionResult> AddDetailedPicDes([FromForm]ADPDModel model)
                {
                    try
                    {
                        // 假设 productId 是你要检查的 ID
                        var productIdExists = await _dbContext.PRODUCTS
                            .AnyAsync(st => st.PRODUCT_ID == model.ProductId);
                        if (productIdExists)
                        {
                            if (model.PD != null)
                            {
                                foreach (var picdes in model.PD)
                                {
                                    using (var ms = new MemoryStream())
                                    {
                                        string imageId = YitIdHelper.NextId().ToString();
                                        await picdes.DetailPic.CopyToAsync(ms);
                                        var imageData = ms.ToArray();
                                        var productDetail = new PRODUCT_DETAIL
                                        {
                                            PRODUCT_ID = model.ProductId,
                                            IMAGE_ID = imageId,
                                            IMAGE = imageData,
                                            DESCRIPTION = picdes.Description ?? null ,
                                        };
                                        _dbContext.PRODUCT_DETAILS.Add(productDetail);
                                    }
                                }
                            }
                            else
                            {
                                return Ok("图片描述列表为空！");
                            }
                        }
                        else
                        {
                            return NotFound(new { message = "不存在该商品" });
                        }
                        await _dbContext.SaveChangesAsync();
                        return Ok("添加商品详情成功！");

                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"添加商品详情图片及描述时发生错误: {ex.Message}");
                    }
                }*/

        /*    [HttpPost("addDetailImage/{productId}")]
    [Authorize]
    public async Task<IActionResult> AddDetailImage(string productId, [FromForm] List<IFormFile> images, [FromForm] List<string> descriptions)
    {
        try
        {
            // 检查是否有上传图片
            if (images == null || !images.Any())
            {
                return BadRequest("No images uploaded.");
            }

            // 检查图片数量和描述数量是否一致
            if (descriptions == null || descriptions.Count != images.Count)
            {
                return BadRequest("The number of descriptions does not match the number of images.");
            }

            for (int i = 0; i < images.Count; i++)
            {
                using (var ms = new MemoryStream())
                {
                    string imageId = YitIdHelper.NextId().ToString(); // 生成唯一的 IMAGE_ID
                    await images[i].CopyToAsync(ms); // 将图片拷贝到内存流中
                    var imageData = ms.ToArray(); // 转换为字节数组

                    // 新增到 PRODUCT_DETAIL 表
                    var productDetail = new PRODUCT_DETAIL
                    {
                        PRODUCT_ID = productId,
                        IMAGE_ID = imageId,
                        IMAGE = imageData,
                        DESCRIPTION = descriptions[i] // 与图片对应的描述
                    };
                    _dbContext.PRODUCT_DETAILS.Add(productDetail);
                }
            }

            await _dbContext.SaveChangesAsync(); // 保存更改到数据库中

            return Ok("Images and corresponding descriptions added successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }*/

        /*
        [HttpGet("getProductDescription")]
        public async Task<IActionResult> GetProductDescription(string imageId)
        {
            try
            {
                var productDetails = await _dbContext.PRODUCT_DETAILS
                    .Where(p => p.IMAGE_ID == imageId)
                    .ToListAsync();

                if (productDetails == null || !productDetails.Any())
                {
                    return NotFound("No details found for the given product ID.");
                }

                var descriptions = productDetails.Select(pd => new
                {
                    pd.DESCRIPTION
                }).ToList();

                return Ok(descriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }*/
    }
}

