using Godot;
using System;

public class Entry : Node
{
	public override void _Ready()
	{
		GD.Print("Start GD Paint");

		if (OS.HasFeature("server"))
		{
			GD.Print("Starting server");
			GetTree().ChangeScene("res://server/ServerEntry.tscn");
		} else if (OS.HasFeature("client"))
		{
			GD.Print("Starting client");
			GetTree().ChangeScene("res://client/ClientEntry.tscn");
		}
		else
		{
			GD.Print("Missing feature flag");
			GetTree().ChangeScene("res://client/ClientEntry.tscn");
		}
	}

}
