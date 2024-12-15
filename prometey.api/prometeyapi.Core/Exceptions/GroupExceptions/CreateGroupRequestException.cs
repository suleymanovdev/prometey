namespace prometeyapi.Core.Exceptions.GroupExceptions;

public class CreateGroupRequestException : Exception
{
    public CreateGroupRequestException(string message) : base(message)
    {
    }

    public CreateGroupRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "CREATE_GROUP_REQUEST_EXCEPTION: " + base.Message + " Please check the group name and try again.";
        }
    }
}