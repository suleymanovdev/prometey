namespace prometeyapi.Core.Exceptions.PostExceptions;

public class DeletePostRequestException : Exception
{
    public DeletePostRequestException(string message) : base(message)
    {
    }

    public DeletePostRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "DELETE_POST_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}