using System;
using System.Diagnostics;
using System.Windows.Forms;
using MPNextControl.Plugins;
using System.Threading;
using System.Threading.Tasks;
using MPNextControl.Core.XML_Rpc.Gbx;
using MPNextControl.Core.XMLRPC;
using MPNextControl.Core.XMLRPC.Structures;
using System.Collections;

class Script : Plugin
{
	RPCServer RPCServer;
	GbxRemote Remote;

	public delegate void HandleOnBeginMap(SMapInfo newMapInfo);
	public event HandleOnBeginMap OnBeginMap;

	public string StaticName
	{
		get { return "Handle"; }
	}

	public bool OnLoad()
	{
		return true;
	}

	public void SetRPCServer(RPCServer server)
	{
		RPCServer = server;
		Remote = server.Remote;
	}

	public string GetLogin()
	{
		var task = AsyncGetLogin();
		task.Wait();
		Console.WriteLine(task.Result);
		return task.Result;
	}

	public async Task<string> AsyncGetLogin()
	{
		Console.WriteLine(Remote == null);
        GbxCall request = await Remote.AsyncRequest("GetSystemInfo", new object[] {}, true);
        GbxCall done = Remote.GetResponse(request.Handle);
        Hashtable ht = (Hashtable) done.Params[0];
        return ((string) ht["ServerLogin"]);
	}

	public MPNextControl.Services.Static.GameInfo GetGameInfo()
	{
		var task = AsyncGetGameInfo();
		task.Wait();
		Console.WriteLine(task.Result.GameName);
		return task.Result;
	}

	public async Task<MPNextControl.Services.Static.GameInfo> AsyncGetGameInfo()
	{
		GbxCall request = await Remote.AsyncRequest("GetCurrentMapInfo", new object[] {}, true);

		GbxCall response = Remote.GetResponse(request.Handle);

		var cmi = new MPNextControl.Services.Static.GameInfo();
		Hashtable ht = (Hashtable) response.Params[0];

		cmi.GameName = (string) ht["Environnement"];
		var enviro = cmi.GameName;
		if (enviro == "Valley" || enviro == "Stadium" || enviro == "Lagoon" || enviro == "Canyon" || enviro == "Avalanche")
		{
			cmi.GameName = "tm";
		}
		else if (enviro == "Storm" || enviro == "Cryo" || enviro == "Meteor")
		{
			cmi.GameName = "sm";
		}
		return cmi;
}

	public CurrentMapInfo GetCurrentMapInfo()
	{
		var task = AsyncGetCurrentMapInfo();
		task.Wait();
		return task.Result;
	}

	public async Task<CurrentMapInfo> AsyncGetCurrentMapInfo()
	{
			GbxCall request = await Remote.AsyncRequest("GetCurrentMapInfo", new object[] {}, true);

			GbxCall response = Remote.GetResponse(request.Handle);

			if (response.Params.Count == 1 &&
					response.Params[0].GetType() == typeof (Hashtable))
			{
					Hashtable ht = (Hashtable) response.Params[0];
					CurrentMapInfo cmi = new CurrentMapInfo();
					cmi.Author = (string) ht["Author"];
					cmi.Environnement = (string) ht["Environnement"];
					cmi.FileName = (string) ht["FileName"];
					cmi.MapStyle = (string) ht["MapStyle"];
					cmi.Name = (string) ht["Name"];
					cmi.UId = (string) ht["UId"];
					cmi.GoldTime = (int) ht["GoldTime"];
					cmi.CopperPrice = (int) ht["CopperPrice"];
					cmi.NbCheckpoints = (int) ht["NbCheckpoints"];
					cmi.Null = false;
					return cmi;
			}

			return new CurrentMapInfo() {Null = true};

	}


	public void ChatSend(string Text)
	{
		Remote.Request("ChatSend", new object[] { Text });
	}

	public void ChatSendServerMessage(string Text)
	{
		Remote.Request("ChatSendServerMessage", new object[] { Text });
	}

	/// <summary>
	///	Send a manialink to the client
	/// </summary>
	public void SendManialink(object playerLogin, string maniaLink, int timeOut = 0, bool hideWhenClicked = false)
	{
		if (playerLogin.GetType() == typeof(string) || playerLogin.GetType() == typeof(string[]))
		{
			Remote.Request("SendDisplayManialinkPageToLogin", new object[] { playerLogin, maniaLink, timeOut, hideWhenClicked });
		}
		if (playerLogin.GetType() == typeof(int))
		{
			Remote.Request("SendDisplayManialinkPageToId", new object[] { playerLogin, maniaLink, timeOut, hideWhenClicked });
		}
		else
			Remote.Request("SendDisplayManialinkPage", new object[] { maniaLink, timeOut, hideWhenClicked });
	}
}
