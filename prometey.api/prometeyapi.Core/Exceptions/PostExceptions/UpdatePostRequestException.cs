namespace prometeyapi.Core.Exceptions.PostExceptions;

public class UpdatePostRequestException : Exception
{
    public UpdatePostRequestException(string message) : base(message)
    {
    }

    public UpdatePostRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "UPDATE_POST_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}