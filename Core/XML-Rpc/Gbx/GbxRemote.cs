using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace MPNextControl.Core.XML_Rpc.Gbx
{
    public class GbxRemote
    {
        public string Ip;
        public int Port;

        public byte[] Buffer;

        public Socket tcpSocket;

        private AutoResetEvent callRead = new AutoResetEvent(false);

        public event GbxCallbackHandler EventGbxCallback;
        public event OnDisconnectHandler EventOnDisconnectCallback;

        private int requests;
        private Hashtable responses = new Hashtable();
        private Hashtable callbackList = new Hashtable();

        public GbxRemote(string ip, int port)
        {
            Ip = string.IsNullOrEmpty(ip) ? "127.0.0.1" : ip;
            Port = port;
        }

        public bool TryConnect()
        {
            var timeOut = 5000;
            var triesLeft = 3;
            var IPEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);

            tcpSocket = new Socket(IPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the socket...
            tcpSocket.Connect(IPEndPoint);

            // First, we need to check if it's connected
            while(!tcpSocket.Connected)
            {
                if (triesLeft <= 0)
                {
                    throw new CannotConnectException();
                }
                Thread.Sleep(timeOut);
            }

            HandShake();

            Buffer = new byte [ 8 ];
            tcpSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), null);

            return true;
        }

        public bool HandShake()
        {
            if (tcpSocket.Connected)
            {
                // get size ...
                var buffer = new byte[4];
                tcpSocket.Receive(buffer);
                int Size = BitConverter.ToInt32(buffer, 0);

                // get handshake ...
                byte[] HandshakeBuffer = new byte[Size];
                tcpSocket.Receive(HandshakeBuffer);
                string Handshake = Encoding.UTF8.GetString(HandshakeBuffer);

                // check if compatible ...
                if (Handshake != "GBXRemote 2")
                    return false;
                else
                    return true;
            }
            throw new ServerNotConnectedException();
        }

        private void OnDisconnectCallback()
        {
            if (EventOnDisconnectCallback != null)
                EventOnDisconnectCallback(this);
        }

        private void OnGbxCallback(GbxCallbackEventArgs e)
        {
            if (EventGbxCallback != null)
            {
                EventGbxCallback(this, e);
                Console.WriteLine(e.Response.MethodName);
            }
        }

        public void OnReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                tcpSocket.EndReceive(asyncResult);

                GbxCall call = ToGbxCall(tcpSocket, Buffer);

                // watch out for the next calls ...
                Buffer = new byte [ 8 ];
                asyncResult = tcpSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveData), null);

                if (call.Type == MessageTypes.Callback)
                {

                    // throw new event ...
                    GbxCallbackEventArgs eArgs = new GbxCallbackEventArgs(call);
                    OnGbxCallback(eArgs);
                }
                else
                {

                    // add the response to the queue ...
                    lock (this)
                    {
                        responses.Add(call.Handle, call);
                    }

                    // callback if any method was set ...
                    if (callbackList [ call.Handle ] != null)
                    {
                        ((GbxCallCallbackHandler)callbackList [ call.Handle ]).BeginInvoke(call, null, null);
                        callbackList.Remove(call.Handle);
                    }
                }
            }
            catch
            {

            }
            finally
            {
                callRead.Set();
            }
        }

        /*
        * -- 
        *
        */

        private static bool SendRpc(Socket inSocket, byte [ ] inData)
        {
            int offset = 0;
            int len = inData.Length;
            int bytesSent;
            try
            {
                while (len > 0)
                {
                    bytesSent = inSocket.Send(inData, offset, len, SocketFlags.None);
                    len -= bytesSent;
                    offset += bytesSent;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static byte [ ] ReceiveRpc(Socket inSocket, int inLength)
        {
            byte[] data = new byte[inLength];
            int offset = 0;
            byte[] buffer;
            while (inLength > 0)
            {
                int read = Math.Min(inLength, 1024);
                buffer = new byte [ read ];
                int bytesRead = inSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                Array.Copy(buffer, 0, data, offset, buffer.Length);
                inLength -= bytesRead;
                offset += bytesRead;
            }
            return data;
        }

        public static int SendCall(Socket inSocket, GbxCall inCall)
        {
            if (inSocket == null || inCall == null) return 0;
            if (inSocket.Connected)
            {
                lock (inSocket)
                {
                    try
                    {
                        // create request body ...
                        byte[] body = Encoding.UTF8.GetBytes(inCall.Xml);

                        // create response header ...
                        byte[] bSize = BitConverter.GetBytes(body.Length);
                        byte[] bHandle = BitConverter.GetBytes(inCall.Handle);

                        // create call data ...
                        byte[] call = new byte[bSize.Length + bHandle.Length + body.Length];
                        Array.Copy(bSize, 0, call, 0, bSize.Length);
                        Array.Copy(bHandle, 0, call, 4, bHandle.Length);
                        Array.Copy(body, 0, call, 8, body.Length);

                        // send call ...
                        inSocket.Send(call);

                        return inCall.Handle;
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            throw new ServerNotConnectedException();
        }

        public GbxCall ToGbxCall(Socket socket, byte [ ] buffer)
        {
            if (socket.Connected)
            {
                lock (socket)
                {
                    byte[] bSize = new byte[4];
                    byte[] bHandle = new byte[4];

                    if (buffer == null)
                    {
                        socket.Receive(bSize);
                        socket.Receive(bHandle);
                    }
                    else
                    {
                        Array.Copy(buffer, 0, bSize, 0, 4);
                        Array.Copy(buffer, 4, bHandle, 0, 4);
                    }
                    int size = BitConverter.ToInt32(bSize, 0);
                    int handle = BitConverter.ToInt32(bHandle, 0);

                    // receive response body ...
                    byte[] data = ReceiveRpc(socket, size);

                    // parse the response ...
                    GbxCall call = new GbxCall(handle, data);

                    return call;
                }
            }
            throw new ServerNotConnectedException();
        }

        public async void Request(string methodName, object[] Params)
        {
            await AsyncRequest(methodName, Params);
        }

        /// <summary>
        /// Sends a request to the server.
        /// This method should be executed in a async function with await parameter.
        /// If not, if you don't need to have a return, use <see cref="Request(string, object[])"/>
        /// But if you want to have a return but in a non async function, do AsyncRequest(...).Wait();, it block until there is a response
        /// </summary>
        /// <param name="inMethodName">The method to call.</param>
        /// <param name="inParams">Parameters describing your request.</param>
        /// <returns>Returns a response object from the server.</returns>
        public async Task<GbxCall> AsyncRequest(string inMethodName, object [ ] inParams)
        {
            // reset event ...
            callRead.Reset();

            var timeOut = Environment.TickCount + 5000;

            // send the call and remember the handle we are waiting on ...
            GbxCall Request = new GbxCall(inMethodName, inParams);
            Request.Handle = --this.requests;
            int handle = SendCall(this.tcpSocket, Request);

            // wait until we received the call ...
            await Task.Run(() =>
            {
                while (responses [ handle ] == null && tcpSocket.Connected && timeOut > Environment.TickCount)
                {
                    callRead.WaitOne();
                }
                if (timeOut < Environment.TickCount) tcpSocket.Disconnect(true);
            }).ContinueWith(t => t);

            // did we get disconnected ?
            if (!tcpSocket.Connected)
            {
                // WHATEVER, WE NEED THAT THE GLORIOUS CONTROLLER CAN SEND THIS GLORIOUS XMLRPC CALL
                // SO WE ARE GOING TO RECONNECT TO THIS PEASANT SERVER ONCE FOR ALL
                // AND SEND THIS GLORIOUS XMLRPC CALL MASTER RACE
                tcpSocket.Close();
                TryConnect();
                return await AsyncRequest(inMethodName, inParams);
            }

            // get the call and return it ...
            return GetResponse(handle);
        }

        /// <summary>
        /// Sends a request to the server and blocks until a response has been received.
        /// </summary>
        /// <param name="inMethodName">The method to call.</param>
        /// <param name="inParams">Parameters describing your request.</param>
        /// <returns>Returns a response object from the server.</returns>
        public async Task<GbxCall> AsyncRequest(string inMethodName, object inParams)
        {
            // reset event ...
            callRead.Reset();

            // send the call and remember the handle we are waiting on ...
            GbxCall Request = new GbxCall(inMethodName, inParams);
            Request.Handle = --this.requests;
            int handle = SendCall(this.tcpSocket, Request);

            // wait until we received the call ...
            do
            {
                callRead.WaitOne();
            } while (responses [ handle ] == null && tcpSocket.Connected);

            // did we get disconnected ?
            if (!tcpSocket.Connected)
            {
                Console.WriteLine("ERROR! <NotConnectedException");
                return null;
            }

            // get the call and return it ...
            return GetResponse(handle);
        }

        /// <summary>
        /// Sends a Request and does not wait for a response of the server.
        /// The response will be written into a buffer or you can set a callback method
        /// that will be executed.
        /// </summary>
        /// <param name="inMethodName">The method to call.</param>
        /// <param name="inParams">Parameters describing your request.</param>
        /// <param name="callbackHandler">An optional delegate which is callen when the response is available otherwise set it to null.</param>
        /// <returns>Returns a handle to your request.</returns>
        public int AsyncRequest(string inMethodName, object [ ] inParams, GbxCallCallbackHandler callbackHandler)
        {
            // send the call and remember the handle ...
            GbxCall Request = new GbxCall(inMethodName, inParams);
            Request.Handle = --this.requests;
            int handle = SendCall(this.tcpSocket, Request);

            lock (this)
            {
                if (handle != 0)
                {
                    // register a callback on this request ...
                    if (callbackHandler != null)
                    {

                        callbackList.Add(handle, callbackHandler);
                    }

                    // return handle id ...
                    return handle;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets an asynchron response from the list.
        /// </summary>
        /// <param name="inHandle">The handle which was returned from AsyncRequest.</param>
        /// <returns>Returns the cached response.</returns>
        public GbxCall GetResponse(int inHandle)
        {
            return (GbxCall)responses [ inHandle ];
        }

        public delegate void GbxCallbackHandler(GbxRemote o, GbxCallbackEventArgs e);
        public delegate void OnDisconnectHandler(GbxRemote o);
        public delegate void GbxCallCallbackHandler(GbxCall res);

        public class GbxCallbackEventArgs : EventArgs
        {
            public readonly GbxCall Response;

            public GbxCallbackEventArgs(GbxCall response)
            {
                Response = response;
            }
        }
    }
}
