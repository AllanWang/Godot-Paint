#nullable enable
using Godot;
using System;
using System.Collections.Generic;
using Google.Protobuf;
using Paint.Proto;

public class ServerNetwork : GameState
{
    private WebSocketServer? NetworkPeer
    {
        get => GetTree().NetworkPeer as WebSocketServer;
        set
        {
            GetTree().NetworkPeer = value;
            SetProcess(value != null);
        }
    }

    private ISet<int> _playersReady = new HashSet<int>();

    [Signal]
    public delegate void NetworkConnected();

    [Signal]
    public delegate void NetworkDisconnected();

    [Signal]
    public delegate void LobbyUpdate(byte[] lobbyBytes);

    public override void _Ready()
    {
        base._Ready();
        GetTree().Connect("network_peer_connected", this, nameof(_PeerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_PeerDisconnected));
        GD.Print("ServerNetwork Ready");

        Connect();
    }

    protected override int ClientId()
    {
        return GetTree().GetRpcSenderId();
    }

    private void _PeerConnected(int id)
    {
        GD.Print($"Welcome {id}");
    }

    private void _PeerDisconnected(int id)
    {
        GD.Print($"Goodbye {id}");
    }

    [Remote]
    public void RequestHostGame(byte[] playerBytes)
    {
        GD.Print("RequestHostGame");
        var player = playerBytes.ToPlayerData();
        var id = GetTree().GetRpcSenderId();
        // TODO check lobby not empty; in the future we will have unique keys
        Lobby = new LobbyData
        {
            Host = id
        };
        Lobby.Players[id] = player;
        EmitSignal(nameof(LobbyUpdate), Lobby.ToByteArray());
        RpcId(id, nameof(ClientNetwork.SetLobbyRemote), Lobby.ToByteArray());
        GD.Print($"Return {id} {Lobby}");
    }

    [Remote]
    public void RequestJoinGame(byte[] playerBytes)
    {
        var player = playerBytes.ToPlayerData();
        var id = GetTree().GetRpcSenderId();

        if (Lobby == null)
        {
            RpcId(id, nameof(ClientNetwork.LobbyError), "No host found");
            return;
        }

        Lobby.Players[id] = player;
        EmitSignal(nameof(LobbyUpdate), Lobby.ToByteArray());        Rpc(nameof(ClientNetwork.SetLobbyRemote), Lobby.ToByteArray());
    }

    [Remote]
    public void RequestStartGame()
    {
        if (!IsCallerHost())
        {
            GD.Print("Start request sent by non host");
            return;
        }

        _playersReady.Clear();
        // TODO check if we want to send lobby again for pre start
        // and have client return back hash code for verification?
        Rpc(nameof(ClientNetwork.PreStartGame));
    }

    [Remote]
    public void StartGameReady()
    {
        if (Lobby == null) return;
        var id = GetTree().GetRpcSenderId();
        _playersReady.Add(id);
        if (_playersReady.SetEquals(Lobby.Players.Keys))
        {
            Rpc(nameof(ClientNetwork.PostStartGame));
        }
    }

    public void Connect()
    {
        var server = new WebSocketServer();
        var error = server.Listen(DefaultPort, gdMpApi: true);
        if (error != Error.Ok)
        {
            GD.PrintErr("Server connection error");
            GetTree().Quit();
            return;
        }

        NetworkPeer = server;
        GD.Print("Started Host Server");
        EmitSignal(nameof(NetworkConnected));
    }

    public void Disconnect()
    {
        if (NetworkPeer == null) return;
        GD.Print("Disconnecting server");
        EmitSignal(nameof(NetworkDisconnected));
        EndGame();
        NetworkPeer = null;
        SetProcess(false);
    }


    public override void EndGame()
    {
        base.EndGame();
        Rpc(nameof(ClientNetwork.EndGameRemote));
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (NetworkPeer == null) return;
        if (NetworkPeer.IsListening())
        {
            NetworkPeer.Poll();
        }
    }
}