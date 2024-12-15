namespace prometeyapi.Core.Exceptions.ApplicationExceptions;

public class DeleteApplicationRequestException : Exception
{
    public DeleteApplicationRequestException(string message) : base(message)
    {
    }

    public DeleteApplicationRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "DELETE_APPLICATION_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}
