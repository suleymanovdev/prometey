namespace prometeyapi.Core.Exceptions.PostExceptions;

public class CreatePostRequestException : Exception
{
    public CreatePostRequestException(string message) : base(message)
    {
    }

    public CreatePostRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "CREATE_POST_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}