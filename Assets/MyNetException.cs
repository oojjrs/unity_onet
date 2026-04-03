using System;

namespace oojjrs.onet
{
    public class MyNetException : MyRequestFailedException
    {
        public string Reason { get; }

        internal MyNetException(string reason, int errorCode, string message, Exception innerException)
            : base(errorCode, message, innerException)
        {
            Reason = reason;
        }
    }
}
