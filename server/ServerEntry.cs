using Godot;
using System;

public class ServerEntry : Node
{
	public override void _Ready()
	{
		var network = new ServerNetwork
		{
			Name = "ServerNetwork"
		};
		GetTree().Root.AddChild(network);
		GetTree().ChangeScene("res://server/ServerNetwork.tscn");
	}
}
