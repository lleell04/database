using Microsoft.EntityFrameworkCore;
using BackendCode.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Yitter.IdGenerator;
using Alipay.AopSdk.AspnetCore;
using System.Configuration;

var Idoptions = new IdGeneratorOptions();
YitIdHelper.SetIdGenerator(Idoptions);

var builder = WebApplication.CreateBuilder(args);

// 配置日志记录
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

//cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/account/login"; //设置登录路径
        options.AccessDeniedPath = "/api/account/accessdenied"; //设置拒绝访问路径
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); //设置Cookie过期时间

        options.Cookie.HttpOnly = true;

        //////////注意注意注意注意：想用本地前端，用下面这两句。
/*        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None; //允许跨站Cookie*/

        /////////注意注意注意注意：想用服务器，用下面这两句
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Lax; // 或者 SameSiteMode.Strict

    });

//跨域请求
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
               builder => builder
                   //.WithOrigins("http://localhost:8080")
                   .WithOrigins("http://localhost:5173")
                   //.WithOrigins("http://127.0.0.1:5173")//前端根据实际修改
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<YourDbContext>(options => {
    string connectionString = builder.Configuration.GetConnectionString("OracleConnection"); ;
    options.UseOracle(connectionString);
});

// 添加支付宝服务
builder.Services.AddAlipay(builder.Configuration.GetSection("Alipay"));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();//开发环境下显示详细错误页面
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowSpecificOrigin");// 使用CORS策略
}

//app.UseHttpsRedirection();
app.UseAuthentication(); // 添加认证中间件
app.UseAuthorization();// 添加授权中间件

app.MapControllers();

app.Run();