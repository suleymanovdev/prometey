namespace prometeyapi.Core.Exceptions.ApplicationExceptions;

public class GetApplicationRequestException : Exception
{
    public GetApplicationRequestException(string message) : base(message)
    {
    }

    public GetApplicationRequestException(string message, Exception inner) : base(message, inner)
    {
    }

    public override string Message
    {
        get
        {
            return "GET_APPLICATION_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}