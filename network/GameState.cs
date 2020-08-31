#nullable enable
using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Paint.Proto;


public abstract class GameState : Node
{
    public readonly int DefaultPort = 8080;

    public LobbyData? Lobby;

    [Signal]
    public delegate void LobbyChanged();

    [Signal]
    public delegate void GameEnded();

    protected abstract int ClientId();

    protected bool IsCallerHost()
    {
        return ClientId() == Lobby?.Host;
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
        if (Lobby == null)
        {
            GD.Print("Attempted to register player in invalid lobby");
            return;
        }

        PlayerData player = playerBytes.ToPlayerData();
        var id = GetTree().GetRpcSenderId();

        Lobby.Players[id] = player;

        EmitSignal(nameof(LobbyChanged));

        GD.Print($"Add {id}: {player}");
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
        EmitSignal(nameof(LobbyChanged));
    }

    public virtual void EndGame()
    {
        Lobby = null;
    }
}