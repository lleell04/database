namespace BackendCode.DTOs.UserInfo
{
    public class UserInfoDTO
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? EMAIL { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public int? TotalCredits { get; set; }
        public string? Address { get; set; }
    }

    public class BuyerMessageModel
    {
        public string AccountId { get; set; }
        public string? UserName { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set;}
        public string? Address { get; set; }
    }

    public class SellerMessageModel
    {
        public string AccountId { get; set; }
        public string? UserName { get; set; }
        public string? StoreName { get; set; }
        public string? Address { get; set; }
    }

    public class AdministratorMessageModel
    {
        public string AccountId { get; set; }
        public string? UserName { get; set; }
    }

    public class GPADModel
    {
        public string? Id { get; set; }
    }

    public class SPADModel
    {
        public string? Id { get; set; }
        public IFormFile? Photo { get; set; }
        public string? Describtion { get; set; }
    }

    public class PhotoAndDescribtionDTO1
    {
        public BuyerInfoImageModel? Photo { get; set; }
        public string? Describtion { get; set; }
    }
    public class PhotoAndDescribtionDTO2
    {
        public StoreInfoImageModel? Photo { get; set; }
        public string? Describtion { get; set; }
    }
}
