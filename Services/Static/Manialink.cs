using MPNextControl.Core.XMLRPC;
using MPNextControl.Services.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPNextControl.Services.Static
{
    public class Manialink
    {

        public Dictionary<StyleRule, string> styleRules = new Dictionary<StyleRule, string>();

        public enum StyleRule
        {
            BackGroundQuad,
            BackGroundLabel,
            BackGroundBigLabel,
            BackGroundBigQuad,
            QuadTitle,
            LabelTitle,
            HeadLabel,
            HeadQuad,
            ColumnLabel,
            ColumnQuad,
            ColumnTitleLabel,
            ColumnTitleQuad,
            EndOfQuad
        }

        public void Send(RPCServer server, object playerName, string manialinkContents, string manialinkName, int timeOut = 0, bool hideWhenClicked = false)
        {
            string buildedManialink = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?><manialink version=\"2\" id=\"NC::" + manialinkName + "\">" + manialinkContents + "</manialink>";
            if (server != null)
            {
                server.Script.SendManialink(playerName, buildedManialink, timeOut, hideWhenClicked);
                return;
            }
            foreach (var aserver in StaticPower.CurrentCore.Servers.Where(s => s.Remote.tcpSocket.Connected))
            {
                aserver.Script.SendManialink(playerName, buildedManialink, timeOut, hideWhenClicked);
            }
            return;
        }

        /// <summary>
        /// For the creation of new widget
        /// </summary>
        public class Widget
        {
            public Vector3 Position = new Vector3();
            public string Name;
            public string Contents;
            public Dictionary<string, object> Types;
        }

        public void SetStyleRule(StyleRule style, string rule) => styleRules [ style ] = rule;

        public string GetStyleRule(StyleRule style)
        {
            var rule = "";
            styleRules.TryGetValue(style, out rule);
            return rule;
        }

        public void ApplyStyleRule(ref string ml)
        {
            ml = ml
                .Replace(@"globalstyle=""BackGroundQuad""", GetStyleRule(StyleRule.BackGroundQuad))
                .Replace(@"globalstyle=""BackGroundLabel""", GetStyleRule(StyleRule.BackGroundLabel))
                .Replace(@"globalstyle=""BackGroundBigQuad""", GetStyleRule(StyleRule.BackGroundBigQuad))
                .Replace(@"globalstyle=""BackGroundBigLabel""", GetStyleRule(StyleRule.BackGroundBigLabel))
                .Replace(@"globalstyle=""QuadTitle""", GetStyleRule(StyleRule.QuadTitle))
                .Replace(@"globalstyle=""LabelTitle""", GetStyleRule(StyleRule.LabelTitle))
                .Replace(@"globalstyle=""HeadQuad""", GetStyleRule(StyleRule.HeadQuad))
                .Replace(@"globalstyle=""HeadLabel""", GetStyleRule(StyleRule.HeadLabel))
                .Replace(@"globalstyle=""ColumnTitleQuad""", GetStyleRule(StyleRule.ColumnTitleQuad))
                .Replace(@"globalstyle=""ColumnTitleLabel""", GetStyleRule(StyleRule.ColumnTitleLabel))
                .Replace(@"globalstyle=""ColumnQuad""", GetStyleRule(StyleRule.ColumnQuad))
                .Replace(@"globalstyle=""ColumnLabel""", GetStyleRule(StyleRule.ColumnLabel))
                .Replace(@"globalstyle=""EndOfQuad""", GetStyleRule(StyleRule.EndOfQuad));
        }

        public void SetDefaultStyleRule()
        {
            SetStyleRule(StyleRule.BackGroundQuad, @" bgcolor=""000"" opacity=""0.75"" ");
            SetStyleRule(StyleRule.BackGroundLabel, @" textcolor=""fff"" opacity=""1"" textemboss=""1"" ");
        }
    }
}
