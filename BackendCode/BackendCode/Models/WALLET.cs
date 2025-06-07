namespace BackendCode.Models
{
    public class WALLET
    {
        public string ACCOUNT_ID { get; set; }
        public decimal BALANCE { get; set; }

        public ACCOUNT ACCOUNT { get; set; }
    }
}