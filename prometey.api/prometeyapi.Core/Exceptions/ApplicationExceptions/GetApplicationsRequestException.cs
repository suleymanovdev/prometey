namespace prometeyapi.Core.Exceptions.ApplicationExceptions;

public class GetApplicationsRequestException : Exception
{
    public GetApplicationsRequestException(string message) : base(message)
    {
    }

    public GetApplicationsRequestException(string message, Exception inner) : base(message, inner)
    {
    }

    public override string Message
    {
        get
        {
            return "GET_APPLICATIONS_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}