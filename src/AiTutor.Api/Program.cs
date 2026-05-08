using AiTutor.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 后端 API 只使用控制台日志，避免 Windows EventLog 在本地开发权限不足时导致请求异常中断。
// 调用链：Kestrel/中间件/Controller -> ILogger -> ConsoleLogger；模型 Provider 的异常也会走这条安全日志链路。
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAiTutorInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
