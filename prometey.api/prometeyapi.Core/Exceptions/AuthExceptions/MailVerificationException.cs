namespace prometeyapi.Core.Exceptions.AuthExceptions;

public class MailVerificationException : Exception
{
    public MailVerificationException(string message) : base(message)
    {
    }

    public MailVerificationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "MAIL_VERIFICATION_EXCEPTION: " + base.Message + " Verification process failed. Please try again.";
        }
    }
}