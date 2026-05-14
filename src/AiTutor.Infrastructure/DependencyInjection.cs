using AiTutor.Core.Interfaces;
using AiTutor.Infrastructure.Agents;
using AiTutor.Infrastructure.Data;
using AiTutor.Infrastructure.Options;
using AiTutor.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AiTutor.Infrastructure;

/// <summary>
/// Infrastructure 层依赖注入入口。
/// </summary>
/// <remarks>
/// 调用链位置：AiTutor.Api 的 Program 调用 AddAiTutorInfrastructure，
/// 这里集中注册 DbContext、AgentRouter、AgentService、Mock Provider、各个 Agent 和日志服务。
/// 这样 Controller 不需要知道具体实现类，后续 Phase 3 切换真实 DeepSeek / GLM Provider 时，
/// 只需要替换这里的 Provider 注册或增加配置分支。
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// 注册 AiTutor 后端基础设施服务。
    /// </summary>
    /// <param name="services">ASP.NET Core 服务集合。</param>
    /// <param name="configuration">应用配置，用于读取连接字符串。</param>
    /// <returns>注册后的服务集合，便于 Program 链式调用。</returns>
    /// <remarks>
    /// 代码逻辑：
    /// 1. 优先读取 ConnectionStrings:AiTutorDb；
    /// 2. 如果没有配置，则使用本地开发库 AiTutorDb 的 Windows 集成认证连接；
    /// 3. 注册规则路由和 Mock Agent 链路；
    /// 4. 注册保存会话、问题、回答、路由日志和模型调用日志所需的服务。
    /// </remarks>
    public static IServiceCollection AddAiTutorInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AiTutorDb")
            ?? "Server=.;Database=AiTutorDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        services.AddDbContext<AiTutorDbContext>(options => options.UseSqlServer(connectionString));
        services.Configure<AiProviderOptions>(options => ConfigureAiProviderOptions(configuration, options));
        // 模型请求的超时统一由各 Provider 内部的 CancellationTokenSource 控制，
        // 避免 HttpClient 默认 100 秒超时先于 AiProviders:TimeoutSeconds 触发。
        services.AddHttpClient<DeepSeekTextModelProvider>(client =>
        {
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        });
        services.AddHttpClient<GlmVisionModelProvider>(client =>
        {
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        });

        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<IAgentRouter, AgentRouter>();
        services.AddScoped<MockTextModelProvider>();
        services.AddScoped<MockVisionModelProvider>();
        services.AddScoped<ITextModelProvider>(CreateTextModelProvider);
        services.AddScoped<IVisionModelProvider>(CreateVisionModelProvider);
        services.AddScoped<IPromptTemplateService, PromptTemplateService>();
        services.AddScoped<ITextbookKnowledgeService, TextbookKnowledgeService>();
        services.AddScoped<IWrongQuestionService, WrongQuestionService>();
        services.AddScoped<IHomeworkCheckService, HomeworkCheckService>();
        services.AddScoped<IModelCallLogService, ModelCallLogService>();
        services.AddScoped<IAgentRouteLogService, AgentRouteLogService>();
        services.AddScoped<IMediaResourceService, MediaResourceService>();

        services.AddScoped<IAgent, ChatAgent>();
        services.AddScoped<IAgent, VisionAgent>();
        services.AddScoped<IAgent, HomeworkCheckAgent>();
        services.AddScoped<IAgent, WrongBookAgent>();
        services.AddScoped<IAgent, TextbookRagAgent>();
        services.AddScoped<IAgent, VoiceAgent>();
        services.AddScoped<IAgent, AvatarAgent>();

        return services;
    }

    /// <summary>
    /// 根据 AiProviders:UseMock 创建文本模型 Provider。
    /// </summary>
    /// <param name="serviceProvider">ASP.NET Core 依赖注入容器。</param>
    /// <returns>MockTextModelProvider 或 DeepSeekTextModelProvider。</returns>
    /// <remarks>
    /// 调用链：Controller -> AgentService -> ChatAgent -> ITextModelProvider。
    /// UseMock=true 时返回 Mock，确保没有 API Key 也能 Swagger 测试；
    /// UseMock=false 时返回 DeepSeek，实现真实文本模型接入。真实密钥仍由配置系统注入，不写入代码。
    /// </remarks>
    private static ITextModelProvider CreateTextModelProvider(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<AiProviderOptions>>().Value;
        return options.UseMock
            ? serviceProvider.GetRequiredService<MockTextModelProvider>()
            : serviceProvider.GetRequiredService<DeepSeekTextModelProvider>();
    }

    /// <summary>
    /// 根据 AiProviders:UseMock 创建视觉模型 Provider。
    /// </summary>
    /// <param name="serviceProvider">ASP.NET Core 依赖注入容器。</param>
    /// <returns>MockVisionModelProvider 或 GlmVisionModelProvider。</returns>
    /// <remarks>
    /// 调用链：Controller -> AgentService -> VisionAgent -> IVisionModelProvider。
    /// UseMock=true 时保持 Phase 2 的 Mock 图片讲题能力；
    /// UseMock=false 时切换到 GLM 视觉模型，支持通过配置读取 BaseUrl、ApiKey 和 VisionModel。
    /// </remarks>
    private static IVisionModelProvider CreateVisionModelProvider(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<AiProviderOptions>>().Value;
        return options.UseMock
            ? serviceProvider.GetRequiredService<MockVisionModelProvider>()
            : serviceProvider.GetRequiredService<GlmVisionModelProvider>();
    }

    /// <summary>
    /// 从 IConfiguration 手动读取 AI Provider 配置。
    /// </summary>
    /// <param name="configuration">应用配置根对象。</param>
    /// <param name="options">需要填充的 AiProviderOptions 实例。</param>
    /// <remarks>
    /// 调用链：AddAiTutorInfrastructure -> services.Configure -> IOptions&lt;AiProviderOptions&gt;。
    /// 这里不用配置绑定扩展包，直接读取明确字段；这样 appsettings.json、appsettings.Development.json、
    /// 用户机密和环境变量仍然都可以通过 ASP.NET Core 配置系统覆盖这些值。
    /// </remarks>
    private static void ConfigureAiProviderOptions(IConfiguration configuration, AiProviderOptions options)
    {
        options.UseMock = bool.TryParse(configuration["AiProviders:UseMock"], out var useMock) ? useMock : true;
        options.TimeoutSeconds = int.TryParse(configuration["AiProviders:TimeoutSeconds"], out var timeoutSeconds) ? timeoutSeconds : 30;
        options.DeepSeek.BaseUrl = configuration["AiProviders:DeepSeek:BaseUrl"] ?? options.DeepSeek.BaseUrl;
        options.DeepSeek.ApiKey = configuration["AiProviders:DeepSeek:ApiKey"] ?? string.Empty;
        options.DeepSeek.DefaultModel = configuration["AiProviders:DeepSeek:DefaultModel"] ?? options.DeepSeek.DefaultModel;
        options.Zhipu.BaseUrl = configuration["AiProviders:Zhipu:BaseUrl"] ?? options.Zhipu.BaseUrl;
        options.Zhipu.ApiKey = configuration["AiProviders:Zhipu:ApiKey"] ?? string.Empty;
        options.Zhipu.VisionModel = configuration["AiProviders:Zhipu:VisionModel"] ?? options.Zhipu.VisionModel;
    }
}
