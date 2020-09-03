using Godot;
using System;

public class ServerUI : Control
{
	private ServerNetwork _network;
	private Button _start;
	private Button _stop;

	public override void _Ready()
	{
		_network = (ServerNetwork) GetNode("/root/ServerNetwork");

		var container = (Container) GetNode("CenterContainer/VBoxContainer");
		_start = (Button) container.GetNode("Start");
		_stop = (Button) container.GetNode("Stop");

		_start.Connect("pressed", this, nameof(StartNetwork));
		_stop.Connect("pressed", this, nameof(StopNetwork));
	}

	private void StartNetwork()
	{
		_network.Connect();
	}

	private void StopNetwork()
	{
		_network.Disconnect();
	}
}
