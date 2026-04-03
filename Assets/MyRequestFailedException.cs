using System;

namespace oojjrs.onet
{
    public class MyRequestFailedException : Exception
    {
        public int ErrorCode { get; }

        internal MyRequestFailedException(int errorCode, string message)
            : this(errorCode, message, null)
        {
        }

        internal MyRequestFailedException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
