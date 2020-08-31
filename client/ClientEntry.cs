using Godot;
using System;

public class ClientEntry : Node
{
	public override void _Ready()
	{
		var network = new ClientNetwork
		{
			Name = "ClientNetwork"
		};
		GetTree().Root.AddChild(network);
		GetTree().ChangeScene("res://client/lobby.tscn");
	}
}
