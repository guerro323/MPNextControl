using System;
using System.Diagnostics;
using MPNextControl.Plugins;
using MPNextControl.Core;
using MPNextControl.Core.XMLRPC;
using MPNextControl.Core.XML_Rpc.Gbx;
using MPNextControl.Core.XMLRPC.Structures;
using MPNextControl.Services.Static;
using System.Linq;

using CB = MPNextControl.Services.Static.KnowCallbacks;
using MLWidget = MPNextControl.Services.Static.Manialink.Widget;
using System.IO;
using MPNextControl.Services.Maths;
using System.Collections.Generic;
using System.Configuration;

class MapInfo : Plugin
{
    override public string StaticName => "MapInfo";
    override public string Name => "Map Widget";
    override public string Author => "Guerro";
    override public double Version => 0.1;
    override public int VersionBuild => 0;

    public MLWidget Widget = new MLWidget();

    private void SendML(RPCServer server)
    {
        var result = File.ReadAllText("Plugins/MapInfo/Interface/LegacyWidget.xml")
            .Replace("@[MainFrame.PosX]", Widget.Position.X.ToString())
            .Replace("@[MainFrame.PosY]", Widget.Position.Y.ToString());
        ManialinkManager.ApplyStyleRule(ref result);
        /*result = $@"
<frame posn=""{Widget.Position.X} {Widget.Position.Y}"">
    <quad halign=""center"" valign=""center"" sizen=""60 20"" style=""Bgs1InRace"" substyle=""BgHealthBar""/>
    <quad halign=""center"" valign=""center"" sizen=""60 20"" style=""Bgs1InRace"" substyle=""BgHealthBar"" opacity=""0.25""/>
</frame>";*/
        ManialinkManager.Send(server, new object(), result, "MapInfo");
    }

    public override bool OnLoad()
    {
        Console.WriteLine($":D {ConfigurationManager.AppSettings.Count}");

        double x, y;
        x = double.Parse(Config.GetConfig(StaticName, PluginConfig.ConfigType.Config, "WidgetPosX", 110).ToString());
        y = double.Parse(Config.GetConfig(StaticName, PluginConfig.ConfigType.Config, "WidgetPosY", 90).ToString());
        Widget.Position = new Vector3(x, y, 0);
        Console.WriteLine(Widget.Position.X);
        foreach (var Server in Servers)
        {
            Server.Script.ChatSendServerMessage(HelperManager.FormatedString("Map", $"Playing on {GetCorrectName(Server.Service.GameInfo.GameName)} : $i{Server.Script.GetCurrentMapInfo().Name}", Helper.Level.Header));
            SendML(Server);
            Server.Remote.Request("GetVersion", new object[0]);
        }
        return true;
    }

    public void OnBeginMap(SMapInfo mapInfo)
    {
      Console.WriteLine("yessssssssss");
    }

    public void OnGbxResponse(RPCServer Server, int handle, GbxCall e)
    {
      Console.WriteLine("i'm keking right now!");
      Console.WriteLine(e.MethodName);
    }

    public override void OnGbxCallback(RPCServer Server, GbxRemote.GbxCallbackEventArgs Args)
    {
        if (Args.Response.MethodName == CB.ManiaPlanet.BeginMap)
        {
            Server.Script.ChatSendServerMessage(HelperManager.FormatedString("Map", $"Playing on {GetCorrectName(Server.Service.GameInfo.GameName)} : $i{Server.Script.GetCurrentMapInfo().Name}", Helper.Level.Header));
        }
    }

    public string GetCorrectName(string compressed)
    {
        if (compressed == "sm") return "map";
        else return "track";
    }
}
