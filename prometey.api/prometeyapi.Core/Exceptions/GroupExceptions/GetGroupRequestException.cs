using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prometeyapi.Core.Exceptions.GroupExceptions;

public class GetGroupRequestException : Exception
{
    public GetGroupRequestException(string message) : base(message)
    {
    }

    public GetGroupRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Message
    {
        get
        {
            return "GET_GROUP_REQUEST_EXCEPTION: " + base.Message;
        }
    }
}