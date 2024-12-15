namespace prometeyapi.Core.Exceptions.AuthExceptions;

public class SignUpRequestException : Exception
{
    public SignUpRequestException(string message) : base(message)
    {
    }

    public SignUpRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "SIGNUP_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}
