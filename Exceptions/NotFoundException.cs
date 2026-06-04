namespace TimeRecord.Exceptions;

public class NotFoundException : Exception
{
    public int StatusCode { get; }
    public NotFoundException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}