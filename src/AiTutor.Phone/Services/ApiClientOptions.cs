namespace AiTutor.Maui.Services;

public class ApiClientOptions
{
    /// <summary>
    /// AiTutor.Api 基础地址。Android 模拟器访问宿主机通常使用 http://10.0.2.2:端口。
    /// </summary>
    public string BaseUrl { get; set; } = "http://10.0.2.2:5088";

    /// <summary>
    /// HTTP 请求超时时间，单位秒。
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
