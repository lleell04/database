using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendCode.Data;
using BackendCode.Models;
using BackendCode.DTOs.Payment;
using System.Data;
using BackendCode.DTOs;
using Alipay.AopSdk.Core.Domain;
using System.Linq;
using BackendCode.Services;
using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Request;

namespace BackendCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        public IdGenerator idGenerator;

        public PaymentController(YourDbContext context)
        {
            _dbContext = context;
            idGenerator = new IdGenerator();
        }

        /********************************/
        /* 创建退货订单接口             */
        /* 传入订单ID和退货理由         */
        /* 在数据库建立退货信息         */
        /********************************/
        [HttpPost("AddReturn")]
        public async Task<IActionResult> AddReturnAsync([FromForm] ReturnRequestDTO returnRequestDto)
        {
            /* 检查订单是否存在 */
            var order = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.ORDER_ID == returnRequestDto.orderID);
            if (order == null) //订单不存在
            {
                return NotFound("订单不存在，无法退货");
            }

            /* 检查订单状态是否允许退货 */
            if (order.ORDER_STATUS == "待退货" || order.ORDER_STATUS == "已退货")
            {
                return BadRequest("订单已经申请退货");
            }

            /* 创建退货记录对象，并设置退货信息 */
            var returnRecord = new RETURN
            {
                ORDER_ID = order.ORDER_ID,
                RETURN_TIME = DateTime.Now,
                RETURN_REASON = returnRequestDto.ReturnReason,
                RETURN_STATUS = "待同意"
            };

            order.ORDER_STATUS = "待退货"; //更新订单状态为“待退货” 等待商家处理
            order.RETURN_OR_NOT = true; //已退货

            _dbContext.RETURNS.Add(returnRecord); //将退货记录对象添加到数据库上下文

            await _dbContext.SaveChangesAsync(); //保存数据库上下文中的更改到数据库

            return Ok("成功生成退货订单，等待商家处理"); //返回退货信息
        }

        /********************************/
        /* 商家同意退货更新钱包接口     */
        /* 更新买家和商家钱包           */
        /* 返回买家和商家钱包余额       */
        /********************************/
        [HttpPut("HandleReturn")]
        public async Task<IActionResult> HandleReturnAsync(string returnID)
        {
            /* 检查退货记录是否存在 */
            var returnRecord = await _dbContext.RETURNS.FirstOrDefaultAsync(r => r.ORDER_ID == returnID);
            if (returnRecord == null) //退货记录不存在
            {
                return NotFound("退货记录不存在");
            }

            /* 找到对应订单 */
            var order = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.ORDER_ID == returnRecord.ORDER_ID);
            if (order == null) //未找到订单信息
            {
                return NotFound("相应订单不存在");
            }

            returnRecord.RETURN_STATUS = "已同意"; //更新退货状态

            /* 买家钱包 */
            var buyerWallet = await _dbContext.WALLETS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == order.BUYER_ACCOUNT_ID);
            if (buyerWallet == null)
            {
                return NotFound("买家钱包不存在");
            }

            /* 卖家钱包 */
            var sellerWallet = await _dbContext.WALLETS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == order.STORE_ACCOUNT_ID);
            if (sellerWallet == null)
            {
                return NotFound("卖家钱包不存在");
            }

            /* 分别更新钱包余额 */            
            buyerWallet.BALANCE += order.ACTUAL_PAY; //将订单实付金额退还给买家
            sellerWallet.BALANCE -= order.ACTUAL_PAY; //从卖家钱包中扣除订单实付金额

            order.ORDER_STATUS = "已退货"; //更新订单状态
            //order.RETURN_OR_NOT = true; //已退货
           
            await _dbContext.SaveChangesAsync(); //保存数据库上下文中的更改到数据库

            /* 返回买家和商家钱包余额 */ 
           
            var buyerWalletBalance = new WalletBalanceDTO //买家余额
            {
                AccountId = buyerWallet.ACCOUNT_ID,
                Balance = buyerWallet.BALANCE
            };
            
            var sellerWalletBalance = new WalletBalanceDTO //商家余额
            {
                AccountId = sellerWallet.ACCOUNT_ID,
                Balance = sellerWallet.BALANCE
            };

            var walletBalances = new List<WalletBalanceDTO> { buyerWalletBalance, sellerWalletBalance };

            return Ok(walletBalances); //返回钱包余额
        }

        /********************************/
        /* 返回订单信息接口             */
        /* 传入{buyerId,productId}      */
        /* 初步生成订单信息 存入数据库  */
        /********************************/
        [HttpPost("AddOrders")]
        public async Task<IActionResult> AddOrdersAsync([FromForm] OrderDTO orderDTO)
        {
            /* 找到对应商品 */
            var product = await _dbContext.PRODUCTS.FirstOrDefaultAsync(o => o.PRODUCT_ID == orderDTO.ProductId);
            if (product == null) //商品ID不存在
            {
                return NotFound("未找到商品");
            }
           
            /* 第一张商品图片 */
            var productImage = await _dbContext.PRODUCT_IMAGES
                .Where(pi => pi.PRODUCT_ID == orderDTO.ProductId)
                .Select(pi => new ImageModel { ImageId = pi.IMAGE_ID })
                .FirstOrDefaultAsync();

            /* 获取买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == orderDTO.BuyerId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到买家信息");
            }

            /* 获取商家信息 */
            var store = await _dbContext.STORES.FirstOrDefaultAsync(w => w.ACCOUNT_ID == product.ACCOUNT_ID);
            if (store == null) //商家ID不存在
            {
                return NotFound("未找到商家信息");
            }

            /* 商品已经售出 */
            if (product.SALE_OR_NOT) 
            {
                var order_exist = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.PRODUCT_ID == orderDTO.ProductId);
                
                if (order_exist.BUYER_ACCOUNT_ID == orderDTO.BuyerId)
                {
                    /* 返回订单相关信息 */
                    var order_Info = new OrderRelatedDTO
                    {
                        credits = buyer.TOTAL_CREDITS,
                        address = order_exist.DELIVERY_ADDRESS,
                        username = order_exist.USERNAME,
                        orderId = order_exist.ORDER_ID,
                        createTime = order_exist.CREATE_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                        //格式化createTime为易读的格式
                        isPaid = order_exist.ORDER_STATUS != "待付款",
                        picture = productImage
                    };

                    return Ok(order_Info); //返回订单相关信息
                }
                else
                {
                    return BadRequest("商品已售出");
                }
            }

            /* 商品未售出 */
            string orderId = idGenerator.GetNextId(); //生成订单号

            product.SALE_OR_NOT = true; //修改商品售出状态

            /* 获取商品的折扣信息 */
            var marketProduct = await _dbContext.MARKET_PRODUCTS.FirstOrDefaultAsync(mp => mp.PRODUCT_ID == orderDTO.ProductId);

            /* 计算订单金额(折扣价) */
            decimal totalPay = product.PRODUCT_PRICE; //原价
            if (marketProduct != null) //有折扣
            {
                totalPay = product.PRODUCT_PRICE * marketProduct.DISCOUNT_PRICE; //计算折扣价格
            }

            /* 创建订单对象，并设置订单信息 */
            var order = new ORDERS
            {
                ORDER_ID = orderId, //订单ID
                PRODUCT_ID = orderDTO.ProductId, //商品ID
                BUYER_ACCOUNT_ID = orderDTO.BuyerId, //买家ID
                STORE_ACCOUNT_ID = store.ACCOUNT_ID, //商家ID
                TOTAL_PAY = totalPay, //订单金额（折扣价）
                ORDER_STATUS = "待付款", //订单状态
                CREATE_TIME = DateTime.Now, //创建时间
                RETURN_OR_NOT = false, //是否退货
                DELIVERY_ADDRESS = buyer.ADDRESS, //默认收货地址
                USERNAME = buyer.USER_NAME //收件人昵称
            };

            _dbContext.ORDERS.Add(order); //将订单对象添加到数据库上下文

            await _dbContext.SaveChangesAsync(); //保存数据库上下文中的更改到数据库

            /* 返回订单相关信息 */
            var orderInfo = new OrderRelatedDTO
            {
                credits = buyer.TOTAL_CREDITS,
                address = order.DELIVERY_ADDRESS,
                username = order.USERNAME,
                orderId = orderId,
                createTime = order.CREATE_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                //格式化createTime为易读的格式
                isPaid = order.ORDER_STATUS != "待付款",
                picture = productImage
            };

            return Ok(orderInfo); //返回订单相关信息
        }

        /********************************/
        /* 买家确认订单信息并支付(钱包) */
        /* 更新买家卖家钱包余额         */
        /* 完善订单信息 返回买家积分    */
        /********************************/
        [HttpPut("ConfirmOrders")]
        public async Task<IActionResult> ConfirmOrdersAsync([FromForm] OrderConfirmDTO orderConfirmDTO)
        {
            /* 找到对应订单 */ 
            var order = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.ORDER_ID == orderConfirmDTO.orderId);
            //商品不存在
            while (order == null)
            {
                return NotFound("未找到订单");
            }

            /* 获取买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == order.BUYER_ACCOUNT_ID);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到买家信息");
            }

            /* 获取买家钱包信息 */
            var buyerWallet = await _dbContext.WALLETS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == order.BUYER_ACCOUNT_ID);
            if (buyerWallet == null) //买家钱包不存在
            {
                return NotFound("未找到买家钱包");
            }

            /* 获取商家钱包信息 */
            var sellerWallet = await _dbContext.WALLETS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == order.STORE_ACCOUNT_ID);
            if (sellerWallet == null) //如果商家钱包不存在
            {
                return NotFound("未找到商家钱包");
            }

            /* 检查买家钱包余额是否足够 */
            if (buyerWallet.BALANCE < orderConfirmDTO.actual_pay)
            {
                return BadRequest("买家钱包余额不足，请及时充值");
            }

            //更新买家钱包余额：从买家钱包中扣除交易金额
            buyerWallet.BALANCE -= orderConfirmDTO.actual_pay;

            //更新卖家钱包余额：将交易金额添加到卖家钱包
            sellerWallet.BALANCE += orderConfirmDTO.actual_pay;

            int a = (int)(orderConfirmDTO.total_pay - orderConfirmDTO.actual_pay);
            buyer.TOTAL_CREDITS -= 100 * a; //更新买家积分

            /* 完善订单信息 */
            order.USERNAME = orderConfirmDTO.username; //收件人昵称
            order.DELIVERY_ADDRESS = orderConfirmDTO.order_address; //收货地址
            order.ACTUAL_PAY = orderConfirmDTO.actual_pay; //实付金额
            order.TOTAL_PAY = orderConfirmDTO.total_pay; //订单金额
            order.ORDER_STATUS = "已付款"; //订单状态
            order.BONUS_CREDITS = (int)orderConfirmDTO.actual_pay; //订单积分

            buyer.TOTAL_CREDITS += order.BONUS_CREDITS; //更新买家积分

            await _dbContext.SaveChangesAsync(); //保存数据库上下文中的更改到数据库

            var credits = new CreditsDTO
            {
                BonusCredits = order.BONUS_CREDITS, //获得积分
                Credits = buyer.TOTAL_CREDITS //当前积分
            };

            return Ok(credits); //返回订单信息
        }

        /**********************************/
        /* 买家确认订单信息并支付(支付宝) */
        /* 更新买家卖家钱包余额           */
        /* 完善订单信息 返回买家积分      */
        /**********************************/
        [HttpPut("ConfirmOrdersAlipay")]
        public async Task<IActionResult> ConfirmOrdersAlipayAsync([FromForm] OrderConfirmDTO orderConfirmDTO)
        {
            /* 找到对应订单 */
            var order = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.ORDER_ID == orderConfirmDTO.orderId);
            //商品不存在
            while (order == null)
            {
                return NotFound("未找到订单");
            }

            /* 获取买家信息 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == order.BUYER_ACCOUNT_ID);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到买家信息");
            }

            /* 获取商家钱包信息 */
            var sellerWallet = await _dbContext.WALLETS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == order.STORE_ACCOUNT_ID);
            if (sellerWallet == null) //如果商家钱包不存在
            {
                return NotFound("未找到商家钱包");
            }

            //更新卖家钱包余额：将交易金额添加到卖家钱包
            sellerWallet.BALANCE += orderConfirmDTO.actual_pay;

            int a = (int)(orderConfirmDTO.total_pay - orderConfirmDTO.actual_pay);
            buyer.TOTAL_CREDITS -= 100 * a; //更新买家积分

            /* 完善订单信息 */
            order.USERNAME = orderConfirmDTO.username; //收件人昵称
            order.DELIVERY_ADDRESS = orderConfirmDTO.order_address; //收货地址
            order.ACTUAL_PAY = orderConfirmDTO.actual_pay; //实付金额
            order.TOTAL_PAY = orderConfirmDTO.total_pay; //订单金额
            order.ORDER_STATUS = "已付款"; //订单状态
            order.BONUS_CREDITS = (int)orderConfirmDTO.actual_pay; //订单积分

            buyer.TOTAL_CREDITS += order.BONUS_CREDITS; //更新买家积分

            await _dbContext.SaveChangesAsync(); //保存数据库上下文中的更改到数据库

            var credits = new CreditsDTO
            {
                BonusCredits = order.BONUS_CREDITS, //获得积分
                Credits = buyer.TOTAL_CREDITS //当前积分
            };

            return Ok(credits); //返回订单信息
        }

        /********************************/
        /* 删除订单接口                 */
        /* 传入订单号 orderId           */
        /* 从数据库中删除指定订单       */
        /* 删除退货订单同时删除退货记录 */
        /********************************/
        [HttpDelete("DeleteOrders")]
        public async Task<IActionResult> DeleteOrdersAsync(string orderId)
        {
            /* 检查订单号是否为空 */
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("订单号不能为空");
            }

            /* 在数据库中查找订单 */
            var order = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.ORDER_ID == orderId);
            if (order == null) //订单不存在
            {
                return NotFound("未找到订单");
            }

            /* 检查订单状态是否为“已退货”*/
            if (order.ORDER_STATUS == "已退货")
            {
                //查找并删除与订单相关的退货记录
                var returnRecord = await _dbContext.RETURNS.FirstOrDefaultAsync(r => r.ORDER_ID == order.ORDER_ID);
                if (returnRecord != null)
                {
                    _dbContext.RETURNS.Remove(returnRecord); //删除退货记录
                }
            }

            _dbContext.ORDERS.Remove(order); //删除订单

            await _dbContext.SaveChangesAsync(); //保存更改至数据库

            return Ok("订单已删除"); //返回成功信息
        }

        /********************************/
        /* 查看用户钱包余额接口         */
        /********************************/
        [HttpGet("GetWalletBalance")]
        public async Task<IActionResult> GetWalletBalanceAsync(string accountID)
        {
            /* 获取买家钱包信息 */
            var wallet = await _dbContext.WALLETS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == accountID);
            if (wallet == null)
            {
                return NotFound("未找到钱包");
            }

            return Ok(wallet.BALANCE); //返回钱包余额
        }

        /********************************/
        /* 获取买家所有订单接口         */
        /* 传入买家ID-buyerId           */
        /* 传出该买家的所有订单信息     */
        /********************************/
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrdersAsync(string buyerId)
        {
            /* 验证买家ID是否存在 */
            var buyer = await _dbContext.BUYERS.FirstOrDefaultAsync(b => b.ACCOUNT_ID == buyerId);
            if (buyer == null) //买家ID不存在
            {
                return NotFound("未找到买家信息");
            }

            /* 查找买家的所有订单信息 */
            var orders = await _dbContext.ORDERS.Where(o => o.BUYER_ACCOUNT_ID == buyerId).ToListAsync();
            if (orders == null || !orders.Any())
            {
                return NotFound("买家没有任何订单");
            }

            /* 创建订单信息列表 */
            var orderInfos = new List<OrderInfoDTO>();
            foreach (var order in orders) //遍历订单获取订单信息
            {
                /* 获取商品和店铺 */
                var product = await _dbContext.PRODUCTS.FirstOrDefaultAsync(o => o.PRODUCT_ID == order.PRODUCT_ID);
                var store = await _dbContext.STORES.FirstOrDefaultAsync(o => o.ACCOUNT_ID == order.STORE_ACCOUNT_ID);
                var picture = await _dbContext.PRODUCT_IMAGES.FirstOrDefaultAsync(pi =>pi.PRODUCT_ID==order.PRODUCT_ID);
                
                /* 订单详细信息 */
                var orderInfo = new OrderInfoDTO
                {
                    CreateTime = order.CREATE_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                    OrderId = order.ORDER_ID,
                    ProductName = product.PRODUCT_NAME,
                    StoreName = store.STORE_NAME,
                    Price = product.PRODUCT_PRICE, //原价
                    TotalPay = order.TOTAL_PAY,    //折扣价格
                    ActualPay = order.ACTUAL_PAY,  //实付金额
                    OrderStatus = order.ORDER_STATUS,
                    
                    ProductId = product.PRODUCT_ID
                };
                if (picture != null)
                {
                    orderInfo.Picture = new ImageModel { ImageId = picture.IMAGE_ID };//修改为ID
                }
                orderInfos.Add(orderInfo); //将OrderInfoDTO对象添加到列表中
            }

            return Ok(orderInfos); //返回订单信息列表
        }

        /********************************/
        /* 买家确认收货接口             */
        /* 传入orderId 修改订单状态     */
        /********************************/
        [HttpPut("MarkOrderReceived")]
        public async Task<IActionResult> MarkOrderReceivedAsync(string orderId)
        {
            //检查订单号是否为空
            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("订单号不能为空");
            }

            //在数据库中查找订单
            var order = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.ORDER_ID == orderId);
            if (order == null) 
            {
                return NotFound("未找到订单");
            }

            //更新订单状态为“已签收”
            order.ORDER_STATUS = "已签收";

            //保存更改至数据库
            await _dbContext.SaveChangesAsync();

            return Ok("订单状态已更新为已签收"); //返回成功信息
        }

    }
}