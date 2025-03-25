namespace SharedLibrary.Middlewares.GlobalExceptionHandler;

public class BusinessException : Exception
{
    public BusinessException(string message, string title = "Business Validation Exception", int status = 400)
        : base(message)
    {
        Title = title;
        ExceptionMessage = message;
        Status = status;
    }

    public BusinessException() : base()
    {
    }

    public BusinessException(string? message) : base(message)
    {
        ExceptionMessage = message ?? string.Empty;
    }

    public BusinessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public string Title { get; set; }
    public int Status { get; set; }
    public string ExceptionMessage { get; set; }
}