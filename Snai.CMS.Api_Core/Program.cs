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

// ������ǰ����־����
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("storage/logs/log-init-.log",
        rollingInterval: RollingInterval.Day, // ÿ��һ���ĵ�
        retainedFileCountLimit: 24 * 30 // ��ౣ�� 30 ��� Log �ĵ�
    )
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // ע����־�����滻������ǰ����־����
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration) //������
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
    );  

    // ��ע����֤����������
    builder.Services.AddControllers(options => options.Filters.Add<ValidParamsFilter>());

    // ע������
    builder.Services.Configure<LogonSettings>(builder.Configuration.GetSection(nameof(LogonSettings)));
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
    builder.Services.Configure<PwdSaltSettings>(builder.Configuration.GetSection(nameof(PwdSaltSettings)));

    // ע��ȫ�ֱ���
    builder.Services.AddSingleton<Consts>();

    //ע��HttpContext��������Controller֮��ĵط�ʹ��
    builder.Services.AddHttpContextAccessor();

    // ע���������
    builder.Services.AddScoped<HttpContextExtension>();

    // ע�����ݿ�����
    var connectionString = builder.Configuration.GetConnectionString("MySQL");
    builder.Services.AddDbContext<CMSContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    // ע�����ݿ�ʵ��
    builder.Services.AddScoped<CMSDao>();

    // ע��ҵ��ʵ��
    builder.Services.AddScoped<CMSBO>();

    // ע��jwt
    builder.Services.AddSingleton<JwtHelper>();     // JwtHelper ����token��
    builder.Services.AddJwt();                      // ��֤token

    // ע��Ȩ���ж�
    var permissionRequirement = new PermissionRequirement();
    builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
    builder.Services.AddSingleton(permissionRequirement);
    builder.Services.AddAuthorization(option => {
        option.AddPolicy("Permission", policy => policy.AddRequirements(permissionRequirement));
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseAuthentication(); // ��֤
    app.UseAuthorization();  // ��Ȩ

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