using Serilog;
using Serilog.Events;

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
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration) //������
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