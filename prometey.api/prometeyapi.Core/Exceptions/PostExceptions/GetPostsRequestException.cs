namespace prometeyapi.Core.Exceptions.PostExceptions;

public class GetPostsRequestException : Exception
{
    public GetPostsRequestException(string message) : base(message)
    {
    }

    public GetPostsRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "GET_POSTS_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}