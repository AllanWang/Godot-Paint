using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Paint.Proto;

public class Lobby : Control
{
    private ClientNetwork _network;

    private LineEdit _name;
    private Button _host;
    private Button _join;
    private Button _cancel;
    private Container _userContainer;

    public override void _Ready()
    {
        _network = (ClientNetwork) GetNode("/root/ClientNetwork");

        _userContainer = (Container) GetNode("CenterContainer/HBoxContainer/UserContainer");

        var inputNode = GetNode("CenterContainer/HBoxContainer/InputContainer");
        _name = (LineEdit) inputNode.GetNode("Name");
        _host = (Button) inputNode.GetNode("Host");
        _join = (Button) inputNode.GetNode("Join");
        _cancel = (Button) inputNode.GetNode("Cancel");

        _host.Connect("pressed", this, nameof(RequestHost));
        _join.Connect("pressed", this, nameof(RequestJoin));
        _cancel.Connect("pressed", this, nameof(RequestCancel));

        _network.Connect(nameof(GameState.LobbyChanged), this, nameof(UpdatePlayerList));
        _network.Connect(nameof(GameState.GameEnded), this, nameof(GameEnded));
    }

    private void RequestHost()
    {
        _network.HostGame(new PlayerData
        {
            Name = _name.Text.OrFallback(RandomName)
        });
    }

    private void RequestJoin()
    {
        _network.JoinGame(new PlayerData
        {
            Name = _name.Text.OrFallback(RandomName)
        });
    }

    private void RequestCancel()
    {
        _network.EndGame();
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

        foreach (var player in _network.Lobby?.Players.Values ?? Enumerable.Empty<PlayerData>())
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