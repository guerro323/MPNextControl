using System;
using System.Diagnostics;
using System.Windows.Forms;
using MPNextControl.Plugins;
using MPNextControl.Core;
using MPNextControl.Core.XMLRPC;
using MPNextControl.Core.XML_Rpc.Gbx;
using MPNextControl.Services.Static;

class MapInfo : Plugin
{
	public string StaticName = "MapInfo";
	public string Name = "Map Widget";
	public string Author = "Guerro";
	public double Version = 0.1;
	public int VersionBuild = 0;

	public bool OnLoad()
	{
		base.OnLoad();
		Console.WriteLine("kek");
		Servers[0].Do("ChatSendServerMessage", "$i$eeeMap $ff0॥ $fff Playing on " + GetCorrectName(Servers[0].Service.GameInfo.GameName) + " : ");
		Servers[0].Script.ChatSendServerMessage("kek");
		return true;
	}

	public void OnGbxCallBacks(RPCServer Server, GbxRemote.GbxCallbackEventArgs Args)
	{
		Console.WriteLine("> {Args.Response.MethodName}");
		if (Args.Response.MethodName == "ManiaPlanet.BeginMap")
		{
			Servers[0].Do("ChatSendServerMessage", "$i$eeeMap $ff0॥ $fff Playing on " + GetCorrectName(Servers[0].Service.GameInfo.GameName) + " : ");
		}
	}

	public string GetCorrectName(string compressed)
	{
		if (compressed == "sm") return "map";
		else return "track";
	}
}
