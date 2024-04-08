using Serilog;
using Serilog.Events;

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
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration) //读配置
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
    );  

    builder.Services.AddControllers();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseAuthorization();

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