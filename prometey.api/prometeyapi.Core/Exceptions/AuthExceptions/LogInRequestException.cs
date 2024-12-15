namespace prometeyapi.Core.Exceptions.AuthExceptions;

public class LogInRequestException : Exception
{
    public LogInRequestException(string message) : base(message)
    {
    }

    public LogInRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "LOGIN_REQUEST_EXCEPTION: " + base.Message + " Please check the email or password and try again.";
        }
    }
}