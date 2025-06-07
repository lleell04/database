using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendCode.Data;
using BackendCode.DTOs.Administrator;
using Yitter.IdGenerator;
using BackendCode.DTOs;
using BackendCode.Models;
using Alipay.AopSdk.Core.Domain;

namespace Administrator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        

        public AdministratorController(YourDbContext context)
        {
            _dbContext = context;
        }


        [HttpPost("GetAllAuthentication")]
        public async Task<IActionResult> GetAllAuthenticationAsync([FromBody]GAAModel model)
        {

            // 获取所有申请信息
            var authentications = await _dbContext.SUBMIT_AUTHENTICATIONS
                .Where(p => p.ADMINISTRATOR_ACCOUNT_ID == model.adminId)
                .ToListAsync();

            if (authentications == null || !authentications.Any())
            {
                return NotFound("No authentication was found");
            }

            var allAuthenticationDtos = authentications.Select(p => new ShowAuthenticationDTO
            {
                StoreId = p.STORE_ACCOUNT_ID,
                Authentication = p.AUTHENTICATION,
                Status = p.STATUS,
                //Photo = p.PHOTO != null ? Convert.ToBase64String(p.PHOTO) : null,
                Photo = new AuthImageModel
                {
                    ImageId = p.STORE_ACCOUNT_ID
                },

            }).ToList();
            // 返回商品信息
            return Ok(allAuthenticationDtos);
        }

        //修改
        [HttpPut("UpdateStoreAuthentication")]
        public async Task<IActionResult> UpdateStoreAuthentication([FromBody] USAModel model)
        {
            // 查找提交的认证
            var temp = await _dbContext.SUBMIT_AUTHENTICATIONS
                .FirstOrDefaultAsync(a => a.STORE_ACCOUNT_ID == model.storeId);

            // 查找商店
            var tempstore = await _dbContext.STORES
                .FirstOrDefaultAsync(b => b.ACCOUNT_ID == model.storeId);

            if (temp == null)
            {
                return NotFound("No authentication found for the given store.");
            }

            // 确保商店存在
            if (tempstore == null)
            {
                return NotFound("Store not found.");
            }

            if (model.result)
            {
                temp.STATUS = "已通过";
                temp.ADMINISTRATOR_ACCOUNT_ID = model.adminId;
                tempstore.CERTIFICATION = true;
            }
            else
            {
                temp.STATUS = "已拒绝";
                temp.ADMINISTRATOR_ACCOUNT_ID = model.adminId;
                tempstore.CERTIFICATION = false;
            }

            // 保存更改
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok($"申请已被处理，处理结果:{temp.STATUS}，处理人:{model.adminId}");
            }
            catch (DbUpdateException ex)
            {
                // 捕获数据库更新异常并返回错误消息
                return StatusCode(500, $"An error occurred while updating the database: {ex.Message}");
            }
            
        }

        //向拥有指定类型商品的商家发送邀请
        [HttpPost("InviteStores")]
        public async Task<IActionResult> InviteStores([FromBody] ISModel model)
        {
            //根据传入的小类Id找到小类名称
            var result = await _dbContext.SUB_CATEGORYS
                .Where(c => c.SUBCATEGORY_ID == model.InviteTag)
                .FirstOrDefaultAsync();
            if (result == null)
            {
                return Ok("传入错误的小分类Id");
            }

            string searchString = result.CATEGORY_NAME + result.SUBCATEGORY_NAME;
            //新版找商家：
            var storeIds = await _dbContext.STORE_BUSINESS_DIRECTIONS
                         .Where(st => st.BUSINESS_TAG.Contains(searchString))
                         .Select(st => st.STORE_ID)
                         .Distinct()
                         .ToListAsync();

            foreach (var storeId in storeIds)
            {
                var store = await _dbContext.MARKET_STORES
                     .Where(st => st.STORE_ACCOUNT_ID==storeId&&st.MARKET_ID==model.MarketId)
                     .FirstOrDefaultAsync();
                if (store != null) { continue; }

                _dbContext.MARKET_STORES.Add(new MARKET_STORE()
                {
                    MARKET_ID = model.MarketId,
                    STORE_ACCOUNT_ID = storeId,
                    IN_OR_NOT = false,
                });
            }

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok($"邀请商家成功，邀请Tag为{searchString}");
            }
            catch (DbUpdateException ex)
            {
                // 捕获数据库更新异常并返回错误消息
                return StatusCode(500, $"邀请商家时出现错误: {ex.Message}");
            }
        }


        //添加市集并发出邀请
        [HttpPut("AddMarket")]
        public async Task<IActionResult> AddMarket([FromForm] AMModel model)
        {
            string uidb = YitIdHelper.NextId().ToString();
            uidb = "M" + uidb;
            string pic_id = YitIdHelper.NextId().ToString();

            var ms = new MemoryStream();
            await model.posterImg.CopyToAsync(ms);
            var imageData = ms.ToArray();

            _dbContext.MARKETS.Add(new BackendCode.Models.MARKET()
            {
                MARKET_ID=uidb,
                THEME=model.theme,
                START_TIME=model.startTime,
                END_TIME=model.endTime,
                DETAIL=model.detail,
                POSTERIMG=imageData,
                IMAGE_ID= pic_id
            });
            _dbContext.SaveChanges();

            _dbContext.SET_UP_MARKETPLACES.Add(new BackendCode.Models.SET_UP_MARKETPLACE()
            {
                MARKET_ID = uidb,
                ADMINISTRATOR_ACCOUNT_ID=model.adminId,
            });
            _dbContext.SaveChanges();


            /*            //查找有哪些符合条件的商家
                        var storeIds = _dbContext.PRODUCTS
                                           .Where(p => p.TAG == model.option)
                                           .Select(p => p.ACCOUNT_ID)
                                           .Distinct()  // 如果需要唯一的商店 ID，可以加上 Distinct()
                                           .ToList();

                        foreach (var storeId in storeIds)
                        {
                            _dbContext.MARKET_STORES.Add(new BackendCode.Models.MARKET_STORE()
                            {
                                MARKET_ID = uidb,
                                STORE_ACCOUNT_ID = storeId,
                                IN_OR_NOT=false,
                            }) ;
                        }*/
            //考虑邀请接口单独拆开？考虑传入字符串直接匹配商家经营方向？
            //现在接口传入的是小类ID
            /*var result = await _dbContext.SUB_CATEGORYS
                .Where(c => c.SUBCATEGORY_ID == model.option)
                .FirstOrDefaultAsync();
            string searchString = result.CATEGORY_NAME+ result.SUBCATEGORY_NAME;
            //新版找商家：
            var storeIds = await _dbContext.STORE_BUSINESS_DIRECTIONS
                         .Where(st => st.BUSINESS_TAG.Contains(searchString))
                         .Select(st => st.STORE_ID)
                         .Distinct()
                         .ToListAsync();
            foreach (var storeId in storeIds)
            {
                _dbContext.MARKET_STORES.Add(new BackendCode.Models.MARKET_STORE()
                {
                    MARKET_ID = uidb,
                    STORE_ACCOUNT_ID = storeId,
                    IN_OR_NOT = false,
                });
            }*/


            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok($"市集添加成功，市集id:{uidb}");
            }
            catch (DbUpdateException ex)
            {
                // 捕获数据库更新异常并返回错误消息
                return StatusCode(500, $"An error occurred while updating the database: {ex.Message}");
            }

        }


        [HttpGet("GetAllMarket")]
        public async Task<IActionResult> GetAllMarket()
        {

            // 获取所有申请信息
            var markets = _dbContext.MARKETS
                        .OrderBy(m => m.START_TIME)
                        .ToList();

            if (markets == null || !markets.Any())
            {
                return NotFound("现在还没有市集");
            }


            var allmarketDtos = markets.Select(p => new ShowMarketDTO
            {
                marketId=p.MARKET_ID,
                theme=p.THEME,
                startTime=p.START_TIME,
                endTime=p.END_TIME,
                detail=p.DETAIL,
                //posterImg= Convert.ToBase64String(p.POSTERIMG),
                image=new MarketImageModel { ImageId=p.IMAGE_ID,}

            }).ToList();
            // 返回商品信息
            return Ok(allmarketDtos);
        }


        [HttpPut("DeleteMarket")]
        public async Task<IActionResult> DeleteMarket([FromBody]DMModel model)
        {
            DateTime currentDateTime = DateTime.Now;
            var startTime = _dbContext.MARKETS
                                    .Where(m => m.MARKET_ID == model.marketId)
                                    .Select(m => m.START_TIME)
                                    .FirstOrDefault();
            if(startTime == null)
            {
                return NotFound("不存在该市集");
            }
            if (currentDateTime >= startTime)
            {
                return Ok("删除失败！无法删除已经开始的市集");
            }
            try
            {
                // 在SET_UP_MARKETPLACES中删除找到的行
                var itemsToDelete = await _dbContext.SET_UP_MARKETPLACES
                    .Where(item => item.MARKET_ID == model.marketId)
                    .ToListAsync();

                _dbContext.SET_UP_MARKETPLACES.RemoveRange(itemsToDelete);
                await _dbContext.SaveChangesAsync();

                // 在MARKET_STORES中删除找到的行
                var itemsToDelete2 = await _dbContext.MARKET_STORES
                    .Where(item => item.MARKET_ID == model.marketId)
                    .ToListAsync();

                _dbContext.MARKET_STORES.RemoveRange(itemsToDelete2);
                await _dbContext.SaveChangesAsync();

                // 在MARKET_PRODUCTS中删除找到的行
                var itemsToDelete3 = await _dbContext.MARKET_PRODUCTS
                    .Where(item => item.MARKET_ID == model.marketId)
                    .ToListAsync();

                _dbContext.MARKET_PRODUCTS.RemoveRange(itemsToDelete3);
                await _dbContext.SaveChangesAsync();

                // 在MARKETS中删除找到的行
                var itemsToDelete4 = await _dbContext.MARKETS
                    .Where(item => item.MARKET_ID == model.marketId)
                    .ToListAsync();

                _dbContext.MARKETS.RemoveRange(itemsToDelete4);
                await _dbContext.SaveChangesAsync();


                return Ok($"删除市集成功！删除市集编号： {model.marketId}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"删除市集失败: {ex.Message}");
            }
        }


        [HttpGet("GetPostReport")]
        public async Task<IActionResult> GetPostReport()
        {
            try
            {
                var query = from report in _dbContext.REPORTS
                            join complainPost in _dbContext.COMPLAIN_POSTS on report.REPORT_ID equals complainPost.REPORT_ID
                            join post in _dbContext.POSTS on complainPost.POST_ID equals post.POST_ID
                            join postImage in _dbContext.POST_IMAGES on post.POST_ID equals postImage.POST_ID into postImagesGroup
                            select new ShowReportDTO
                            {
                                reportId = report.REPORT_ID,
                                buyerAccountId = complainPost.BUYER_ACCOUNT_ID,
                                reportingTime = report.REPORT_TIME,
                                postTime = post.RELEASE_TIME,
                                reportingReason = report.REPORT_REASON,
                                postContent = post.POST_CONTENT,
                                postTitle = post.POST_TITLE,
                                auditResults = report.AUDIT_RESULTS,
                                //postImages = postImagesGroup.Select(pi => Convert.ToBase64String(pi.IMAGE)) .ToList(),
                                postImages = postImagesGroup.Select(img => new PostImageModel
                                {
                                    ImageId = img.IMAGE_ID
                                })
                                 .ToList(),
                            };
                var res = await query.ToListAsync();
                return Ok(res);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"访问失败: {ex.Message}");
            }
            
        }


        [HttpGet("GetCommentReport")]
        public async Task<IActionResult> GetCommentReport()
        {
            try
            {
                var query = from report in _dbContext.REPORTS
                            join complainC in _dbContext.COMPLAIN_COMMENTS on report.REPORT_ID equals complainC.REPORT_ID
                            join post in _dbContext.COMMENT_POSTS on complainC.COMMENT_ID equals post.COMMENT_ID
                            select new ShowCommentRepoDTO
                            {
                                ReportId = report.REPORT_ID,
                                BuyerAccountId = complainC.ACCOUNT_ID,
                                ReportingTime = report.REPORT_TIME,
                                PostTime = post.EVALUATION_TIME ,
                                ReportingReason = report.REPORT_REASON,
                                PostContent = post.EVALUATION_CONTENT,
                                CommentId = complainC.COMMENT_ID,
                                AuditResults = report.AUDIT_RESULTS,

                            };

                var res = await query.ToListAsync();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"访问失败: {ex.Message}");
            }

        }


        //修改
        [HttpPut("AuditReport")]
        public async Task<IActionResult> AuditReport([FromBody] ARModel model)
        {
            DateTime currentDateTime = DateTime.Now;
            // 查找reportid
            var reporttoc = await _dbContext.REPORTS
                .FirstOrDefaultAsync(a => a.REPORT_ID == model.reportId);

            if (reporttoc == null)
            {
                return NotFound("不存在该条投诉记录");
            }

            if (model.auditResult=="删除")
            {
                reporttoc.AUDIT_RESULTS = model.auditResult;
                reporttoc.AUDIT_TIME = currentDateTime;
                reporttoc.ADMINISTRATOR_ACCOUNT_ID = model.adminId;

                var complain_post = await _dbContext.COMPLAIN_POSTS
              .FirstOrDefaultAsync(a => a.REPORT_ID == model.reportId);
                if(complain_post == null)
                {
                    var complain_comment = await _dbContext.COMPLAIN_COMMENTS
             .FirstOrDefaultAsync(a => a.REPORT_ID == model.reportId);
                    if(complain_comment == null){ return NotFound(new { Message = "要删除的内容不存在！" }); }
                    else
                    {
                        var comment = await _dbContext.COMMENT_POSTS
             .FirstOrDefaultAsync(a => a.COMMENT_ID == complain_comment.COMMENT_ID);
                        _dbContext.COMMENT_POSTS.Remove(comment);
                    }
                }
                else
                {
                    var post = await _dbContext.POSTS
                    .FirstOrDefaultAsync(a => a.POST_ID == complain_post.POST_ID);
                    _dbContext.POSTS.Remove(post);
                }
            }
            else
            {
                reporttoc.AUDIT_RESULTS = model.auditResult;
                reporttoc.AUDIT_TIME = currentDateTime;
                reporttoc.ADMINISTRATOR_ACCOUNT_ID = model.adminId;
            }

            // 保存更改
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok($"举报已被处理，处理结果:已{model.auditResult}，处理人:{model.adminId}");
            }
            catch (DbUpdateException ex)
            {
                // 捕获数据库更新异常并返回错误消息
                return StatusCode(500, $"An error occurred while updating the database: {ex.Message}");
            }

        }
    }

}

