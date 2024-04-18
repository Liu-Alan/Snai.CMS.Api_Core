using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Serilog;
using Serilog.Events;
using Snai.CMS.Api_Core.Business;
using Snai.CMS.Api_Core.Common.Infrastructure;
using Snai.CMS.Api_Core.Common.Infrastructure.Auth;
using Snai.CMS.Api_Core.Common.Infrastructure.Extension;
using Snai.CMS.Api_Core.Common.Infrastructure.Filters;
using Snai.CMS.Api_Core.Common.Infrastructure.Jwt;
using Snai.CMS.Api_Core.DataAccess;
using Snai.CMS.Api_Core.Entities.Settings;

// 读配置前的日志服务
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("storage/logs/log-init-.log",
        rollingInterval: RollingInterval.Day, // 每天一个文档
        retainedFileCountLimit: 24 * 30 // 最多保留 30 天的 Log 文档
    )
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // 注册日志服务，替换读配置前的日志服务
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration) //读配置
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
    );  

    // 加注入验证参数过滤器
    builder.Services.AddControllers(options => options.Filters.Add<ValidParamsFilter>());

    // 注册配置
    builder.Services.Configure<LogonSettings>(builder.Configuration.GetSection(nameof(LogonSettings)));
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
    builder.Services.Configure<PwdSaltSettings>(builder.Configuration.GetSection(nameof(PwdSaltSettings)));

    // 注册全局变量
    builder.Services.AddSingleton<Consts>();

    //注册HttpContext，用于在Controller之外的地方使用
    builder.Services.AddHttpContextAccessor();

    // 注册基础工具
    builder.Services.AddScoped<HttpContextExtension>();

    // 注册数据库连接
    var connectionString = builder.Configuration.GetConnectionString("MySQL");
    builder.Services.AddDbContext<CMSContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    // 注册数据库实现
    builder.Services.AddScoped<CMSDao>();

    // 注册业务实现
    builder.Services.AddScoped<CMSBO>();

    // 注册jwt
    builder.Services.AddSingleton<JwtHelper>();     // JwtHelper 生成token等
    builder.Services.AddJwt();                      // 验证token

    // 注册权限判断
    var permissionRequirement = new PermissionRequirement();
    builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
    builder.Services.AddSingleton(permissionRequirement);
    builder.Services.AddAuthorization(option => {
        option.AddPolicy("Permission", policy => policy.AddRequirements(permissionRequirement));
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseAuthentication(); // 认证
    app.UseAuthorization();  // 授权

    app.MapControllers();

    app.Run();
}
catch (Exception e)
{
    Log.Error(e, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}