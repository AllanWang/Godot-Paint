using Godot;
using System;

public class Message : PanelContainer
{
	private Label _text;
	public MessageData data;

	public override void _Ready()
	{
		_text = (Label) GetNode("Margins/Container/Text");
		SetData();
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
