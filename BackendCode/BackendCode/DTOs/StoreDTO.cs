namespace BackendCode.DTOs.Store
{
    public class MarketDTO
    {
        public string MarketId { get; set; }
        public string? Theme { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Detail { get; set; }
        public List<MarketImageModel>? PosterImg { get; set; }
        public bool IsStoreParticipating { get; set; }
    }

    public class UpdateMarketStoreDTO
    {
        public string MarketId { get; set; }
        public string StoreId { get; set; }
        public bool InOrOut { get; set; }
    }

    public class OrderDto
    {
        public string ORDER_ID { get; set; }
        public decimal? TOTAL_PAY { get; set; }
        public decimal? ACTUAL_PAY { get; set; }
        public string ? ORDER_STATUS { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string? DELIVERY_NUMBER { get; set; }
        public decimal? SCORE { get; set; }
        public string? REMARK { get; set; }
        public bool? RETURN_OR_NOT { get; set; }
    }
    public class StoreUpdateDto
    {
        public string StoreName { get; set; }
        public string Address { get; set; }
    }
    public class SubmitAuthenticationRequest
    {
        public string storeId { get; set; }
        public IFormFile Photo { get; set; }
        public string Authentication { get; set; }
    }

}
