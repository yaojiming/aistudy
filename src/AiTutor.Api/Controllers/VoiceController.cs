using AiTutor.Core.Interfaces;
using AiTutor.Shared.Voice;
using Microsoft.AspNetCore.Mvc;

namespace AiTutor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VoiceController : ControllerBase
{
    private readonly ISpeechToTextService _speechToTextService;
    private readonly ITextToSpeechService _textToSpeechService;
    private readonly ILogger<VoiceController> _logger;

    public VoiceController(
        ISpeechToTextService speechToTextService,
        ITextToSpeechService textToSpeechService,
        ILogger<VoiceController> logger)
    {
        _speechToTextService = speechToTextService;
        _textToSpeechService = textToSpeechService;
        _logger = logger;
    }

    [HttpPost("asr")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    [ProducesResponseType(typeof(SpeechToTextResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<SpeechToTextResult>> ConvertSpeechToText(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return Ok(new SpeechToTextResult
            {
                Success = false,
                ErrorMessage = "录音文件为空"
            });
        }

        _logger.LogInformation(
            "Voice ASR upload received. FileName={FileName}, ContentType={ContentType}, Length={Length}",
            file.FileName,
            file.ContentType,
            file.Length);

        await using var stream = file.OpenReadStream();
        var result = await _speechToTextService.ConvertSpeechToTextAsync(
            stream,
            file.FileName,
            file.ContentType,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("tts")]
    [ProducesResponseType(typeof(TtsResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<TtsResult>> ConvertTextToSpeech([FromBody] TtsRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return Ok(new TtsResult
            {
                Success = false,
                ErrorMessage = "合成文本为空"
            });
        }

        _logger.LogInformation("Voice TTS request received. TextLength={TextLength}", request.Text.Length);
        var result = await _textToSpeechService.ConvertTextToSpeechAsync(request.Text, cancellationToken);
        return Ok(result);
    }
}

