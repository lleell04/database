using BackendCode.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BackendCode.DTOs.Store;
using BackendCode.Models;

namespace StoreOrderController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreOrderController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly ILogger<StoreOrderController> _logger;

        public StoreOrderController(YourDbContext dbContext, ILogger<StoreOrderController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        //get接口显示订单
        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders(string storeId, int orderStatus)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("Store ID is required.");
            }

            if (orderStatus < 0 || orderStatus > 6)
            {
                return BadRequest("Invalid status type. Allowed values are 0-6.");
            }
            try
            {
                IQueryable<ORDERS> query = _dbContext.ORDERS
                    .Where(o => o.STORE_ACCOUNT_ID == storeId);
                    switch (orderStatus)
                        {
                        case 0: // 全部
                            break;
                        case 1: // 待付款
                            query = query.Where(o => o.ORDER_STATUS == "待付款" && o.DELIVERY_NUMBER == null);
                            break;
                        case 2: // 已付款
                            query = query.Where(o => o.ORDER_STATUS == "已付款" && o.DELIVERY_NUMBER == null);
                            break;
                        case 3: // 运输中
                            query = query.Where(o => o.ORDER_STATUS == "运输中" && o.DELIVERY_NUMBER != null);
                            break;
                        case 4: // 已签收
                            query = query.Where(o => o.ORDER_STATUS == "已签收");
                            break;
                        case 5: // 待退货
                            query = query.Where(o => o.ORDER_STATUS == "待退货");
                            break;
                        case 6: // 已退货
                            query = query.Where(o => o.ORDER_STATUS == "已退货" && o.RETURN_OR_NOT == true);
                            break;
                        default:
                            return BadRequest("Invalid order status value.");
                        }
                    var orders = await query.ToListAsync();
                    var orderDTOs =orders.Select(o => new OrderDto
                    {
                        ORDER_ID = o.ORDER_ID,
                        TOTAL_PAY = o.TOTAL_PAY,
                        ACTUAL_PAY = o.ACTUAL_PAY,
                        ORDER_STATUS = o.ORDER_STATUS,
                        CREATE_TIME = o.CREATE_TIME,
                        DELIVERY_NUMBER = o.DELIVERY_NUMBER,
                        SCORE = o.SCORE,
                        REMARK = o.REMARK,
                        RETURN_OR_NOT = o.RETURN_OR_NOT,
                        
                    });

                return Ok(orderDTOs);
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex) when (ex.Number == 12704)
            {
                _logger.LogError(ex, "Charset mismatch error.");
                return StatusCode(500, "Internal server error: Charset mismatch.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for store {storeId}", storeId);
                return StatusCode(500, "Internal server error");
            }
        }

        // get接口传入store_id和order_id，返回匹配的orderDTO信息
        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(string storeId, string orderId)
        {
            if (string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Store ID and Order ID are required.");
            }

            try
            {
                var order = await _dbContext.ORDERS
                    .Where(o => o.STORE_ACCOUNT_ID == storeId && o.ORDER_ID == orderId)
                    .Select(o => new OrderDto
                    {
                        ORDER_ID = o.ORDER_ID,
                        TOTAL_PAY = o.TOTAL_PAY,
                        ACTUAL_PAY = o.ACTUAL_PAY,
                        ORDER_STATUS = o.ORDER_STATUS,
                        CREATE_TIME = o.CREATE_TIME,
                        DELIVERY_NUMBER = o.DELIVERY_NUMBER,
                        SCORE = o.SCORE,
                        REMARK = o.REMARK,
                        RETURN_OR_NOT = o.RETURN_OR_NOT,
                    })
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order for store {storeId} and order {orderId}", storeId, orderId);
                return StatusCode(500, "Internal server error");
            }
        }

        // get接口传入store_id和创建时间，返回该店该日的订单信息
        [HttpGet("GetOrdersByDate")]
        public async Task<IActionResult> GetOrdersByDate(string storeId, DateTime date)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                return BadRequest("Store ID is required.");
            }

            try
            {
                // 先检查 storeId 是否存在
                var storeExists = await _dbContext.STORES.AnyAsync(s => s.ACCOUNT_ID == storeId);
                if (!storeExists)
                {
                    return NotFound("Store ID 无匹配");
                }

                var orders = await _dbContext.ORDERS
                    .Where(o => o.STORE_ACCOUNT_ID == storeId && o.CREATE_TIME.Date == date.Date)
                    .Select(o => new OrderDto
                    {
                        ORDER_ID = o.ORDER_ID,
                        TOTAL_PAY = o.TOTAL_PAY,
                        ACTUAL_PAY = o.ACTUAL_PAY,
                        ORDER_STATUS = o.ORDER_STATUS,
                        CREATE_TIME = o.CREATE_TIME,
                        DELIVERY_NUMBER = o.DELIVERY_NUMBER,
                        SCORE = o.SCORE,
                        REMARK = o.REMARK,
                        RETURN_OR_NOT = o.RETURN_OR_NOT,
                    })
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return Ok("当日无订单");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for store {storeId} on date {date}", storeId, date);
                return StatusCode(500, "Internal server error");
            }
        }


        // put接口传入store_id和delivery_number，更改数据库中的delivery_number，且order_status改为”运输中“
        [HttpPut("UpdateDeliveryNumber")]
        public async Task<IActionResult> UpdateDeliveryNumber(string storeId, string orderId, string deliveryNumber)
        {
            if (string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(deliveryNumber))
            {
                return BadRequest("Store ID, Order ID and Delivery Number are required.");
            }

            try
            {
                // 先检查 storeId 是否存在
                var storeExists = await _dbContext.STORES.AnyAsync(s => s.ACCOUNT_ID == storeId);
                if (!storeExists)
                {
                    return NotFound("Store ID 无匹配");
                }

                var order = await _dbContext.ORDERS
                    .FirstOrDefaultAsync(o => o.STORE_ACCOUNT_ID == storeId && o.ORDER_ID == orderId);

                if (order == null)
                {
                    return NotFound("OrderID 无匹配");
                }
                if (order.ORDER_STATUS != "已付款")
                {
                    return BadRequest("已发货或未付款商品无法重新发货");
                }
                order.DELIVERY_NUMBER = deliveryNumber;
                order.ORDER_STATUS = "运输中";

                await _dbContext.SaveChangesAsync();

                return Ok("Delivery number updated and order status changed to '运输中'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delivery number for store {storeId} and order {orderId}", storeId, orderId);
                return StatusCode(500, "Internal server error");
            }
        }

        // put接口传入store_id和order_id，确认order_status为“待退货”，则更改order_status为“已退货”，且return_or_not置为true，数据库中RETURN表的return_status置为“已同意”
        [HttpPut("ConfirmReturn")]
        public async Task<IActionResult> ConfirmReturn(string storeId, string orderId)
        {
            if (string.IsNullOrEmpty(storeId) || string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Store ID and Order ID are required.");
            }

            try
            {
                // 先检查 storeId 是否存在
                var storeExists = await _dbContext.STORES.AnyAsync(s => s.ACCOUNT_ID == storeId);
                if (!storeExists)
                {
                    return NotFound("Store ID 无匹配");
                }

                var order = await _dbContext.ORDERS
                    .FirstOrDefaultAsync(o => o.STORE_ACCOUNT_ID == storeId && o.ORDER_ID == orderId);

                if (order == null)
                {
                    return NotFound("OrderID 无匹配");
                }

                if (order.ORDER_STATUS != "待退货")
                {
                    return BadRequest("Order status is not '待退货'.");
                }

                order.ORDER_STATUS = "已退货";
                order.RETURN_OR_NOT = true;

                var returnRecord = await _dbContext.RETURNS
                    .FirstOrDefaultAsync(r => r.ORDER_ID == orderId);

                if (returnRecord != null)
                {
                    returnRecord.RETURN_STATUS = "已同意";
                }

                await _dbContext.SaveChangesAsync();

                return Ok("Order status updated to '已退货' and return status updated to '已同意'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming return for store {storeId} and order {orderId}", storeId, orderId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
