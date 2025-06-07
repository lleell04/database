namespace BackendCode.DTOs.Payment
{
    public class ReturnRequestDTO
    {
        public string orderID { get; set; }
        public string ReturnReason { get; set; }
    }

    public class WalletBalanceDTO
    {
        public string AccountId { get; set; }
        public decimal Balance { get; set; }
    }

    public class OrderDTO
    {
        public string BuyerId { get; set; }
        public string ProductId { get; set; }
    }

    public class OrderRelatedDTO
    {
        public decimal credits { get; set; }
        public string address { get; set; } //默认收货地址
        public string username { get; set; } //收货人名称
        public string orderId { get; set; }
        public string createTime { get; set; }
        public bool isPaid { get; set; } //是否付款
        public ImageModel picture { get; set; }
    }

    public class OrderConfirmDTO
    {
        public string orderId { get; set; }
        public string order_address { get; set; }
        public string username { get; set; }
        public decimal actual_pay { get; set; }
        public decimal total_pay { get; set; }
    }

    public class CreditsDTO
    {
        public int BonusCredits { get; set; }
        public int Credits { get; set; }
    }

    public class RechargeDTO
    {
        public string BuyerId { get; set; }
        public decimal Amount { get; set; }
        public string returnUrl { get; set; }
    }

    public class RechargedWalletDTO
    {
        public string BuyerId { get; set; }
        public decimal Balance { get; set; }
    }

    public class OrderInfoDTO
    {
        public string CreateTime { get; set; }
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public string StoreName { get; set; }
        public decimal TotalPay { get; set; }
        public decimal ActualPay { get; set; }
        public string OrderStatus { get; set; }
        public ImageModel? Picture { get; set; }

        public string ProductId { get; set; }
        public decimal Price { get; set; }
    }

    public class AlipayDTO
    { 
        public string orderID { get; set; }
        public string actualPay { get; set; }
        public string returnUrl { get; set; }
    }
}