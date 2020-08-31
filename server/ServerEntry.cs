using Godot;
using System;

public class ServerEntry : Node
{
	public override void _Ready()
	{
		GetTree().ChangeScene("res://server/ServerNetwork.tscn");
	}
}
