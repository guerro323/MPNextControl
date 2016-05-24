using System;
using System.Runtime.Serialization;

namespace MPNextControl.Core.XML_Rpc.Gbx
{
    [Serializable]
    internal class ServerNotConnectedException : Exception
    {
        public ServerNotConnectedException()
        {
        }

        public ServerNotConnectedException(string message) : base(message)
        {
        }

        public ServerNotConnectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServerNotConnectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}