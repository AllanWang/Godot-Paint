using Godot;
using System;

public class Entry : Node
{
	public override void _Ready()
	{
		GD.Print("Start GD Paint");

		if (OS.HasFeature("paint_server"))
		{
			StartServer();
		}
		else if (OS.HasFeature("paint_client"))
		{
			StartClient();
		}
		else
		{
			GD.Print("Missing feature flag");
			// StartClient();
			StartServer();
		}
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
