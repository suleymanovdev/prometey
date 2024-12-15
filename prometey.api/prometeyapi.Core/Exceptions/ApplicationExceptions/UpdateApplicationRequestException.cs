public class UpdateApplicationRequestException : Exception
{
    public UpdateApplicationRequestException(string message) : base(message)
    {
    }

    public UpdateApplicationRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "UPDATE_APPLICATION_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}
