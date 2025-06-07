using BackendCode.Data;
using BackendCode.DTOs.CommentModel;
using BackendCode.DTOs.PostModel;
using BackendCode.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BackendCode.Services;
using BackendCode.DTOs;


namespace BackendCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly YourDbContext _context;
        //public string filePath = "./Services/post_id.txt";
        public IdGenerator postIdGenerator;

        //public string filePath2 = "./Services/image_id.txt";
        public IdGenerator imageIdGenerator;

        //public string filePath3 = "./Services/comment_id.txt";
        public IdGenerator commentIdGenerator;

        //public string filePath4 = "./Services/report_id.txt";
        public IdGenerator reportIdGenerator;
        public PostController(YourDbContext context)
        {
            _context = context;
            postIdGenerator = new IdGenerator();
            imageIdGenerator = new IdGenerator();
            commentIdGenerator = new IdGenerator();
            reportIdGenerator = new IdGenerator();
        }

        //发布帖子 （同时传文本和不定数量的图片）（注意得先登录才能成功调试此接口）
        [HttpPost("create_post")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] PostModel post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            // 从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 创建新帖子
            var newPost = new POST()
            {
                POST_ID = postIdGenerator.GetNextId(),
                RELEASE_TIME = DateTime.Now,
                POST_TITLE = post.PostTitle,
                POST_CONTENT = post.PostContent,
                NUMBER_OF_LIKES = 0,
                NUMBER_OF_COMMENTS = 0,
                ACCOUNT_ID = userId.ToString()
            };

            _context.POSTS.Add(newPost);

            // 处理图片上传
            //.Any检查集合中是否有任何元素（防止是空集）
            if (post.Images != null && post.Images.Any())
            {
                foreach (var image in post.Images)
                {
                    using (var ms = new MemoryStream())
                    {
                        await image.CopyToAsync(ms);
                        var imageData = ms.ToArray();

                        var postImage = new POST_IMAGE
                        {
                            POST_ID = newPost.POST_ID,
                            IMAGE_ID = imageIdGenerator.GetNextId(),
                            IMAGE = imageData
                        };
                        _context.POST_IMAGES.Add(postImage);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "帖子上传成功！", postId = newPost.POST_ID }); // 返回信息中含有帖子ID
        }

        //删除帖子 不需要进行身份验证（方便管理员删） 用户只能删自己的帖子，所以不会出问题
        [HttpDelete("delete_post")]
        [Authorize]
        public async Task<IActionResult> DeletePost(string postId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                return BadRequest(new { message = "帖子ID不能为空。" });
            }
            // 从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 查找帖子
            var post = await _context.POSTS.FindAsync(postId);
            if (post == null)
            {
                return NotFound(new { message = "帖子未找到。" });
            }

            // 确认用户是否有权限删除该帖子
            if (post.ACCOUNT_ID != userId)
            {
                return Forbid();
            }
            _context.POSTS.Remove(post);

            await _context.SaveChangesAsync();

            return Ok(new { message = "帖子删除成功。", operator_id = userId });
        }

        //为帖子添加评论
        [HttpPost("add_comment_to_post")]
        [Authorize]
        public async Task<IActionResult> AddCommentToPost([FromBody] CommentModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var post = await _context.POSTS.FindAsync(comment.PostId);
            if (post == null)
            {
                return NotFound(new { message = "您评论的帖子不存在！" });
            }
            post.NUMBER_OF_COMMENTS++;//原帖子增加评论数
            COMMENT_POST newcomment = new COMMENT_POST()
            {
                COMMENT_ID = commentIdGenerator.GetNextId(),
                BUYER_ACCOUNT_ID = userId,
                POST_ID = comment.PostId,
                EVALUATION_TIME = DateTime.Now,
                EVALUATION_CONTENT = comment.CommentContext,
                NUMBER_OF_SUBCOMMENTS = 0
                
            };
            _context.COMMENT_POSTS.Add(newcomment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "评论帖子成功！", comment_id = newcomment.COMMENT_ID });
        }

        //回复评论（即评论的评论）注意：仅支持二级评论，不支持给帖子评论的评论再添加评论
        [HttpPost("add_comment_to_comment")]
        [Authorize]
        public async Task<IActionResult> AddCommentToComment([FromBody] CommentModel2 comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var target_comment = await _context.COMMENT_POSTS.FindAsync(comment.CommentId);
            if (target_comment == null)
            {
                return NotFound(new { message = "您想回复的评论不存在！" });
            }
            target_comment.NUMBER_OF_SUBCOMMENTS++;//原评论增加子评论数
            var target_post = await _context.POSTS.FindAsync(target_comment.POST_ID);
            if (target_post == null)
                return NotFound(new { message = "原帖子不存在！" });
            target_post.NUMBER_OF_COMMENTS++;//原帖子增加评论数
            COMMENT_COMMENT newcomment = new COMMENT_COMMENT()
            {
                COMMENT_ID = commentIdGenerator.GetNextId(),
                ACCOUNT_ID = userId,
                COMMENTED_COMMENT_ID = comment.CommentId,
                COMMENT_TIME = DateTime.Now,
                COMMENT_CONTENT = comment.CommentContext
            };
            _context.COMMENT_COMMENTS.Add(newcomment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "评论回复成功！", comment_id = newcomment.COMMENT_ID });
        }

        //删除评论
        [HttpDelete("/delete_comment_to_post")]
        [Authorize]
        public async Task<IActionResult> DeleteCommentToPost([FromBody] CommentDeleteModel comment_delete_info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var userRole = User.FindFirst("UserRole")?.Value;

            var comment = await _context.COMMENT_POSTS
            .FirstOrDefaultAsync(c => c.COMMENT_ID == comment_delete_info.CommentId);
            if (comment == null) { return NotFound(new {message="评论不存在！"}); }
            //测试测试测试！！！！！！！！！
            //这里让帖主也可以删除自己帖子下方的评论
            var post = await _context.POSTS.FindAsync(comment.POST_ID);
            if (post == null)
            {
                return NotFound();
            }
            if (userId != comment.BUYER_ACCOUNT_ID && userRole != "管理员" && userId != post.ACCOUNT_ID)
                return BadRequest(new{message= "无权限删除此评论！"});
            _context.COMMENT_POSTS.Remove(comment);//删除评论
            post.NUMBER_OF_COMMENTS--;//原帖子减少评论数
            await _context.SaveChangesAsync();
            return Ok(new { message = "评论删除成功！", operator_id = userId });
        }

        //删除评论的评论
        [HttpDelete("/delete_comment_to_comment")]
        [Authorize]
        public async Task<IActionResult> DeleteCommentToComment([FromBody] CommentDeleteModel comment_delete_info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var userRole = User.FindFirst("UserRole")?.Value;

            var comment = await _context.COMMENT_COMMENTS
            .FirstOrDefaultAsync(c => c.COMMENT_ID == comment_delete_info.CommentId);
            if (comment == null)
                return NotFound(new { message = "要删除的评论不存在！" });

            var parent_comment = await _context.COMMENT_POSTS.FindAsync(comment.COMMENTED_COMMENT_ID);
            if (parent_comment == null)
            {
                return NotFound(new { message = "父评论不存在！" });
            }
            parent_comment.NUMBER_OF_SUBCOMMENTS--;//父评论减少子评论数

            //也可以使用 .FirstOrDefaultAsync（）方法逐级查询，但效率不如单个 LINQ 查询高
            var post = await (from cc in _context.COMMENT_COMMENTS
                              join cp in _context.COMMENT_POSTS on cc.COMMENTED_COMMENT_ID equals cp.COMMENT_ID
                              join p in _context.POSTS on cp.POST_ID equals p.POST_ID
                              where cc.COMMENT_ID == comment_delete_info.CommentId
                              select p).FirstOrDefaultAsync();
            if (post == null)
                return NotFound(new { message = "评论所在的帖子不存在！" });
            string postOwner = post.ACCOUNT_ID;
            //测试测试测试！！！！！！！！！
            //这里让帖主也可以删除自己帖子下方的评论
            if (userId != comment.ACCOUNT_ID && userRole != "管理员" && userId != postOwner)
                return BadRequest(new { message = "无权限删除此评论！" });
            _context.COMMENT_COMMENTS.Remove(comment);//删除评论

            post.NUMBER_OF_COMMENTS--;//原帖子减少评论数
            await _context.SaveChangesAsync();
            return Ok(new { message = "评论删除成功！", operator_id = userId });
        }

        //点赞帖子
        [HttpPost("like_post")]
        [Authorize]
        public async Task<IActionResult> LikePost([FromBody] LikePostModel like)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 检查是否已经点赞
            var existingLike = await _context.LIKE_POSTS
                .FirstOrDefaultAsync(e => e.BUYER_ACCOUNT_ID == userId && e.POST_ID == like.PostId);

            if (existingLike != null)
                return BadRequest(new { message = "您已经点赞过该帖子。" });

            var post = await _context.POSTS
                .FirstOrDefaultAsync(e => e.POST_ID == like.PostId);
            if (post == null)
                return NotFound(new { message = "您喜欢的帖子不存在！" });
            post.NUMBER_OF_LIKES++;//原帖子增加点赞数

            // 创建新的点赞记录
            var likePost = new LIKE_POST
            {
                BUYER_ACCOUNT_ID = userId,
                POST_ID = like.PostId
            };
            _context.LIKE_POSTS.Add(likePost);

            await _context.SaveChangesAsync();

            return Ok(new { message = "帖子点赞成功。" });
        }

        //取消点赞帖子
        [HttpPost("unlike_post")]
        [Authorize]
        public async Task<IActionResult> UnlikePost([FromBody] LikePostModel unlike)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            // 从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 检查是否存在点赞记录
            var existingLike = await _context.LIKE_POSTS
                .FirstOrDefaultAsync(e => e.BUYER_ACCOUNT_ID == userId && e.POST_ID == unlike.PostId);

            if (existingLike == null)
            {
                return BadRequest(new { message = "您尚未点赞该帖子。" });
            }

            var post = await _context.POSTS
                .FirstOrDefaultAsync(e => e.POST_ID == unlike.PostId);
            if (post == null)
            {
                return NotFound(new { message = "您取消点赞的帖子不存在！" });
            }
            post.NUMBER_OF_LIKES--;// 原帖子减少点赞数

            // 删除点赞记录
            _context.LIKE_POSTS.Remove(existingLike);

            await _context.SaveChangesAsync();

            return Ok(new { message = "取消点赞帖子成功。" });
        }

        //举报帖子
        [HttpPost("report_post")]
        [Authorize]
        public async Task<IActionResult> ReportPost([FromBody] ReportPostModel reportModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            // 从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 检查帖子是否存在
            var post = await _context.POSTS.FirstOrDefaultAsync(e => e.POST_ID == reportModel.PostId);
            if (post == null)
            {
                return NotFound(new { message = "被举报的帖子不存在！" });
            }

            //创建新的举报
            var report = new REPORT
            {
                REPORT_ID = reportIdGenerator.GetNextId(),
                REPORT_REASON = reportModel.ReportReason, // 假设 reportModel 包含 ReportReason 字段
                REPORT_TIME = DateTime.Now
            };
            _context.REPORTS.Add(report);
            //加入举报信息
            var complain = new COMPLAIN_POST
            {
                BUYER_ACCOUNT_ID = userId,
                POST_ID = reportModel.PostId,
                REPORT_ID = report.REPORT_ID
            };
            _context.COMPLAIN_POSTS.Add(complain);
            await _context.SaveChangesAsync();

            return Ok(new { message = "帖子举报成功。" });
        }

        //举报评论 注意只能举报一级评论
        [HttpPost("report_comment")]
        [Authorize]
        public async Task<IActionResult> ReportComment([FromBody] ReportCommentModel reportModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            // 从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 检查评论是否存在
            var comment = await _context.COMMENT_POSTS.FirstOrDefaultAsync(e => e.COMMENT_ID == reportModel.CommentId);
            if (comment == null)
            {
                return NotFound(new { message = "想举报的评论不存在！" });
            }

            // 创建新的举报
            var report = new REPORT
            {
                REPORT_ID = reportIdGenerator.GetNextId(),
                REPORT_REASON = reportModel.ReportReason,
                REPORT_TIME = DateTime.Now
            };
            _context.REPORTS.Add(report);

            // 加入举报信息
            var complain = new COMPLAIN_COMMENT
            {
                ACCOUNT_ID = userId,
                COMMENT_ID = reportModel.CommentId,
                REPORT_ID = report.REPORT_ID
            };
            _context.COMPLAIN_COMMENTS.Add(complain);

            await _context.SaveChangesAsync();

            return Ok(new { message = "评论举报成功。" });
        }

        //获取自己写的帖子
        [HttpGet("get_my_posts")]
        [Authorize]
        public async Task<IActionResult> GetMyPost()
        {
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            // 查询该用户写的所有帖子
            var posts = await _context.POSTS
                .Where(p => p.ACCOUNT_ID == userId)
                .Select(p => new 
                {
                    PostId = p.POST_ID, 
                    PostTitle = p.POST_TITLE,
                    NumberOfLikes=p.NUMBER_OF_LIKES,
                    NumberOfComments=p.NUMBER_OF_COMMENTS,
                    ReleaseTime=p.RELEASE_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                    CoverImageId = _context.POST_IMAGES
                .Where(pi => pi.POST_ID == p.POST_ID)
                .Select(pi => new PostImageModel { ImageId=pi.IMAGE_ID })
                .FirstOrDefault() ?? new PostImageModel { ImageId = "0000000000" } // 若没有图片则返回默认图片的ID
                })
                .ToListAsync();

            if (posts == null || !posts.Any())
            {
                return NotFound(new { message = "未找到该用户的帖子。" });
            }

            return Ok(new { message = "已找到所有帖子", target_posts = posts });
        }

        //获取某人写的所有帖子
        [HttpGet("get_someone_posts/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetPostsByUserId(string userId)
        {
            // 检查用户ID是否为空
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "用户ID不能为空。" });
            }

            // 查询该用户写的所有帖子
            var posts = await _context.POSTS
                .Where(p => p.ACCOUNT_ID == userId)
                  .Select(p => new
                  {
                      PostId = p.POST_ID,
                      PostTitle = p.POST_TITLE,
                      NumberOfLikes = p.NUMBER_OF_LIKES,
                      NumberOfComments = p.NUMBER_OF_COMMENTS,
                      ReleaseTime = p.RELEASE_TIME,
                      CoverImageId = _context.POST_IMAGES
                .Where(pi => pi.POST_ID == p.POST_ID)
                .Select(pi =>new PostImageModel {ImageId =pi.IMAGE_ID })
                .FirstOrDefault() ?? new PostImageModel { ImageId = "0000000000" }// 若没有图片则返回默认图片的ID
                  })
                .ToListAsync();

            if (posts == null || !posts.Any())
            {
                return NotFound(new { message = "未找到该用户的帖子。" });
            }

            return Ok(new { message = "已找到所有帖子", target_posts = posts });
        }

        //获取帖子列表
        [HttpPost("get_all_posts")]
        [Authorize]
        public async Task<IActionResult> GetPosts([FromBody] GetPostsModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            var postsQuery = from post in _context.POSTS
                             join buyer in _context.BUYERS
                             on post.ACCOUNT_ID equals buyer.ACCOUNT_ID
                             select new
                             {
                                 PostId=post.POST_ID,
                                 PostTitle=post.POST_TITLE,
                                 ReleaseTime=post.RELEASE_TIME,
                                 NumberOfLikes=post.NUMBER_OF_LIKES,
                                 NumberOfComments=post.NUMBER_OF_COMMENTS,
                                 AuthorId = buyer.ACCOUNT_ID,
                                 AuthorName = buyer.USER_NAME
                             };

            //将 DbSet<POST> 转换为 IQueryable<POST>。即从数据库中获取一个可查询的 IQueryable<POST> 对象
            //表示一个延迟加载的查询，即查询不会立即执行，而是等到你实际迭代查询结果或调用 ToListAsync()、CountAsync() 等方法时才执行。
            //IQueryable<POST>允许在查询执行前对查询进行进一步的组合操作（如过滤、排序、分页等）。

            switch (model.SortBy)
            {
                case "likes":
                    postsQuery = postsQuery.OrderByDescending(p => p.NumberOfLikes);
                    break;
                case "time":
                default:
                    postsQuery = postsQuery.OrderByDescending(p => p.ReleaseTime);
                    break;
            }

            var totalPosts = await postsQuery.CountAsync();// 计算总的帖子数量
            // 分页查询，获取特定页码的帖子数据
            var posts = await postsQuery
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(p => new
                {
                    p.PostId,
                    p.PostTitle,
                    ReleaseTime=p.ReleaseTime.ToString("yyyy年MM月dd日 HH:mm:ss"),//这里参量的名称和表中列名一致，想重命名则在前加XXX=p.XXX
                    p.NumberOfLikes,
                    p.NumberOfComments,
                    p.AuthorId,
                    p.AuthorName,
                    CoverImageId = _context.POST_IMAGES
                .Where(pi => pi.POST_ID == p.PostId)
                .Select(pi => new PostImageModel { ImageId = pi.IMAGE_ID })
       .FirstOrDefault() ?? new PostImageModel { ImageId = "0000000000" } // 若没有图片则返回默认图片的ID
                })
                .ToListAsync();

            //.Select选择所需的字段，将查询结果投影成一个新的对象
            //.ToListAsync()执行查询并将结果转换为一个列表
            return Ok(new { posts = posts, totalPostNums = totalPosts });
        }

        //获取某个帖子相关信息（包含其中的图片、其下的一二级评论）已经弃用
        [HttpGet("get_a_post_detail/{id}")]
        public async Task<IActionResult> GetPostDetail(string id)
        {
            // 检查ID是否为空
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "帖子ID不能为空。" });
            }
            var postQuery = from p in _context.POSTS
                            join b in _context.BUYERS on p.ACCOUNT_ID equals b.ACCOUNT_ID
                            where p.POST_ID == id
                            select new
                            {
                                PostId = p.POST_ID,
                                PostTitle = p.POST_TITLE,
                                PostContent = p.POST_CONTENT,
                                AccountId = p.ACCOUNT_ID,
                                UserName = b.USER_NAME,
                                ReleaseTime = p.RELEASE_TIME,
                                NumberOfComments = p.NUMBER_OF_COMMENTS,
                                NumberOfLikes = p.NUMBER_OF_LIKES
                            };

            var postDetail = await postQuery.FirstOrDefaultAsync();

            if (postDetail == null)
            {
                return NotFound();
            }

            var imagesQuery = from pi in _context.POST_IMAGES
                              where pi.POST_ID == id
                              select new PostImageModel
                              {
                                  ImageId = pi.IMAGE_ID,
                                  //Image = pi.IMAGE 
                              };

            var commentsQuery = from cp in _context.COMMENT_POSTS
                                join b in _context.BUYERS on cp.BUYER_ACCOUNT_ID equals b.ACCOUNT_ID
                                where cp.POST_ID == id
                                select new CommentDetailModel
                                {
                                    CommentId = cp.COMMENT_ID,
                                    AuthorId =b.ACCOUNT_ID,
                                    AuthorName=b.USER_NAME,
                                    CommentContent=cp.EVALUATION_CONTENT,
                                    CommentTime=cp.EVALUATION_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                                    SubComments = new List<SubCommentDetailModel>()
                                };

            var subCommentsQuery = from cc in _context.COMMENT_COMMENTS
                                   join b in _context.BUYERS on cc.ACCOUNT_ID equals b.ACCOUNT_ID
                                   join pc in _context.COMMENT_POSTS on cc.COMMENTED_COMMENT_ID equals pc.COMMENT_ID
                                   where pc.POST_ID == id
                                   select new SubCommentDetailModel
                                   {
                                       CommentId = cc.COMMENT_ID,
                                       CommentContent = cc.COMMENT_CONTENT,
                                       CommentTime = cc.COMMENT_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                                       AuthorId = b.ACCOUNT_ID,
                                       AuthorName = b.USER_NAME,
                                       CommentedCommentId= cc.COMMENTED_COMMENT_ID
                                   };

            var images = await imagesQuery.ToListAsync();
            var comments = await commentsQuery.ToListAsync();
            var subComments = await subCommentsQuery.ToListAsync();
            // 创建字典来保存评论和其对应的子评论
            var commentDict = comments.ToDictionary(c => c.CommentId);

            // 将子评论放入对应的评论中
            foreach (var subComment in subComments)
            {
                if (commentDict.TryGetValue(subComment.CommentedCommentId, out var comment))
                {
                    comment.SubComments.Add(subComment);
                }
            }

            var postDetailDto = new PostDetailModel
            {
                PostId = postDetail.PostId,
                PostTitle = postDetail.PostTitle,
                PostContent = postDetail.PostContent,
                AuthorId = postDetail.AccountId,
                AuthorName = postDetail.UserName,
                ReleaseTime = postDetail.ReleaseTime.ToString("yyyy年MM月dd日 HH:mm:ss"),
                NumberOfComments = postDetail.NumberOfComments,
                NumberOfLikes = postDetail.NumberOfLikes,
                Images = images,
                Comments = comments
            };
            return Ok(new {Message="查询成功！",data= postDetailDto });
        }

        //获取某个帖子相关信息（包含其中的图片、其下的一级评论）
        [HttpGet("get_a_post/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPost(string id)
        {
            // 从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // 检查是否存在点赞记录
            var existingLike = await _context.LIKE_POSTS
                .FirstOrDefaultAsync(e => e.BUYER_ACCOUNT_ID == userId && e.POST_ID == id);

            // 检查ID是否为空
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new { message = "帖子ID不能为空。" });
            }
            var postQuery = from p in _context.POSTS
                            join b in _context.BUYERS on p.ACCOUNT_ID equals b.ACCOUNT_ID
                            where p.POST_ID == id
                            select new
                            {
                                PostId = p.POST_ID,
                                PostTitle = p.POST_TITLE,
                                PostContent = p.POST_CONTENT,
                                AccountId=p.ACCOUNT_ID,
                                UserName=b.USER_NAME,
                                ReleaseTime=p.RELEASE_TIME,
                                NumberOfComments = p.NUMBER_OF_COMMENTS,
                                NumberOfLikes = p.NUMBER_OF_LIKES,
                            };

            var postDetail = await postQuery.FirstOrDefaultAsync();

            if (postDetail == null)
            {
                return NotFound();
            }

            var imagesQuery = from pi in _context.POST_IMAGES
                              where pi.POST_ID == id
                              select new PostImageModel
                              {
                                  ImageId = pi.IMAGE_ID,
                                  //Image = pi.IMAGE
                                  // ImageUrl 会自动计算，不需要显式设置
                              };

            var commentsQuery = from cp in _context.COMMENT_POSTS
                                join b in _context.BUYERS on cp.BUYER_ACCOUNT_ID equals b.ACCOUNT_ID
                                where cp.POST_ID == id
                                select new CommentDetailModel
                                {
                                    CommentId = cp.COMMENT_ID,
                                    AuthorId = b.ACCOUNT_ID,
                                    AuthorName = b.USER_NAME,
                                    AuthorPhoto= new BuyerInfoImageModel { ImageId= b.ACCOUNT_ID },//新增加
                                    CommentContent = cp.EVALUATION_CONTENT,
                                    CommentTime = cp.EVALUATION_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                                };

            var images = await imagesQuery.ToListAsync();
            var comments = await commentsQuery.ToListAsync();

            var postDetailDto = new PostDetailModel
            {
                PostId = postDetail.PostId,
                PostTitle = postDetail.PostTitle,
                PostContent = postDetail.PostContent,
                AuthorId = postDetail.AccountId,
                AuthorName = postDetail.UserName,
                AuthorPhoto = new BuyerInfoImageModel { ImageId = postDetail.AccountId },//新增加
                ReleaseTime = postDetail.ReleaseTime.ToString("yyyy年MM月dd日 HH:mm:ss"),
                NumberOfComments = postDetail.NumberOfComments,
                NumberOfLikes = postDetail.NumberOfLikes,
                Images = images,
                Comments = comments
            };

            return Ok(new { Message = "查询成功！", data = postDetailDto ,liked= existingLike != null });


        }

        //获取某评论的子评论相关信息
        [HttpGet("get_sub_comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> GetSubComments(string commentId)
        {
            // 检查ID是否为空
            if (string.IsNullOrEmpty(commentId))
            {
                return BadRequest(new { message = "评论ID不能为空。" });
            }
            // 确认输入的评论ID是否存在
            var commentExists = await _context.COMMENT_POSTS.AnyAsync(cp => cp.COMMENT_ID == commentId);

            if (!commentExists)
            {
                return NotFound(new { Message = "评论ID不存在" });
            }
            var subCommentsQuery = from cc in _context.COMMENT_COMMENTS
                                   join b in _context.BUYERS on cc.ACCOUNT_ID equals b.ACCOUNT_ID
                                   where cc.COMMENTED_COMMENT_ID == commentId
                                   select new SubCommentDetailModel
                                   {
                                       CommentId = cc.COMMENT_ID,
                                       CommentContent = cc.COMMENT_CONTENT,
                                       CommentTime = cc.COMMENT_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                                       AuthorId = b.ACCOUNT_ID,
                                       AuthorName = b.USER_NAME,
                                       CommentedCommentId = cc.COMMENTED_COMMENT_ID,
                                       AuthorPhoto = new BuyerInfoImageModel { ImageId = b.ACCOUNT_ID }//新增加
                                   };

            var subComments = await subCommentsQuery.ToListAsync();

/*            if (!subComments.Any())
            {
                return NotFound(new { Message = "子评论不存在" });
            }*/

            return Ok(new { Message = "查询成功！", Data = subComments });
        }

        // 获取标题或作者昵称含某关键词的帖子
        [HttpPost("search_posts")]
        [Authorize]
        public async Task<IActionResult> SearchPostsByName([FromBody] SearchPostsDto model)
        {
            if (string.IsNullOrWhiteSpace(model.KeyWord))
            {
                return BadRequest("关键词不能为空！");
            }

            var query = from post in _context.POSTS
                        join buyer in _context.BUYERS on post.ACCOUNT_ID equals buyer.ACCOUNT_ID
                        where post.POST_TITLE.Contains(model.KeyWord) || buyer.USER_NAME.Contains(model.KeyWord)
                        select new 
                        {
                            PostId=post.POST_ID,
                            PostTitle = post.POST_TITLE,
                            ReleaseTime =post.RELEASE_TIME,
                            PostContent =post.POST_CONTENT,
                            NumberOfLikes=post.NUMBER_OF_LIKES,
                            NumberOfComments=post.NUMBER_OF_COMMENTS,
                            AuthorId=post.ACCOUNT_ID,
                            AuthorName = buyer.USER_NAME,
                            Image = _context.POST_IMAGES
                .Where(pi => pi.POST_ID == post.POST_ID)
                .Select(pi =>new PostImageModel {ImageId= pi.IMAGE_ID })
                .FirstOrDefault() ?? new PostImageModel { ImageId = "0000000000" } // 若没有图片则返回默认图片的ID
                        };

            if (model.SortBy.ToLower() == "time")
            {
                query = model.Descending ? query.OrderByDescending(p => p.ReleaseTime) : query.OrderBy(p => p.ReleaseTime) ;
            }
            else if (model. SortBy.ToLower() == "likes")
            {
                query = model.Descending ? query.OrderByDescending(p => p.NumberOfLikes) : query.OrderBy(p => p.NumberOfLikes) ;
            }

            var posts = await query.ToListAsync();
            var count = posts.Count;
            var formattedPosts = posts.Select(p => new PostSimpleModel
            {
                PostId = p.PostId,
                PostTitle = p.PostTitle,
                ReleaseTime = p.ReleaseTime.ToString("yyyy年MM月dd日 HH:mm:ss"),  // 在这里格式化为字符串
                PostContent = p.PostContent,
                NumberOfLikes = p.NumberOfLikes,
                NumberOfComments = p.NumberOfComments,
                AuthorId = p.AuthorId,
                AuthorName = p.AuthorName,
                Image = p.Image
            }).ToList();
            /*            if (count == 0)
                        {
                            return NotFound(new { message = "没有找到匹配的帖子。" });
                        }*/
            return Ok(new { message = "搜索成功！", target_posts= formattedPosts, amount=count });
        }

        // 获取昵称或用户名或ID含关键词的用户
        [HttpGet("search_users/{keyWord}")]
        [Authorize]
        public async Task<IActionResult> SearchBuyers(string keyWord)
        {
            if (string.IsNullOrWhiteSpace(keyWord))
            {
                return BadRequest("关键词不能为空！");
            }

            var buyers = await _context.BUYERS
                .Where(b => b.ACCOUNT_ID==keyWord ||
                            b.USER_NAME.Contains(keyWord) ||
                            b.EMAIL==keyWord)
                .Select(b => new
                {
                    b.ACCOUNT_ID,
                    b.USER_NAME,
                    b.EMAIL
                })
                .ToListAsync();
            var count = buyers.Count;
/*            if (count == 0)
            {
                return NotFound(new { message = "没有找到匹配的用户！" });
            }*/

            return Ok(new {message="搜索成功！",target_users=buyers,amount=count});
        }

        //获取自己的所有帖子评论(考虑不用)
        [HttpGet("my_all_comments")]
        [Authorize]
        public async Task<IActionResult> GetAllComments()
        {
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("请先登录！");
            }
            var comments = await (from cp in _context.COMMENT_POSTS
                                  join p in _context.POSTS on cp.POST_ID equals p.POST_ID
                                  join b in _context.BUYERS on cp.BUYER_ACCOUNT_ID equals b.ACCOUNT_ID
                                  where p.ACCOUNT_ID == userId
                                  select new ReceiveCommentModel
                                  {
                                      CommentId=cp.COMMENT_ID,
                                      PostId=cp.POST_ID,
                                      PostTitle=p.POST_TITLE,
                                      AuthorId=cp.BUYER_ACCOUNT_ID,
                                      AuthorPhoto = new BuyerInfoImageModel { ImageId = cp.BUYER_ACCOUNT_ID },//新增加
                                      AuthorName =b.USER_NAME,
                                      CommentContent= cp.EVALUATION_CONTENT,
                                      CommentTime=cp.EVALUATION_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),

                                  }).ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound(new {message="未查询到任何被评论情况！"});
            }
            return Ok(new {message="查询成功！",data=comments});
        }

        //获取自己未读的帖子评论
        [HttpGet("my_new_comments")]
        [Authorize]
        public async Task<IActionResult> GetNewComments()
        {
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("请先登录！");
            }
            var comments = await (from cp in _context.COMMENT_POSTS
                                  join p in _context.POSTS on cp.POST_ID equals p.POST_ID
                                  join b in _context.BUYERS on cp.BUYER_ACCOUNT_ID equals b.ACCOUNT_ID
                                  where p.ACCOUNT_ID == userId && cp.IS_READ==0
                                  select new ReceiveCommentModel
                                  {
                                      CommentId = cp.COMMENT_ID,
                                      PostId = cp.POST_ID,
                                      PostTitle = p.POST_TITLE,
                                      AuthorId = cp.BUYER_ACCOUNT_ID,
                                      AuthorPhoto = new BuyerInfoImageModel { ImageId = cp.BUYER_ACCOUNT_ID },//新增加
                                      AuthorName = b.USER_NAME,
                                      CommentContent = cp.EVALUATION_CONTENT,
                                      CommentTime = cp.EVALUATION_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),

                                  }).ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound(new { message = "未查询到任何未读评论！" });
            }
            return Ok(new { message = "查询成功！", data = comments });
        }

        //获取自己已读的帖子评论
        [HttpGet("my_read_comments")]
        [Authorize]
        public async Task<IActionResult> GetReadComments()
        {
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("请先登录！");
            }
            var comments = await (from cp in _context.COMMENT_POSTS
                                  join p in _context.POSTS on cp.POST_ID equals p.POST_ID
                                  join b in _context.BUYERS on cp.BUYER_ACCOUNT_ID equals b.ACCOUNT_ID
                                  where p.ACCOUNT_ID == userId && cp.IS_READ == 1
                                  select new ReceiveCommentModel
                                  {
                                      CommentId = cp.COMMENT_ID,
                                      PostId = cp.POST_ID,
                                      PostTitle = p.POST_TITLE,
                                      AuthorId = cp.BUYER_ACCOUNT_ID,
                                      AuthorName = b.USER_NAME,
                                      AuthorPhoto = new BuyerInfoImageModel { ImageId = cp.BUYER_ACCOUNT_ID },//新增加
                                      CommentContent = cp.EVALUATION_CONTENT,
                                      CommentTime = cp.EVALUATION_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),

                                  }).ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound(new { message = "未查询到任何已读评论！" });
            }
            return Ok(new { message = "查询成功！", data = comments });
        }

        //获取自己未读的二级评论
        [HttpGet("my_new_subcomments")]
        [Authorize]
        public async Task<IActionResult> GetNewSubcomments()
        {
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("请先登录！");
            }
            var comments = await (from cc in _context.COMMENT_COMMENTS
                                  join p in _context.COMMENT_POSTS on cc.COMMENTED_COMMENT_ID equals p.COMMENT_ID
                                  join b in _context.BUYERS on cc.ACCOUNT_ID equals b.ACCOUNT_ID
                                  where p.BUYER_ACCOUNT_ID == userId &&  cc.IS_READ == 0
                                  select new ReceiveSubcommentModel
                                  {
                                      CommentId = cc.COMMENT_ID,
                                      CommentedCommentId = cc.COMMENTED_COMMENT_ID,
                                      AuthorId = cc.ACCOUNT_ID,
                                      AuthorName = b.USER_NAME,
                                      AuthorPhoto = new BuyerInfoImageModel { ImageId = cc.ACCOUNT_ID },//新增加
                                      CommentContent = cc.COMMENT_CONTENT,
                                      CommentTime = cc.COMMENT_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                                      PostId = p.POST_ID,//新增加
                                  }).ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound(new { message = "未查询到任何未读子评论！" });
            }
            return Ok(new { message = "查询成功！", data = comments });
        }

        //获取自己未读的二级评论
        [HttpGet("my_read_subcomments")]
        [Authorize]
        public async Task<IActionResult> GetReadSubcomments()
        {
            //从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("请先登录！");
            }
            var comments = await (from cc in _context.COMMENT_COMMENTS
                                  join p in _context.COMMENT_POSTS on cc.COMMENTED_COMMENT_ID equals p.COMMENT_ID
                                  join b in _context.BUYERS on cc.ACCOUNT_ID equals b.ACCOUNT_ID
                                  where p.BUYER_ACCOUNT_ID == userId && cc.IS_READ == 1
                                  select new ReceiveSubcommentModel
                                  {
                                      CommentId = cc.COMMENT_ID,
                                      CommentedCommentId = cc.COMMENTED_COMMENT_ID,
                                      AuthorId = cc.ACCOUNT_ID,
                                      AuthorName = b.USER_NAME,
                                      AuthorPhoto = new BuyerInfoImageModel { ImageId = cc.ACCOUNT_ID },//新增加
                                      CommentContent = cc.COMMENT_CONTENT,
                                      CommentTime = cc.COMMENT_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                                      PostId = p.POST_ID,//新增加
                                  }).ToListAsync();

            if (comments == null || !comments.Any())
            {
                return NotFound(new { message = "未查询到任何未读子评论！" });
            }
            return Ok(new { message = "查询成功！", data = comments });
        }

        //获取我点过的所有赞
        [HttpGet("my_liked_posts")]
        [Authorize]
        public async Task<IActionResult> GetMyLikes()
        {
            // 从cookie中提取用户ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("请先登录！");
            }

            var likedPosts = await (from lp in _context.LIKE_POSTS
                                    join p in _context.POSTS on lp.POST_ID equals p.POST_ID
                                    where lp.BUYER_ACCOUNT_ID == userId
                                    select new LikedPostModel
                                    {
                                        PostId = lp.POST_ID,
                                        PostTitle = p.POST_TITLE,
                                        PostReleaseTime= p.RELEASE_TIME.ToString("yyyy年MM月dd日 HH:mm:ss"),
                                        AuthorId =p.ACCOUNT_ID
                                    }).ToListAsync();

            if (likedPosts == null || !likedPosts.Any())
            {
                return NotFound(new { message = "未查询到任何点赞记录！" });
            }

            return Ok(new { message = "查询成功！", data = likedPosts });
        }

        // 标记评论为已读（可传入一、二级评论的ID）
        [HttpPost("mark_comment/{commentId}")]
        [Authorize]
        public async Task<IActionResult> MarkCommentAsRead(string commentId)
        {
            // 查找评论
            var comment = await _context.COMMENT_POSTS
                .FirstOrDefaultAsync(cp => cp.COMMENT_ID == commentId);
            if (comment != null)
            {
                comment.IS_READ = 1;
                await _context.SaveChangesAsync();
                return Ok(new { message = "（一级）评论已读成功!" });
            }
            var subcomment = await _context.COMMENT_COMMENTS
           .FirstOrDefaultAsync(cp => cp.COMMENT_ID == commentId);
            if (subcomment == null)
            {
                return NotFound(new { message = "评论未找到!" });
            }
            subcomment.IS_READ = 1;
            await _context.SaveChangesAsync();
            return Ok(new { message = "（二级）评论已读成功!" });
        }


    }
}

