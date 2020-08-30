using Godot;
using System;

public class Lobby : Control
{
	private GameState _gameState;

	private LineEdit _name;
	private Button _host;
	private Button _join;
	private Button _cancel;
	private Container _userContainer;

	public override void _Ready()
	{
		_gameState = (GameState) GetNode("/root/GameState");

		_userContainer = (Container) GetNode("CenterContainer/HBoxContainer/UserContainer");

		var inputNode = GetNode("CenterContainer/HBoxContainer/InputContainer");
		_name = (LineEdit) inputNode.GetNode("Name");
		_host = (Button) inputNode.GetNode("Host");
		_join = (Button) inputNode.GetNode("Join");
		_cancel = (Button) inputNode.GetNode("Cancel");

		_host.Connect("pressed", this, nameof(RequestHost));
		_join.Connect("pressed", this, nameof(RequestJoin));
		_cancel.Connect("pressed", this, nameof(RequestCancel));

		_gameState.Connect(nameof(GameState.PlayerListChanged), this, nameof(UpdatePlayerList));
		_gameState.Connect(nameof(GameState.GameEnded), this, nameof(GameEnded));
	}

	private void RequestHost()
	{
		_gameState.Player.Name = _name.Text.OrFallback(RandomName);
		_gameState.HostGame();
	}

	private void RequestJoin()
	{
		_gameState.Player.Name = _name.Text.OrFallback(RandomName);
		_gameState.JoinGame();
	}

	private void RequestCancel()
	{
		_gameState.EndGame();
	}

	private string RandomName()
	{
		return "Test";
	}

	private void UpdatePlayerList()
	{
		foreach (Node child in _userContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var player in _gameState.Players.Values)
		{
			var playerNode = new Label {Text = player.Name};
			_userContainer.AddChild(playerNode);
		}
	}

	private void GameEnded()
	{
		foreach (Node child in _userContainer.GetChildren())
		{
			child.QueueFree();
		}
	}
}
