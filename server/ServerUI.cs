using Godot;
using System;
using Paint.Proto;
using Object = System.Object;

public class ServerUI : Control
{
	private ServerNetwork _network;
	private Button _start;
	private Button _stop;
	private Container _logContainer;

	public override void _Ready()
	{
		_network = (ServerNetwork) GetNode("/root/PaintNetwork");

		var buttonContainer = (Container) GetNode("CenterContainer/HBoxContainer/VBoxContainer");
		_start = (Button) buttonContainer.GetNode("Start");
		_stop = (Button) buttonContainer.GetNode("Stop");

		_logContainer = (Container) GetNode("CenterContainer/HBoxContainer/ScrollContainer/VBoxContainer");

		_start.Connect("pressed", this, nameof(StartNetwork));
		_stop.Connect("pressed", this, nameof(StopNetwork));

		_network.Connect(nameof(ServerNetwork.NetworkConnected), this, nameof(LogMessageTag),
			new Godot.Collections.Array {"NetworkConnected"});
		_network.Connect(nameof(ServerNetwork.NetworkDisconnected), this, nameof(LogMessageTag),
			new Godot.Collections.Array {"NetworkDisconnected"});
		_network.Connect(nameof(ServerNetwork.LobbyUpdate), this, nameof(LogLobby));

		AddLogMessage("Test", "Message");
	}

	private void StartNetwork()
	{
		_network.Connect();
	}

	private void StopNetwork()
	{
		_network.Disconnect();
	}

	private void LogMessageTag(string tag)
	{
		AddLogMessage(tag);
	}

	private void LogLobby(byte[] lobbyBytes)
	{
		AddLogMessage("Lobby", lobbyBytes.ToLobbyData());
	}

	private void LogPlayerConnected(byte[] playerBytes)
	{
		AddLogMessage("Player connected", playerBytes.ToPlayerData());
	}


	private void LogPlayerDisconnected(byte[] playerBytes)
	{
		AddLogMessage("Player disconnected", playerBytes.ToPlayerData());
	}


	private void AddLogMessage(params object[] what)
	{
		var message = string.Join(" - ", Array.ConvertAll(what, Convert.ToString));

		GD.Print($"Log {what.Length}: {message}");

		var node = new Label
		{
			Text = message,
		};
		_logContainer.AddChild(node);
	}
}
