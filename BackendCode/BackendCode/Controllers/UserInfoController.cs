using BackendCode.Data;
using Microsoft.AspNetCore.Mvc;
using BackendCode.DTOs.UserInfo;
using Microsoft.EntityFrameworkCore;
using BackendCode.DTOs;

namespace UserInfo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly YourDbContext _dbContext;
        private readonly ILogger<UserInfoController> _logger;
       
        public UserInfoController(YourDbContext dbContext, ILogger<UserInfoController> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;

        }

        // 根据用户id返回个人详细信息
        [HttpGet("detailedInfo")]
        public async Task<IActionResult> ShowUserInfo(string uid)
        {
            var infos = await _dbContext.BUYERS.FirstOrDefaultAsync(a => a.ACCOUNT_ID == uid);

            if (infos == null)
            {
                return NotFound("User not found.");
            }

            var response = new UserInfoDTO
            {
                Id = infos.ACCOUNT_ID,
                UserName = infos.USER_NAME,
                EMAIL = infos.EMAIL,
                Gender = infos.GENDER,
                Age = infos.AGE,
                TotalCredits = infos.TOTAL_CREDITS,
                Address = infos.ADDRESS
            };
            return Ok(response);
        }


        // 根据用户id返回头像和简介
        [HttpPost("GetPhotoAndDescribtion")]
        public async Task<IActionResult> GetPhotoAndDescribtion([FromBody]GPADModel model)
        {
            string type = model.Id.Substring(0, 1);
            if (type == "U")
            {
                var infos = await _dbContext.BUYERS
                .Where(b => b.ACCOUNT_ID == model.Id)
                .SingleOrDefaultAsync();
                if (infos == null)
                {
                    return NotFound("不存在该用户");
                }

                var response = new PhotoAndDescribtionDTO1
                {
                    Photo = new BuyerInfoImageModel { ImageId = model.Id },
                    Describtion = infos.DESCRIBTION,
                };
                return Ok(response);
            }
            else if(type == "S")
            {
                var infos = await _dbContext.STORES
                .Where(b => b.ACCOUNT_ID == model.Id)
                .SingleOrDefaultAsync();
                if (infos == null)
                {
                    return NotFound("不存在该用户");
                }

                var response = new PhotoAndDescribtionDTO2
                {
                    Photo = new StoreInfoImageModel { ImageId = model.Id },
                    Describtion = infos.DESCRIBTION,
                };
                return Ok(response);
            }
            else
            {
                return BadRequest("错误的账号类型：请检查Id是否为买家或商家");
            }
            

            
        }

        // 修改头像或简介
        [HttpPut("SetPhotoAndDescribtion")]
        public async Task<IActionResult> SetPhotoAndDescribtion([FromForm] SPADModel model)
        {
            string type = model.Id.Substring(0, 1);
            string res = "";
            if (type == "U")
            {
                var temp = await _dbContext.BUYERS
                .Where(b => b.ACCOUNT_ID == model.Id)
                .SingleOrDefaultAsync();

                if (temp == null)
                {
                    return NotFound(new { Message = "不存在该用户" });
                }

                if (model.Describtion != null)
                {
                    temp.DESCRIBTION = model.Describtion;
                    res += "简介已被成功更改 ";
                }

                if (model.Photo != null)
                {
                    var ms = new MemoryStream();
                    await model.Photo.CopyToAsync(ms);
                    var imageData = ms.ToArray();
                    temp.PHOTO = imageData;
                    res += "头像已被成功更改 ";
                }
            }
            else if(type == "S")
            {
                var temp = await _dbContext.STORES
                .Where(b => b.ACCOUNT_ID == model.Id)
                .SingleOrDefaultAsync();

                if (temp == null)
                {
                    return NotFound(new { Message= "不存在该商家" });
                }

                if (model.Describtion != null)
                {
                    temp.DESCRIBTION = model.Describtion;
                    res += "简介已被成功更改 ";
                }

                if (model.Photo != null)
                {
                    var ms = new MemoryStream();
                    await model.Photo.CopyToAsync(ms);
                    var imageData = ms.ToArray();
                    temp.PHOTO = imageData;
                    res += "头像已被成功更改 ";
                }
            }
            else
            {
                return BadRequest("错误的账号类型：请检查Id是否为买家或商家");
            }
            
            // 保存更改
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok($"申请已被处理，处理结果:{res}");
            }
            catch (DbUpdateException ex)
            {
                // 捕获数据库更新异常并返回错误消息
                return StatusCode(500, $"更新数据库时发生错误： {ex.Message}");
            }
        }

    }
}


