using Microsoft.AspNetCore.Http;
using System.Text;

public class StreamResult : IResult
{
    private readonly IAsyncEnumerable<string> _stream;

    public StreamResult(IAsyncEnumerable<string> stream)
    {
        _stream = stream;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.ContentType = "text/plain; charset=utf-8";

        await foreach (var chunk in _stream)
        {
            if (!string.IsNullOrEmpty(chunk))
            {
                var bytes = Encoding.UTF8.GetBytes(chunk);
                await httpContext.Response.Body.WriteAsync(bytes, httpContext.RequestAborted);
                await httpContext.Response.Body.FlushAsync(httpContext.RequestAborted);
            }
        }
    }
}