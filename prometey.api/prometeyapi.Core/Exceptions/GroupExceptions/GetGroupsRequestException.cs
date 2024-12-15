namespace prometeyapi.Core.Exceptions.GroupExceptions;

public class GetGroupsRequestException : Exception
{
    public GetGroupsRequestException(string message) : base(message)
    {
    }

    public GetGroupsRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "GET_GROUPS_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}
