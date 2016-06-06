using System;
using System.Diagnostics;
using MPNextControl.Plugins;
using MPNextControl.Core;
using MPNextControl.Core.XMLRPC;
using MPNextControl.Core.XML_Rpc.Gbx;
using MPNextControl.Services.Static;
using System.Linq;

using CB = MPNextControl.Services.Static.KnowCallbacks;
using MLWidget = MPNextControl.Services.Static.Manialink.Widget;
using System.IO;
using MPNextControl.Services.Maths;
using System.Collections.Generic;
using System.Configuration;

partial class kek : Plugin
{
    override public string StaticName => "ServerInfo";
    override public string Name => "Server Widget";
    override public string Author => "Guerro";
    override public double Version => 0.1;
    override public int VersionBuild => 0;

    public MLWidget Widget = new MLWidget();

    private void SendML(RPCServer server)
    {
        var result = File.ReadAllText("Plugins/ServerInfo/Interface/LegacyWidget.xml")
            .Replace("@[MainFrame.PosX]", Widget.Position.X.ToString())
            .Replace("@[MainFrame.PosY]", Widget.Position.Y.ToString());
        ManialinkManager.ApplyStyleRule(ref result);
        /*result = $@"
<frame posn=""{Widget.Position.X} {Widget.Position.Y}"">
    <quad halign=""center"" valign=""center"" sizen=""60 20"" style=""Bgs1InRace"" substyle=""BgHealthBar""/>
    <quad halign=""center"" valign=""center"" sizen=""60 20"" style=""Bgs1InRace"" substyle=""BgHealthBar"" opacity=""0.25""/>
</frame>";*/
        ManialinkManager.Send(server, new object(), result, "ServerInfo");
    }

    public override bool OnLoad()
    {
        Console.WriteLine($":D {ConfigurationManager.AppSettings.Count}");

        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("Scripted plugin!");
        Console.ResetColor();

        double x, y;
        x = double.Parse(Config.GetConfig(StaticName, PluginConfig.ConfigType.Config, "WidgetPosX", 110).ToString());
        y = double.Parse(Config.GetConfig(StaticName, PluginConfig.ConfigType.Config, "WidgetPosY", 110).ToString());
        Widget.Position = new Vector3(x, y, 0);
        foreach (var Server in Servers)
        {
            Server.Script.ChatSendServerMessage(HelperManager.FormatedString("Map", $"Playing on {GetCorrectName(Server.Service.GameInfo.GameName)} : $i{Server.Script.GetCurrentMapInfo().Name}", Helper.Level.Header));
            SendML(Server);
        }

        // add callback function
        //ServerManager.Callbacks(CB.ManiaPlanet.BeginMap) += OnBeginMap();

        return true;
    }

    public override void OnGbxCallback(RPCServer Server, GbxRemote.GbxCallbackEventArgs Args)
    {
        if (Args.Response.MethodName == CB.ManiaPlanet.BeginMap)
        {
            Server.Script.ChatSendServerMessage(
              HelperManager.FormatedString(
                "Map",
                $"Playing on {GetCorrectName(Server.Service.GameInfo.GameName)} : $i{Server.Script.GetCurrentMapInfo().Name}",
                Helper.Level.Header
              )
            );
        }
    }

    public string GetCorrectName(string compressed)
    {
        if (compressed == "sm")
          return "map";
        else
          return "track";
    }
}
