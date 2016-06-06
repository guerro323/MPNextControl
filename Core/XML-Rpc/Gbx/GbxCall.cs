using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MPNextControl.Core.XML_Rpc.Gbx
{
    public enum MessageTypes
    {
        None,
        Response,
        Request,
        Callback
    }

    public class GbxCall
    {
        private int _mHandle;
        private readonly string _mXml;
        private readonly ArrayList _mParams  = new ArrayList();
        private readonly bool _mError = false;
        private readonly string _mErrorString;
        private readonly int _mErrorCode;
        private string _mMethodName;
        private readonly MessageTypes _mType;

        /// <summary>
        /// Parses an incoming message.
        /// Xml to object.
        /// </summary>
        /// <param name="in_handle"></param>
        /// <param name="in_data"></param>
        public GbxCall(int inHandle, byte [ ] inData)
        {
            _mType = MessageTypes.None;
            _mHandle = inHandle;
            _mXml = Encoding.UTF8.GetString(inData);
            _mErrorCode = 0;
            _mErrorString = "";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_mXml);
            XmlElement methodParams = null;

            // message is of type request ...
            if (xmlDoc [ "methodCall" ] != null)
            {
                // check message type ...
                if (inHandle > 0)
                    _mType = MessageTypes.Callback;
                else
                    _mType = MessageTypes.Request;

                // try to get the method name ...
                if (xmlDoc [ "methodCall" ] [ "methodName" ] != null)
                {
                    _mMethodName = xmlDoc [ "methodCall" ] [ "methodName" ].InnerText;
                }
                else
                    _mError = true;

                // try to get the mehtod's parameters ...
                if (xmlDoc [ "methodCall" ] [ "params" ] != null)
                {
                    _mError = false;
                    methodParams = xmlDoc [ "methodCall" ] [ "params" ];
                }
                else
                    _mError = true;
            }
            else if (xmlDoc [ "methodResponse" ] != null) // message is of type response ...
            {
                // check message type ...
                _mType = MessageTypes.Response;

                if (xmlDoc [ "methodResponse" ] [ "fault" ] != null)
                {
                    Hashtable errStruct = (Hashtable)GbxParser.ParseXml(xmlDoc["methodResponse"]["fault"]);
                    _mErrorCode = (int)errStruct [ "faultCode" ];
                    _mErrorString = (string)errStruct [ "faultString" ];
                    _mError = true;
                }
                else if (xmlDoc [ "methodResponse" ] [ "params" ] != null)
                {
                    _mError = false;
                    methodParams = xmlDoc [ "methodResponse" ] [ "params" ];
                }
                else
                {
                    _mError = true;
                }
            }
            else
            {
                _mError = true;
            }

            // parse each parameter of the message, if there are any ...
            if (methodParams != null)
            {
                foreach (XmlElement param in methodParams)
                {
                    _mParams.Add(GbxParser.ParseXml(param));
                }
            }
        }

        /// <summary>
        /// Parses a response message.
        /// Object to xml.
        /// </summary>
        /// <param name="in_params"></param>
        public GbxCall(object [ ] inParams)
        {
            _mXml = "<?xml version=\"1.0\" ?>\n";
            _mXml += "<methodResponse>\n";
            _mXml += "<params>\n";
            foreach (object param in inParams)
            {
                _mXml += "<param>" + GbxParser.ParseObject(param) + "</param>\n";
            }
            _mXml += "</params>";
            _mXml += "</methodResponse>";
        }

        /// <summary>
        /// Parses a request message.
        /// Object to xml.
        /// </summary>
        /// <param name="in_method_name"></param>
        /// <param name="in_params"></param>
        public GbxCall(string inMethodName, object [ ] inParams)
        {

            _mXml = "<?xml version=\"1.0\" ?>\n";
            _mXml += "<methodCall>\n";
            _mXml += "<methodName>" + inMethodName + "</methodName>\n";
            _mXml += "<params>\n";
            foreach (object param in inParams)
            {
                _mXml += "<param>" + GbxParser.ParseObject(param) + "</param>\n";
            }
            _mXml += "</params>";
            _mXml += "</methodCall>";
            if (inMethodName == "Authenticate") Console.WriteLine(_mXml);
        }

        /// <summary>
        /// Parses a request message.
        /// Object to xml.
        /// </summary>
        /// <param name="in_method_name"></param>
        /// <param name="in_params"></param>
        public GbxCall(string inMethodName, object inParams)
        {
            _mXml = "<?xml version=\"1.0\" ?>\n";
            _mXml += "<methodCall>\n";
            _mXml += "<methodName>" + inMethodName + "</methodName>\n";
            _mXml += "<params>\n";
            _mXml += "<param>" + GbxParser.ParseObject(inParams) + "</param>\n";
            _mXml += "</params>";
            _mXml += "</methodCall>";
        }

        public string MethodName
        {
            get
            {
                return _mMethodName;
            }
            set
            {
                _mMethodName = value;
            }
        }

        public MessageTypes Type
        {
            get
            {
                return _mType;
            }
        }

        public string Xml
        {
            get
            {
                return _mXml;
            }
        }

        public ArrayList Params
        {
            get
            {
                return _mParams;
            }
        }

        public int Size
        {
            get
            {
                return _mXml.Length;
            }
        }

        public int Handle
        {
            get
            {
                return _mHandle;
            }
            set
            {
                _mHandle = value;
            }
        }

        public bool Error
        {
            get
            {
                return _mError;
            }
        }

        public string ErrorString
        {
            get
            {
                return _mErrorString;
            }
        }

        public int ErrorCode
        {
            get
            {
                return _mErrorCode;
            }
        }
    }
}
