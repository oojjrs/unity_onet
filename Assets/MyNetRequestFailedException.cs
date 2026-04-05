using System;

namespace oojjrs.onet
{
    /// <summary>
    /// <see cref="MyNetRequest"/>랑은 관련 없음
    /// </summary>
    public class MyNetRequestFailedException : Exception
    {
        public int ErrorCode { get; }

        internal MyNetRequestFailedException(int errorCode, string message)
            : this(errorCode, message, null)
        {
        }

        internal MyNetRequestFailedException(int errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
