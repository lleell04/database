using BackendCode.DTOs.UserInfo;

namespace BackendCode.DTOs.CommentModel
{
    public class CommentModel
    {
        public string PostId { get; set; }
        public string CommentContext { get; set; }
    }

    public class CommentModel2
    {
        public string CommentId { get; set; }
        public string CommentContext { get; set; }
    }
    public class CommentDeleteModel
    {
        public string CommentId { get; set; }
    }

    public class ReportCommentModel {
        public string CommentId { get; set; }
        public string ReportReason { get; set; }
    }

    public class CommentDetailModel
    {
        public string CommentId { get; set; }
        public string AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public BuyerInfoImageModel AuthorPhoto { get; set; }
        public string CommentTime { get; set; }
        public string? CommentContent { get; set; }
        public List<SubCommentDetailModel> SubComments { get; set; }  //子评论列表
    }

    public class SubCommentDetailModel
    {
        public string CommentId { get; set; }
        public string AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public BuyerInfoImageModel AuthorPhoto { get; set; }
        public string CommentedCommentId { get; set; }
        public string CommentTime { get; set; }
        public string? CommentContent { get; set; }
    }

    public class ReceiveCommentModel
    {
        public string CommentId { get; set; }
        public string AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public BuyerInfoImageModel AuthorPhoto { get; set; }

        public string CommentTime { get; set; }
        public string PostId { get; set; }
        public string CommentContent { get; set; }
        public string PostTitle { get; set; }
    }
    public class ReceiveSubcommentModel
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public BuyerInfoImageModel AuthorPhoto { get; set; }
        public string CommentContent { get; set; }
        public string CommentTime { get; set; }
        public string CommentedCommentId { get; set; }

    }

}

