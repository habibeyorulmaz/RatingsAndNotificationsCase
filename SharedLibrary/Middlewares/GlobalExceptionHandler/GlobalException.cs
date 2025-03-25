using System.Net;

namespace SharedLibrary.Middlewares.GlobalExceptionHandler;

public class GlobalException 
{
    public required string Title { get; set; }
    public HttpStatusCode Status { get; set; }
    public required string Type { get; set; }
    public required string Detail { get; set; }
}