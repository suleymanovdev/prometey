namespace prometeyapi.Core.Exceptions.UserExceptions;

public class GetUserRequestException : Exception
{
    public GetUserRequestException(string message) : base(message)
    {
    }

    public GetUserRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "GET_USER_REQUEST_EXCEPTION: " + base.Message + " Please check the request and try again.";
        }
    }
}