using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPNextControl.Services.Static
{
    /// <summary>
    ///  Provide some help services to a plugin
    ///  <para>( example : Creation of a menu, command, ... )</para>
    /// </summary>
    public class Helper
    {
        public enum Level
        {
            Header,
            Normal,
            Success,
            NoSuccess,
            Canceled,
            Private,
            Error,
            Warning
        }

        public string FormatedString(string beforePrefix, string text, Level level)
        {
            switch (level)
            {
                case Level.Header:
                    return $"$i$eee{beforePrefix} $ff0॥ $fff{text}";
                case Level.Normal:
                    return $"$999{beforePrefix} $29f〉 $fff{text}";
                case Level.Success:
                    return $"$fff{beforePrefix} $999》 $0c0{text}";
                case Level.Canceled:
                    return $"$fff{beforePrefix} $999》 $f70{text}";
                case Level.Error:
                    return $"$fff[$f70{beforePrefix}$fff] $ccc》 $f10{text}";
            }
            return text;
        }

        public string FormatedString(string text, Level level)
        {
            switch (level)
            {
                case Level.Header: return FormatedString("Info", text, level);
                case Level.Normal: return FormatedString("Server", text, level);
                case Level.Success: return FormatedString("Success!", text, level);
                case Level.Canceled: return FormatedString("Canceled", text, level);
                case Level.Error: return FormatedString("Error :(", text, level);
            }
            return text;
        }
    }
}
