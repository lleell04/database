using BackendCode.Data;
using Microsoft.AspNetCore.Mvc;
using BackendCode.DTOs.LoginModel;
using BackendCode.DTOs.UserInfo;
using Microsoft.Extensions.Logging;
//以下是为了cookie新加的
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
//邮件
using System.Net.Mail;
using System.Net;
//以下是为了使用ID生成器和哈希等
using BackendCode.Services;
using Microsoft.EntityFrameworkCore;
using BackendCode.DTOs.Administrator;
using BackendCode.Controllers;
using BackendCode.Models;
using Alipay.AopSdk.Core.Domain;

namespace Account.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly YourDbContext _context;
        //public string filePath = "./Services/account_id.txt";
        public IdGenerator idGenerator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(YourDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            idGenerator = new IdGenerator();
            _logger = logger;//方便调试///////////////////////
        }

        /*登录：
         * 支持用户使用ID或者邮箱作为用户名进行登录
         * 将用户的ID 邮箱 身份装入cookie*/
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //查看是否为买家
            var user = _context.BUYERS.FirstOrDefault(u => u.ACCOUNT_ID == model.Username || u.EMAIL == model.Username);
            //if (user != null) { return Ok(user); }
            if (user != null && VerifyPassword(model.Password, user.PASSWORD))

            {
                var claims = new List<Claim>
                {
                    //new Claim(ClaimTypes.Name, user.USER_NAME),
                    new Claim(ClaimTypes.NameIdentifier, user.ACCOUNT_ID),
                    new Claim("UserRole", "买家"), // 添加用户角色的 Claim为买家
                    new Claim("UserEmail", user.EMAIL) // 添加用户邮箱
                    //注意，这里的值若为空就会引发后端报错。因此最好携带主码
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // 可以在此处设置额外的 Cookie 属性
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { Message = "登录成功！", Role = "买家", userId = user.ACCOUNT_ID });
            }

            //查看是否为卖家
            var user2 = _context.STORES.FirstOrDefault(u => u.ACCOUNT_ID == model.Username || u.EMAIL == model.Username);
            if (user2 != null && VerifyPassword(model.Password, user2.PASSWORD))
            {
                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user2.ACCOUNT_ID),
                                new Claim("UserRole", "商家"), // 添加用户角色的 Claim为商家
                                new Claim("UserEmail", user2.EMAIL) // 添加用户邮箱
                                //注意，这里的值若为空就会引发后端报错。因此最好携带主码
                            };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // 可以在此处设置额外的 Cookie 属性
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { Message = "登录成功！", Role = "商家",userId = user2.ACCOUNT_ID });
            }

            //查看是否为管理员
            var user3 = _context.ADMINISTRATORS.FirstOrDefault(u => u.ACCOUNT_ID == model.Username || u.EMAIL == model.Username);
            if (user3 != null && VerifyPassword(model.Password, user3.PASSWORD))
            {
                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, user3.ACCOUNT_ID),
                                new Claim("UserRole", "管理员"), // 添加用户角色的 Claim为管理员
                                new Claim("UserEmail", user3.EMAIL) // 添加用户邮箱
                                //注意，这里的值若为空就会引发后端报错。因此最好携带主码
                            };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // 可以在此处设置额外的 Cookie 属性
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return Ok(new { Message = "登录成功！", Role = "管理员", userId = user3.ACCOUNT_ID });
            }
            // 如果验证失败
            if (user != null || user2 != null || user3 != null)
                return Unauthorized(new { Message = "密码错误！" });
            return NotFound(new { Message = "账号不存在！请先注册" });
        }

        /*登出：使用户cookie失效*/
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst("UserRole")?.Value;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new
            {
                UserId = userId,
                UserRole = userRole,
                Message = "登出账号成功！"
            });
            //return Ok(new { Message = "登出账号成功！" });
        }

        /*访问受保护端点：
         * 仅做测试之用，无cookie进不去
         * 当用户访问受保护的端点时，ASP.NET Core 会自动将 Claims 从 Cookie 中提取到 HttpContext.User
         */
        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst("UserRole")?.Value;
            return Ok(new { Message = $"{userRole}{userId}成功进入受保护端点！" });
        }

        /*密码验证：*/
        private bool VerifyPassword(string password, string storedHash)
        {
            // 密码验证
            return PasswordHelper.VerifyPassword(password, storedHash);
        }

        /*修改密码：
         * 输入用户名（邮箱或ID）及新密码*/
        [HttpPost("password_reset")]
        public IActionResult PasswordReset([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            var user = _context.BUYERS.FirstOrDefault(u => u.ACCOUNT_ID == model.Username || u.EMAIL == model.Username);
            if(user != null) {
                if (VerifyPassword(model.Password, user.PASSWORD))
                    return BadRequest(new { message = "新密码不能与旧密码相同！" });

                user.PASSWORD = PasswordHelper.HashPassword(model.Password);

                _context.SaveChanges();
                return Ok(new { message = "密码重置成功！" });
            }
            var user2 = _context.STORES.FirstOrDefault(u => u.ACCOUNT_ID == model.Username || u.EMAIL == model.Username);
            if (user2 == null)
            {
                return NotFound(new { message = "账号不存在！" });
            }
            if (VerifyPassword(model.Password, user2.PASSWORD))
                return BadRequest(new { message = "新密码不能与旧密码相同！" });

            user2.PASSWORD = PasswordHelper.HashPassword(model.Password);

            _context.SaveChanges();
            return Ok(new { message = "密码重置成功！" });

        }

        /*向指定邮箱发送邮箱验证码
         * 由前端判断验证码是否正确*/
        [HttpGet("send_verification_code/{email}")]
        public IActionResult SendVerificationCode(string email)
        {

            Console.WriteLine("进入这个函数");
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "邮箱不能为空。" });
            }
            MailMessage message = new MailMessage();

            // 设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
            MailAddress fromAddr = new MailAddress("1473030672@qq.com");
            message.From = fromAddr;
            // 设置收件人,可添加多个,添加方法与下面的一样
            message.To.Add(email);
            // 设置邮件标题
            message.Subject = "注册验证码";
            // 设置邮件内容
            Random r = new Random();
            var _verificationCode = r.Next(1000, 10000).ToString();
            message.Body = "您好，您正在非遗平台瑕宝阁注册账号，您的验证码为 " + _verificationCode;

            // 设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的邮箱管理后台查看
            //SmtpClient client = new SmtpClient("smtp.outlook.com", 587);
            SmtpClient client = new SmtpClient("smtp.qq.com", 587);
            // 设置发送人的邮箱账号和授权码
            client.Credentials = new NetworkCredential("1473030672@qq.com", "pxdzrvnwxffdbafa");
            //client.Credentials = new NetworkCredential("1239716933@qq.com", "tgfcpyibnajfhiac");

            // 启用ssl,也就是安全发送
            client.EnableSsl = true;
            // 发送邮件
            try
            {
                client.Send(message);
                return Ok(new { message = "验证码已发送", verificationCode = _verificationCode });
            }
            catch (Exception ex)
            {
                return BadRequest("验证码发送失败: " + ex.Message);
            }
        }

        /*注册：*/
        [HttpPost("register")]
        public IActionResult UserRegister([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //方便测试加入这个//////记得删
            var userExists = _context.ACCOUNTS.Any(u => u.EMAIL == model.Email);
            if (userExists)
            {
                return BadRequest(new { message = "邮箱已经存在，不能重复注册！" });
            }

            string newId = idGenerator.GetNextId();

            string uidb = newId;

            if (model.Role == "买家")
            {
                uidb = "U" + uidb;
                _context.BUYERS.Add(new BackendCode.Models.BUYER()
                {
                    ACCOUNT_ID = uidb,
                    EMAIL = model.Email,
                    PASSWORD = PasswordHelper.HashPassword(model.Password),
                    TOTAL_CREDITS = 0

                }) ;
                _context.WALLETS.Add(new WALLET()
                {
                    ACCOUNT_ID = uidb,
                    BALANCE = 0
                });
                _context.SaveChanges();
            }
            else if (model.Role == "商家")
            {
                uidb = "S" + uidb;
                _context.STORES.Add(new BackendCode.Models.STORE()
                {
                    ACCOUNT_ID = uidb,
                    EMAIL = model.Email,
                    PASSWORD = PasswordHelper.HashPassword(model.Password),
                    STORE_SCORE = 0,
                });
                _context.WALLETS.Add(new WALLET()
                {
                    ACCOUNT_ID = uidb,
                    BALANCE = 0
                });
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(new {message="Role字段错误！只能为“买家”或“商家”"});
            }

            return Ok(new {message="注册成功！",accountId= uidb });
        }

        /*检查注册状态*/
        [HttpGet("check_register/{email}")]
        public IActionResult CheckRegister(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "邮箱不能为空。" });
            }
            var userExists = _context.ACCOUNTS.Any(u => u.EMAIL == email);
            if (userExists)
            {
                return BadRequest(new { message = "邮箱已经存在，不能重复注册！" });
            }
            else
            {
                return Ok(new { message = "邮箱未被注册" });
            }
        }

        /*获取特定ID的用户基本信息*/
        [HttpGet("get_user_message/{userId}")]
        public async Task<IActionResult> FindUserMessage(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("用户ID不能为空！");
            }
            char role = userId[0];
            if (role=='U')
            {
                var buyer = await _context.BUYERS
                .Where(b => b.ACCOUNT_ID == userId)
                .SingleOrDefaultAsync();
                if (buyer == null)
                    return NotFound(new { message = "买家ID输入错误" });
                return Ok(new { message = "用户查找成功！",target_user= buyer,target_role="买家" });
            }
            else if(role == 'S')
            {
                var seller = await _context.STORES
                .Where(b => b.ACCOUNT_ID == userId)
                .SingleOrDefaultAsync();
                if (seller == null)
                    return NotFound(new { message = "商家ID输入错误" });
                return Ok(new { message = "用户查找成功！", target_user = seller, target_role = "商家" });
            }
            else
            {
                var administrator = await _context.ADMINISTRATORS
                .Where(b => b.ACCOUNT_ID == userId)
                  .SingleOrDefaultAsync();
                if (administrator == null)
                    return NotFound(new { message = "ID不存在!" });
                return Ok(new { message = "用户查找成功！", target_user = administrator, target_role = "管理员" });
            }
        }

        /*修改买家信息*/
        [HttpPost("modify_buyer_message")]
        [Authorize]
        public async Task<IActionResult>ModifyBuyerMessage([FromBody] BuyerMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            if (model == null)
            {
                return BadRequest("请求体不能为空！");
            }
            if (string.IsNullOrWhiteSpace(model.AccountId))
            {
                return BadRequest("用户ID不能为空！");
            }
            var buyer = await _context.BUYERS
                .Where(b => b.ACCOUNT_ID == model.AccountId)
                .SingleOrDefaultAsync();
            if (buyer == null)
            {
                return NotFound(new { message = "没有找到匹配的用户!" });
            }
            string tip = "";
            if (!string.IsNullOrWhiteSpace(model.Address))
            {
                buyer.ADDRESS= model.Address;
                tip += $"地址修改为{model.Address}；";
            }
            if (!string.IsNullOrWhiteSpace(model.UserName))
            {
                buyer.USER_NAME = model.UserName;
                tip += $"用户名修改为{model.UserName}；";
            }
            if (model.Age.HasValue)
            {
                buyer.AGE = model.Age;
                tip += $"年龄修改为{model.Age}；";
            }
            if (!string.IsNullOrWhiteSpace(model.Gender))
            {
                buyer.GENDER = model.Gender;
                tip += $"性别修改为{model.Gender}；";
            }
            _context.SaveChanges();
            return Ok(new {message="用户信息修改成功！",detail=tip});
        }

        /*修改商家信息*/
        [HttpPost("modify_seller_message")]
        [Authorize]
        public async Task<IActionResult> ModifySellerMessage([FromBody] SellerMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            if (model == null)
            {
                return BadRequest("请求体不能为空！");
            }
            if (string.IsNullOrWhiteSpace(model.AccountId))
            {
                return BadRequest("用户ID不能为空！");
            }

            try
            {
                var seller = await _context.STORES
                .Where(b => b.ACCOUNT_ID == model.AccountId)
                    .SingleOrDefaultAsync();

                if (seller == null)
                {
                    return NotFound(new { message = "没有找到匹配的用户!" });
                }
                string tip = "";
                Console.WriteLine($"地址:{seller.ADDRESS}");
                Console.WriteLine($"ID:{seller.ACCOUNT_ID}");
                Console.WriteLine($"用户名:{seller.USER_NAME}");
                Console.WriteLine($"店铺名:{seller.STORE_NAME}");
                if (!string.IsNullOrWhiteSpace(model.Address))
                {
                    Console.WriteLine($"地址修改为{model.Address}");
                    seller.ADDRESS = model.Address;
                    tip += $"地址修改为{model.Address}；";
                }
                if (!string.IsNullOrWhiteSpace(model.UserName))
                {
                    Console.WriteLine($"用户名修改为{model.UserName}");
                    seller.USER_NAME = model.UserName;
                    tip += $"用户名修改为{model.UserName}；";
                }
                if (!string.IsNullOrWhiteSpace(model.StoreName))
                {
                    Console.WriteLine($"店铺名修改为{model.StoreName}");
                    seller.STORE_NAME = model.StoreName;
                    tip += $"店铺名修改为{model.StoreName}；";
                }
                _context.SaveChanges();
                return Ok(new { message = "用户信息修改成功！", detail = tip });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while querying the posts.");
                throw;
            }
           
        }

        /*修改管理员信息*/
        [HttpPost("modify_administrator_message")]
        [Authorize]
        public async Task<IActionResult> ModifyAdministratorMessage([FromBody] AdministratorMessageModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            if (model == null)
            {
                return BadRequest("请求体不能为空！");
            }
            if (string.IsNullOrWhiteSpace(model.AccountId))
            {
                return BadRequest("用户ID不能为空！");
            }
            var administrator = await _context.ADMINISTRATORS
                .Where(b => b.ACCOUNT_ID == model.AccountId)
                .SingleOrDefaultAsync();
            if (administrator == null)
            {
                return NotFound(new { message = "没有找到匹配的用户!" });
            }
            string tip = "";
            if (!string.IsNullOrWhiteSpace(model.UserName))
            {
                administrator.USER_NAME = model.UserName;
                tip += $"用户名修改为{model.UserName}；";
            }
            _context.SaveChanges();
            return Ok(new { message = "用户信息修改成功！", detail = tip });
        }

        //临时使用
        [HttpPost("hash-passwords")]
        public async Task<IActionResult> HashPasswords(string id)
        {
            var buyer = await _context.BUYERS
                .Where(b => b.ACCOUNT_ID == id)
               .SingleOrDefaultAsync();
            if (buyer == null)
                return NotFound(new { message = "id输入错误" });
            //buyer.PASSWORD=PasswordHelper.HashPassword(buyer.PASSWORD);
            //_context.SaveChanges();

            return Ok("买家寻找完毕");
        }
        //临时使用，添加钱包
        [HttpPost("add-wallet")]
        public async Task<IActionResult> AddWalletsToAllAccounts()
        {
            try
            {
                // 获取所有的账户记录
                var accounts = await _context.ACCOUNTS.ToListAsync();

                foreach (var account in accounts)
                {
                    // 检查该账户是否已经有钱包
                    var existingWallet = await _context.WALLETS
                        .FirstOrDefaultAsync(w => w.ACCOUNT_ID == account.ACCOUNT_ID);

                    if (existingWallet == null)
                    {
                        // 如果没有钱包，则创建一个新的钱包
                        var wallet = new WALLET
                        {
                            ACCOUNT_ID = account.ACCOUNT_ID,
                            BALANCE = 0 // 设置初始余额为 0
                        };

                        // 将钱包添加到数据库
                        _context.WALLETS.Add(wallet);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok("Wallets added to all accounts successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to add wallets: " + ex.Message);
            }
        }


        /*管理员注册（内部用）：*/
        [HttpPost("administrator_register")]
        public IActionResult AdministratorRegister([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            //方便测试加入这个//////记得删
            var userExists = _context.ACCOUNTS.Any(u => u.EMAIL == model.Email);
            if (userExists)
            {
                return BadRequest(new { message = "邮箱已经存在，不能重复注册！" });
            }

            string newId = idGenerator.GetNextId();

            string uidb = newId;

            if (model.Role == "买家")
            {
                uidb = "U" + uidb;
                _context.BUYERS.Add(new BackendCode.Models.BUYER()
                {
                    ACCOUNT_ID = uidb,
                    EMAIL = model.Email,
                    PASSWORD = PasswordHelper.HashPassword(model.Password),
                    TOTAL_CREDITS = 0

                });
                _context.WALLETS.Add(new WALLET()
                {
                    ACCOUNT_ID = uidb,
                    BALANCE = 0
                });
                _context.SaveChanges();
            }
            else if (model.Role == "商家")
            {
                uidb = "S" + uidb;
                _context.STORES.Add(new BackendCode.Models.STORE()
                {
                    ACCOUNT_ID = uidb,
                    EMAIL = model.Email,
                    PASSWORD = PasswordHelper.HashPassword(model.Password),
                    STORE_SCORE = 0,
                });
                _context.WALLETS.Add(new WALLET()
                {
                    ACCOUNT_ID = uidb,
                    BALANCE = 0
                });
                _context.SaveChanges();
            }
            else if (model.Role == "管理员")
            {
                uidb = "A" + uidb;
                _context.ADMINISTRATORS.Add(new BackendCode.Models.ADMINISTRATOR()
                {
                    ACCOUNT_ID = uidb,
                    EMAIL = model.Email,
                    PASSWORD = PasswordHelper.HashPassword(model.Password),
                    PERMISSION_LEVEL = 1,
                });
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(new { message = "Role字段错误！只能为“买家”或“商家”或“管理员”" });
            }

            return Ok(new { message = "注册成功！", accountId = uidb });
        }


        //管理员修改密码（内部用）:
        [HttpPost("administrator_password_reset")]
        public IActionResult AdministratorPasswordReset([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);// 如果模型验证失败，返回错误信息
            }
            var user = _context.ADMINISTRATORS.FirstOrDefault(u => u.ACCOUNT_ID == model.Username || u.EMAIL == model.Username);
            if (user != null)
            {
                if (VerifyPassword(model.Password, user.PASSWORD))
                    return BadRequest(new { message = "新密码不能与旧密码相同！" });

                user.PASSWORD = PasswordHelper.HashPassword(model.Password);

                _context.SaveChanges();
                return Ok(new { message = "密码重置成功！" });
            }
            return NotFound(new { message = "该管理员账号不存在！" });
        }
        /*
                //临时使用
                [HttpPut("update-gender")]
                public async Task<IActionResult> UpdateGenderToFemale()
                {
                    try
                    {
                        // 获取所有 BUYER 表中的记录
                        var buyers = await _context.BUYERS.ToListAsync();

                        // 更新每个 BUYER 的 GENDER 字段为 "Female"
                        foreach (var buyer in buyers)
                        {
                            buyer.GENDER = "女";
                        }

                        // 保存更改
                        await _context.SaveChangesAsync();

                        return Ok("已经修改！");
                    }
                    catch (Exception ex)
                    {
                        // 处理异常并返回错误信息
                        return StatusCode(500, $"修改失败！: {ex.Message}");
                    }
                }*/


        /*    var buyers = await _context.BUYERS.ToListAsync();
 *            foreach (var buyer in buyers)
            {
                if (buyer.ACCOUNT_ID == "U00000012")
                    continue;
                buyer.PASSWORD = PasswordHelper.HashPassword(buyer.PASSWORD);
                Console.WriteLine(buyer.PASSWORD, "+++");
            }*/
    }
}


/*if (ModelState.IsValid)
{
    // 如果模型验证成功，执行注册逻辑
    return Ok(new { message = "Registration successful" });
}
else
{
    // 如果模型验证失败，返回错误信息
    return BadRequest(ModelState);
}*/