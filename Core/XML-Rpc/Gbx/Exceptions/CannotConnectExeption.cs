using System;
using System.Runtime.Serialization;

namespace MPNextControl.Core.XML_Rpc.Gbx
{
    [Serializable]
    internal class CannotConnectException : Exception
    {
        public CannotConnectException()
        {
        }

        public CannotConnectException(string message) : base(message)
        {
        }

        public CannotConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotConnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}