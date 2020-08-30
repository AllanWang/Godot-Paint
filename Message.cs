using Godot;
using System;

public class Message : Panel
{

	private Label _text;

	public override void _Ready()
	{
		_text = (Label) GetNode("Text");
	}

	public struct MessageData
	{
		public string Message;
	}

	public void SetData(MessageData data)
	{
		_text.Text = data.Message;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
