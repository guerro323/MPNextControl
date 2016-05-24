using System;
using System.Text;
using System.Xml;
using System.Collections;
using System.Web;

namespace MPNextControl.Core.XML_Rpc.Gbx
{
    public static class GbxParser
    {

        static public string ParseObject(object inParam)
        {
            // open parameter ...
            string xml = "<value>";

            if (inParam.GetType() == typeof(string)) // parse type string ...
            {
                xml += "<string>" + HttpUtility.HtmlEncode((string)inParam) + "</string>";
            }
            else if (inParam.GetType() == typeof(int)) // parse type int32 ...
            {
                xml += "<int>" + (int)inParam + "</int>";
            }
            else if (inParam.GetType() == typeof(double)) // parse type double ...
            {
                xml += "<double>" + (double)inParam + "</double>";
            }
            else if (inParam.GetType() == typeof(bool))  // parse type bool ...
            {
                if ((bool)inParam)
                    xml += "<boolean>1</boolean>";
                else
                    xml += "<boolean>0</boolean>";
            }
            else if (inParam.GetType() == typeof(ArrayList)) // parse type array ...
            {
                xml += "<array><data>";
                foreach (object element in ((ArrayList)inParam))
                {
                    xml += ParseObject(element);
                }
                xml += "</data></array>";
            }
            else if (inParam.GetType() == typeof(Hashtable)) // parse type struct ...
            {
                xml += "<struct>";
                foreach (object key in ((Hashtable)inParam).Keys)
                {
                    xml += "<member>";
                    xml += "<name>" + key.ToString() + "</name>";
                    xml += ParseObject(((Hashtable)inParam) [ key ]);
                    xml += "</member>";
                }
                xml += "</struct>";
            }
            else if (inParam.GetType() == typeof(byte [ ])) // parse type of byte[] into base64
            {
                xml += "<base64>";
                xml += Convert.ToBase64String((byte [ ])inParam);
                xml += "</base64>";
            }

            // close parameter ...
            return xml + "</value>\n";
        }

        static public object ParseXml(XmlElement inParam)
        {
            XmlElement val;
            if (inParam [ "value" ] == null)
            {
                val = inParam;
            }
            else
            {
                val = inParam [ "value" ];
            }

            if (val [ "string" ] != null) // param of type string ...
            {
                return val [ "string" ].InnerText;
            }
            else if (val [ "int" ] != null) // param of type int32 ...
            {
                return Int32.Parse(val [ "int" ].InnerText);
            }
            else if (val [ "i4" ] != null) // param of type int32 (alternative) ...
            {
                return Int32.Parse(val [ "i4" ].InnerText);
            }
            else if (val [ "double" ] != null) // param of type double ...
            {
                return double.Parse(val [ "double" ].InnerText);
            }
            else if (val [ "boolean" ] != null) // param of type boolean ...
            {
                if (val [ "boolean" ].InnerText == "1")
                    return true;
                else
                    return false;
            }
            else if (val [ "struct" ] != null) // param of type struct ...
            {
                Hashtable structure = new Hashtable();
                foreach (XmlElement member in val [ "struct" ])
                {
                    // parse each member ...
                    structure.Add(member [ "name" ].InnerText, ParseXml(member));
                }
                return structure;
            }
            else if (val [ "array" ] != null) // param of type array ...
            {
                ArrayList array = new ArrayList();
                foreach (XmlElement data in val [ "array" ] [ "data" ])
                {
                    // parse each data field ...
                    array.Add(ParseXml(data));
                }
                return array;
            }
            else if (val [ "base64" ] != null) // param of type base64 ...
            {
                byte[] data = Convert.FromBase64String(val["base64"].InnerText);
                return data;
            }

            return null;
        }

    }
}
