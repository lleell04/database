using Microsoft.AspNetCore.Mvc;
using BackendCode.Data;
using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using BackendCode.DTOs.Payment;
using Microsoft.EntityFrameworkCore;
using BackendCode.Services;

namespace BackendCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlipayController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly AlipayService _alipayService;
        public IdGenerator idGenerator;

        public AlipayController(YourDbContext context, AlipayService alipayService)
        {
            _dbContext = context;
            _alipayService = alipayService;
            idGenerator = new IdGenerator();
        }

        /********************************/
        /* 买家使用支付宝支付订单       */
        /********************************/
        [HttpPost]
        public async Task<IActionResult> PayRequest([FromForm] AlipayDTO alipayDTO)
        {
            var order = await _dbContext.ORDERS.FirstOrDefaultAsync(o => o.ORDER_ID == alipayDTO.orderID);
            var product = await _dbContext.PRODUCTS.FirstOrDefaultAsync(o => o.PRODUCT_ID == order.PRODUCT_ID);

            //组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel
            {
                Body = product.DESCRIBTION,        //商品描述
                Subject = product.PRODUCT_NAME,    //商品名称
                TotalAmount = alipayDTO.actualPay, //付款价格
                OutTradeNo = alipayDTO.orderID,    //订单号
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();

            //设置同步回调地址
            request.SetReturnUrl(alipayDTO.returnUrl);
           
            //将业务model载入到request
            request.SetBizModel(model);

            var response = _alipayService.SdkExecute(request);

            //跳转支付宝支付
            return Ok(_alipayService.Options.Gatewayurl + "?" + response.Body);
        }

        /********************************/
        /* 买家充值 更新钱包余额接口    */
        /********************************/
        [HttpPut("RechargeWallet")]
        public async Task<IActionResult> RechargeWalletAsync([FromForm] RechargeDTO rechargeDTO)
        {
            /* 获取买家钱包信息 */
            var wallet = await _dbContext.WALLETS.FirstOrDefaultAsync(w => w.ACCOUNT_ID == rechargeDTO.BuyerId);
            if (wallet == null)
            {
                return NotFound("未找到钱包");
            }

            string orderId = idGenerator.GetNextId(); //生成订单号

            //组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel
            {
                Body = " ",   
                Subject = "钱包充值",   
                TotalAmount = rechargeDTO.Amount.ToString(), //充值金额
                OutTradeNo = orderId, //订单号
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();

            //设置同步回调地址
            request.SetReturnUrl(rechargeDTO.returnUrl);

            //将业务model载入到request
            request.SetBizModel(model);

            var response = _alipayService.SdkExecute(request);

            wallet.BALANCE += rechargeDTO.Amount; //更新钱包余额
            await _dbContext.SaveChangesAsync();  //保存更改

            //跳转支付宝支付
            return Ok(_alipayService.Options.Gatewayurl + "?" + response.Body);  
        }
    }
}