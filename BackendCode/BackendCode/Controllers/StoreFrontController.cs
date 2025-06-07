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
using BackendCode.DTOs;
using Alipay.AopSdk.Core.Domain;
using Yitter.IdGenerator;

namespace StoreFrontController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreFrontController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly ILogger<StoreFrontController> _logger;

        public StoreFrontController(YourDbContext dbContext, ILogger<StoreFrontController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // get接口传入store_id和时间，传出订单统计信息
        [HttpGet("GetOrderStats")]
        public async Task<IActionResult> GetOrderStats(string storeId, DateTime date)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("Store ID is required.");
            }

            try
            {
                var market = await _dbContext.MARKETS
                    .FirstOrDefaultAsync(m => m.START_TIME <= date && m.END_TIME >= date);

                if (market == null)
                {
                    return BadRequest("No active market found for the given date.");
                }

                var orderStats = await _dbContext.ORDERS
                    .Where(o => o.STORE_ACCOUNT_ID == storeId && o.CREATE_TIME.Date == date.Date)
                    .GroupBy(o => o.ORDER_STATUS)
                    .Select(g => new
                    {
                        OrderStatus = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var waitingForShipment = orderStats.FirstOrDefault(os => os.OrderStatus == "已付款")?.Count ?? 0;
                var waitingForReturn = orderStats.FirstOrDefault(os => os.OrderStatus == "待退货")?.Count ?? 0;

                var marketStoreStats = await _dbContext.MARKET_STORES
                    .Where(ms => ms.STORE_ACCOUNT_ID == storeId)
                    .GroupBy(ms => ms.IN_OR_NOT)
                    .Select(g => new
                    {
                        InOrNot = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var inCount = marketStoreStats.FirstOrDefault(ms => ms.InOrNot == true)?.Count ?? 0;
                var outCount = marketStoreStats.FirstOrDefault(ms => ms.InOrNot == false)?.Count ?? 0;

                var orderTotal = await _dbContext.ORDERS
                    .Where(o => o.STORE_ACCOUNT_ID == storeId && o.CREATE_TIME.Date == date.Date)
                    .ToListAsync();

                var orderCount = orderTotal.Count(o => o.ORDER_STATUS != "待付款" && o.ORDER_STATUS != "已退货");
                var totalRevenue = orderTotal.Sum(o => o.ACTUAL_PAY);

                return Ok(new
                {
                    WaitingForShipment = waitingForShipment,
                    WaitingForReturn = waitingForReturn,
                    InCount = inCount,
                    OutCount = outCount,
                    OrderCount = orderCount,
                    TotalRevenue = totalRevenue
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order stats for store {storeId} on date {date}", storeId, date);
                return StatusCode(500, "Internal server error");
            }
        }

        // get接口传入store_id和时间，传出该日起该店一周的订单数目
        [HttpGet("GetWeeklyOrderCount")]
        public async Task<IActionResult> GetWeeklyOrderCount(string storeId, DateTime date)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("Store ID is required.");
            }

            try
            {
                var startDate = date.Date;
                var endDate = date.AddDays(7).Date;

                var weeklyOrders = await _dbContext.ORDERS
                    .Where(o => o.STORE_ACCOUNT_ID == storeId && o.CREATE_TIME.Date >= startDate && o.CREATE_TIME.Date < endDate)
                    .GroupBy(o => o.CREATE_TIME.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count(o => o.ORDER_STATUS != "待付款"&& o.ORDER_STATUS != "已退货")
                    })
                    .ToListAsync();

                var dailyOrderCounts = new List<object>();
                for (var day = startDate; day < endDate; day = day.AddDays(1))
                {
                    var count = weeklyOrders.FirstOrDefault(w => w.Date == day)?.Count ?? 0;
                    dailyOrderCounts.Add(new { Date = day, Count = count });
                }

                return Ok(dailyOrderCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weekly order count for store {storeId} starting from date {date}", storeId, date);
                return StatusCode(500, "Internal server error");
            }
        }

        //Get接口，传出店铺名称和评分
        [HttpGet("UpdateStoreScore")]
        public async Task<IActionResult> UpdateStoreScore(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("Store ID is required.");
            }

            try
            {
                // 计算订单中非空score的平均值
                var averageScore = await _dbContext.ORDERS
                    .Where(o => o.STORE_ACCOUNT_ID == storeId) // 筛选符合条件的订单
                    .Select(o => o.SCORE) // 选择 SCORE 字段
                    .DefaultIfEmpty() // 如果没有数据，则默认值为 null
                    .AverageAsync(); // 计算平均值


                // 找到对应的store
                var store = await _dbContext.STORES
                    .FirstOrDefaultAsync(s => s.ACCOUNT_ID == storeId);

                if (store == null)
                {
                    return NotFound("Store not found.");
                }

                // 更新store的STORE_SCORE
                store.STORE_SCORE = averageScore;

                // 保存更改
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    StoreName = store.STORE_NAME,
                    StoreScore = store.STORE_SCORE
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store score for store {storeId}", storeId);
                return StatusCode(500, "Internal server error");
            }
        }

        //Get接口，传出店铺验证情况
        [HttpGet("UpdateStoreAuth")]
        public async Task<IActionResult> UpdateStoreAuth(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("Store ID is required.");
            }

            try
            {
                // 从submit_authentication表中查找记录
                var authRecord = await _dbContext.SUBMIT_AUTHENTICATIONS
                                                 .Where(sa => sa.STORE_ACCOUNT_ID == storeId)
                                                 .FirstOrDefaultAsync();

                if (authRecord == null)
                {
                    return Ok(0); // 搜索不到记录
                }

                // 根据status返回相应的int值
                switch (authRecord.STATUS)
                {
                    case "待审核":
                        return Ok(1);
                    case "已拒绝":
                        return Ok(2);
                    case "已通过":
                        return Ok(3);
                    default:
                        return Ok(0); // 默认情况下也返回0
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store authentication for store {storeId}", storeId);
                return StatusCode(500, "Internal server error");
            }
        }

        //Get接口，传出店铺申请认证的图片文字
        [HttpGet("GetStoreAuthImg")]
        public async Task<IActionResult> GetStoreAuthImg(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("Store ID is required.");
            }

            try
            {
                var store = await _dbContext.STORES.FindAsync(storeId); // 从数据库中查找store记录
                var auth = await _dbContext.SUBMIT_AUTHENTICATIONS
                            .Where(a => a.STORE_ACCOUNT_ID == storeId)
                            .FirstOrDefaultAsync();
                if (store == null)
                {
                    return NotFound("Store not found.");
                }
                if (auth == null)
                {
                    return NotFound("Submission not found.");
                }
                var status=auth. STATUS;
                if (status != "已通过")
                {
                    return BadRequest("Submission not approved");
                }
                return Ok(new
                {
                    Authentication = auth.AUTHENTICATION,
                    //photo = auth.PHOTO
                    Photo = new AuthImageModel
                    {
                        ImageId = auth.STORE_ACCOUNT_ID
                    },
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store authentication for store {storeId}", storeId);
                return StatusCode(500, "Internal server error");
            }
        }
        //Post提交审核接口
        [HttpPost("SubmitAuthentication")]
        public async Task<IActionResult> SubmitAuthentication([FromForm] SubmitAuthenticationRequest request)
        {
            if (string.IsNullOrEmpty(request.storeId))
            {
                return BadRequest("Store ID is required.");
            }

            if (string.IsNullOrEmpty(request.Authentication))
            {
                return BadRequest("Authentication string is required.");
            }

            try
            {
                // 随机选择一个管理员ID
                var randomAdmin = await _dbContext.ADMINISTRATORS
                    .OrderBy(r => Guid.NewGuid())
                    .FirstOrDefaultAsync();

                if (randomAdmin == null)
                {
                    return NotFound("No administrators found.");
                }

                // 查找是否已存在记录
                var existingRecord = await _dbContext.SUBMIT_AUTHENTICATIONS
                    .FirstOrDefaultAsync(sa => sa.STORE_ACCOUNT_ID == request.storeId);

                if (existingRecord != null)
                {
                    if (existingRecord.STATUS == "待审核" || existingRecord.STATUS == "已通过")
                    {
                        return Ok("已通过或审核中无需重复上传");
                    }
                    else if (existingRecord.STATUS == "已拒绝")
                    {
                        // 删除旧记录
                        _dbContext.SUBMIT_AUTHENTICATIONS.Remove(existingRecord);
                        await _dbContext.SaveChangesAsync();
                    }
                }

                var ms = new MemoryStream();
                string uidb = YitIdHelper.NextId().ToString();
                await request.Photo.CopyToAsync(ms);
                var imageData = ms.ToArray();

                // 创建新的提交验证记录
                var submitAuthentication = new SUBMIT_AUTHENTICATION
                {
                    STORE_ACCOUNT_ID = request.storeId,
                    ADMINISTRATOR_ACCOUNT_ID = randomAdmin.ACCOUNT_ID,
                    AUTHENTICATION = request.Authentication,
                    STATUS = "待审核",
                    PHOTO = imageData
                };

                // 添加到数据库并保存
                _dbContext.SUBMIT_AUTHENTICATIONS.Add(submitAuthentication);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Authentication submitted successfully.",
                    StoreId = submitAuthentication.STORE_ACCOUNT_ID,
                    AdministratorId = submitAuthentication.ADMINISTRATOR_ACCOUNT_ID,
                    Status = submitAuthentication.STATUS
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting authentication for store {storeId}", request.storeId);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}

