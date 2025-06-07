namespace BackendCode.Models
{
    public class RETURN
    {
        public string ORDER_ID { get; set; }
        public DateTime? RETURN_TIME { get; set; }
        public string? RETURN_REASON { get; set; }
        public string? RETURN_STATUS { get; set; }

        public ORDERS ORDERS { get; set; }
    }
}
