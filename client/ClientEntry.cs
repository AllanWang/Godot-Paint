using Godot;
using System;

public class ClientEntry : Node
{
	public override void _Ready()
	{
		var network = new ClientNetwork
		{
			Name = "PaintNetwork"
		};
		GetTree().Root.AddChild(network);
		GetTree().ChangeScene("res://client/lobby.tscn");
	}
}
