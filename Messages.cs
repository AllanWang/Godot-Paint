using Godot;
using System;

public class Messages : ScrollContainer
{
	private PackedScene _messageLoader;
	private VBoxContainer _messageList;

	public override void _Ready()
	{
		_messageLoader = GD.Load<PackedScene>("res://message.tscn");
		_messageList = (VBoxContainer) GetNode("MessageList");
		for (var i = 0; i < 100; i++)
			AddItem(new Message.MessageData {Message = "This is an intro message"});
		// Console.WriteLine($"Children {_messageList.GetChildCount()}");
	}

	private void AddItem(Message.MessageData data)
	{
		var message = (Message) _messageLoader.Instance();
		message.data = data;
		_messageList.AddChild(message);
	}
}
