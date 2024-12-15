namespace prometeyapi.Core.Exceptions.UserExceptions;

public class DeleteUserRequestException : Exception
{
    public DeleteUserRequestException(string message) : base(message)
    {
    }

    public DeleteUserRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "DELETE_USER_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}