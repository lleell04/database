using BackendCode.DTOs.CommentModel;
namespace BackendCode.DTOs.PostModel
{
    public class PostModel
    {
        public string PostTitle { get; set; }
        public string? PostContent { get; set; }
        public List<IFormFile> ?Images { get; set; }

    }

    public class LikePostModel
    {
        public string PostId { get; set; }
    }

    public class ReportPostModel
    {
        public string PostId { get; set; }
        public string ReportReason { get; set; }
    }

    public class GetPostsModel
    {
        public string SortBy { get; set; } = "time";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PostSimpleModel
    {
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public string? ReleaseTime { get; set; }
        public int? NumberOfLikes { get; set; }
        public int? NumberOfComments { get; set; }
        public PostImageModel Image { get; set; }
    }

    public class SearchPostsDto
    {
        public string KeyWord { get; set; }
        public string SortBy { get; set; } = "time"; // 默认排序字段
        public bool Descending { get; set; } = true; // 默认降序排序
    }
    public class PostDetailModel
    {
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public BuyerInfoImageModel AuthorPhoto { get; set; }
        public string PostTitle { get; set; }
        public string PostContent { get; set; }
        public string? ReleaseTime { get; set; }
        public int? NumberOfLikes { get; set; }
        public int? NumberOfComments { get; set; }
        public List<PostImageModel>? Images { get; set; }
        public List<CommentDetailModel> Comments { get; set; }
    }



    public class LikedPostModel
    {
        public string PostId { get; set; }
        public string PostTitle { get; set; }
        public string? PostReleaseTime { get; set; }
        public string AuthorId { get; set; }
    }

}
