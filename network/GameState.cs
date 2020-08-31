using Godot;
using System;
using System.Collections.Generic;
using Google.Protobuf;
using Paint.Proto;


public class GameState : Node
{
    public readonly int DefaultPort = 8080;

    // public PlayerData Player = new PlayerData();

    public readonly Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();

    public LobbyData? Lobby = null;

    [Signal]
    public delegate void PlayerListChanged();

    [Signal]
    public delegate void GameEnded();

    public override void _Ready()
    {
    }

    // private void _PlayerConnected(int id)
    // {
    //     // TODO set player name
    //
    //     GD.Print($"Welcome {Player.Name}");
    //
    //     RpcId(id, nameof(RegisterPlayer), Player.ToByteArray());
    // }

    private void _PlayerDisconnected(int id)
    {
        GD.Print("Player disconnected");

        RemovePlayer(id);
    }

    [Remote]
    public void RegisterPlayer(byte[] playerBytes)
    {
        PlayerData player = playerBytes.ToPlayerData();
        var id = GetTree().GetRpcSenderId();

        Players[id] = player;

        EmitSignal(nameof(PlayerListChanged));

        GD.Print($"Add {id}: {player}");
    }

    public void RequestStartGame()
    {
        // Trace.Assert(GetTree().IsNetworkServer());

        foreach (var playerId in Players.Keys)
        {
            RpcId(playerId, nameof(PreStartGame));
        }

        PreStartGame();
    }

    [Remote]
    private void PreStartGame()
    {
        // if (HasNode("/root/PaintRoot")) return; // TODO

        GetNode("/root/Lobby").QueueFree();
        var paintRoot = (PaintRoot) _paintLoader.Instance();
        GetTree().Root.AddChild(paintRoot);

        _playersReady.Clear();

        if (!GetTree().IsNetworkServer())
        {
            RpcId(1, nameof(ReadyToStart), GetTree().GetNetworkUniqueId());
        }
        else if (Players.Count == 0)
        {
            // Start if we are the only person
            // Debug only?
            PostStartGame();
        }
    }

    [Remote]
    private void ReadyToStart(int id)
    {
        // assert server
        _playersReady.Add(id);
        if (!_playersReady.SetEquals(Players.Keys)) return;
        foreach (var playerId in _playersReady)
        {
            RpcId(playerId, nameof(PostStartGame));
        }

        PostStartGame();
    }

    [Remote]
    private void PostStartGame()
    {
        GetTree().Paused = false;
    }


    [Remote]
    public void RemovePlayer(int id)
    {
        if (Lobby?.Players.ContainsKey(id) != true) return;
        Lobby.Players.Remove(id);
        EmitSignal(nameof(PlayerListChanged));
    }

    // public void EndGame()
    // {
    //     if (GetTree().NetworkPeer != null)
    //     {
    //         GD.Print("Disconnected from network");
    //         ((NetworkedMultiplayerENet) GetTree().NetworkPeer).CloseConnection();
    //         GetTree().NetworkPeer = null;
    //     }
    //
    //     if (HasNode("/root/PaintRoot"))
    //     {
    //         GetNode("/root/PaintRoot").QueueFree();
    //     }
    //
    //     Players.Clear();
    //     EmitSignal(nameof(GameEnded));
    // }
}