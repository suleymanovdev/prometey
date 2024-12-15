namespace prometeyapi.Core.Exceptions.UserExceptions;

public class UpdateUserRequestException : Exception
{
    public UpdateUserRequestException(string message) : base(message)
    {
    }

    public UpdateUserRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "UPDATE_USER_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}