namespace prometeyapi.Core.Exceptions.PostExceptions;

public class GetPostRequestException : Exception
{
    public GetPostRequestException(string message) : base(message)
    {
    }

    public GetPostRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "GET_POST_REQUEST_EXCEPTION" + base.Message;
        }
    }
}