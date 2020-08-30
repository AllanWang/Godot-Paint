using Godot;
using System;

public class Message : Panel
{
	private Label _text;
	public MessageData data;

	public override void _Ready()
	{
		_text = (Label) GetNode("Text");
		SetData();
		RectMinSize = new Vector2(100, 100);
	}

	public struct MessageData
	{
		public string Message;
	}

	private void SetData()
	{
		_text.Text = data.Message;
	}
}
