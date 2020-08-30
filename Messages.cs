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
		AddItem(new Message.MessageData {Message = "This is an intro message"});
	}

	private void AddItem(Message.MessageData data)
	{
		var message = (Message) _messageLoader.Instance();
		message.data = data;
		_messageList.AddChild(message);
	}
}
