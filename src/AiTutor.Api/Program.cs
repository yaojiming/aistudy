using AiTutor.Infrastructure;
using Serilog;

var logDirectory = Path.Combine(AppContext.BaseDirectory, "Log");
var logFilePath = Path.Combine(logDirectory, "aitutor-api-.log");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        logFilePath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true,
        encoding: System.Text.Encoding.UTF8,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAiTutorInfrastructure(builder.Configuration);

    var app = builder.Build();

    var swaggerEnabled = app.Environment.IsDevelopment()
        || builder.Configuration.GetValue<bool>("Swagger:Enabled");
    if (swaggerEnabled)
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    var httpsRedirectionEnabled = builder.Configuration.GetValue<bool>("HttpsRedirection:Enabled");
    if (httpsRedirectionEnabled)
    {
        app.UseHttpsRedirection();
    }

    app.UseSerilogRequestLogging();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
