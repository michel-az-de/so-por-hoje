using System.Net;

namespace SoPorHoje.Tests.Helpers;

/// <summary>
/// Fake HttpMessageHandler for testing HTTP-dependent code without real network calls.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string? _content;
    private readonly HttpStatusCode _statusCode;
    private readonly TimeSpan? _delay;
    private readonly Exception? _exception;

    public MockHttpMessageHandler(
        string? content = null,
        HttpStatusCode statusCode = HttpStatusCode.OK,
        TimeSpan? delay = null,
        Exception? exception = null)
    {
        _content = content;
        _statusCode = statusCode;
        _delay = delay;
        _exception = exception;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_delay.HasValue)
            await Task.Delay(_delay.Value, cancellationToken);

        if (_exception != null)
            throw _exception;

        return new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_content ?? string.Empty),
        };
    }
}
