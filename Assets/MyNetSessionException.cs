using System;
using Unity.Services.Multiplayer;

namespace oojjrs.onet
{
    public class MyNetSessionException : Exception
    {
        public SessionError Error { get; private set; }
        public string ErrorString => Error.ToString();

        internal MyNetSessionException(string message, SessionError error, Exception innerException)
            : base(message, innerException)
        {
            Error = error;
        }
    }
}
