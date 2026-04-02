using System;

namespace oojjrs.onet
{
    public class MyNetException : Exception
    {
        public int ErrorCode { get; }
        public string Reason { get; }

        internal MyNetException(string reason, int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Reason = reason;
        }
    }
}
