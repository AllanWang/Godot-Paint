using Godot;
using System;

public class EntrySelection : Control
{
	private Button _server;
	private Button _client;

	public override void _Ready()
	{
		var container = (Container) GetNode("VBoxContainer");

		_server = (Button) container.GetNode("Server");
		_client = (Button) container.GetNode("Client");

		_server.Connect("pressed", this, nameof(StartServer));
		_client.Connect("pressed", this, nameof(StartClient));
	}

	private void StartServer()
	{
		GD.Print("Starting server");
		GetTree().ChangeScene("res://server/ServerEntry.tscn");
	}

	private void StartClient()
	{
		GD.Print("Starting client");
		GetTree().ChangeScene("res://client/ClientEntry.tscn");
	}
}
