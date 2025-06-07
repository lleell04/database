namespace BackendCode.DTOs
{
    public class CategoryDTO
    {
        public string SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
    }

    public class CategoryDetailDTO
    {
        public string CategoryDescription { get; set; }
        public CategoryImageModel CategoryPhoto { get; set; }
    }
    public class UploadCategoryPic
    {
        public string CategoryName { get; set; }
        public IFormFile CategoryPhoto { get; set; }
    }
}
