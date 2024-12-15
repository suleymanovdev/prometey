namespace prometeyapi.Core.Exceptions.ApplicationExceptions;

public class CreateApplicationRequestException : Exception
{
    public CreateApplicationRequestException(string message) : base(message)
    {
    }
    
    public CreateApplicationRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "CREATE_APPLICATION_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}
