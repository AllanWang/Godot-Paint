using Godot;
using System;

public class ServerEntry : Node
{
	public override void _Ready()
	{
		var network = new ServerNetwork
		{
			Name = "PaintNetwork"
		};
		GetTree().Root.AddChild(network);
		// GetTree().ChangeScene("res://server/ServerUI.tscn");
	}
}
