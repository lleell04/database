namespace BackendCode.Models
{
    public class STORE : ACCOUNT
    {
        public string? STORE_NAME { get; set; }
        public decimal? STORE_SCORE { get; set; }
        public bool CERTIFICATION { get; set; }
        public string? ADDRESS { get; set; }
        public virtual ICollection<STORE_BUSINESS_DIRECTION> STORE_BUSINESS_DIRECTIONS { get; set; } = new List<STORE_BUSINESS_DIRECTION>();
    }
}
