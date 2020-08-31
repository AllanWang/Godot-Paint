using Godot;
using System;
using System.Collections.Generic;
using Google.Protobuf;
using Paint.Proto;

public class ClientNetwork : GameState
{
    private WebSocketClient? NetworkPeer
    {
        get => GetTree().NetworkPeer as WebSocketClient;
        set
        {
            GetTree().NetworkPeer = value;
            SetProcess(value != null);
        }
    }

    public PlayerData PlayerData = new PlayerData();

    private PackedScene _paintLoader;
    private PackedScene _lobbyLoader;

    string Address = "127.0.0.1";

    [Signal]
    public delegate void ConnectionSucceeded();

    [Signal]
    public delegate void ConnectionFailed();

    [Signal]
    public delegate void SetLobby(LobbyData lobby);

    [Signal]
    public delegate void LobbyError(string error);

    public override void _Ready()
    {
        _paintLoader = GD.Load<PackedScene>("res://paint_root.tscn");
        _lobbyLoader = GD.Load<PackedScene>("res://lobby.tscn");

        GetTree().Connect("network_peer_connected", this, nameof(_PeerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_PeerDisconnected));
        GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
        GetTree().Connect("connection_failed", this, nameof(_ConnectionFailed));
        GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
    }

    private void Connect(PlayerData data)
    {
        var client = new WebSocketClient();
        var error = client.ConnectToUrl($"ws://{Address}:{DefaultPort}", gdMpApi: true);
        if (error != Error.Ok)
        {
            GD.PrintErr("Client connection error");
            return;
        }

        PlayerData = data;
        NetworkPeer = client;
        GD.Print("Connected client to network");
    }

    public void HostGame(PlayerData data)
    {
        Connect(data);
        RpcId(1, nameof(ServerNetwork.RequestHostGame), data.ToByteArray());
    }

    public void JoinGame(PlayerData data)
    {
        Connect(data);
        RpcId(1, "");
    }

    [Remote]
    public void SetLobbyRemote(byte[] lobbyBytes)
    {
        Lobby = lobbyBytes.ToLobbyData();
        EmitSignal(nameof(SetLobby), Lobby);
    }

    [Remote]
    public void LobbyErrorRemote(string error)
    {
        GD.Print($"Lobby error {error}");
        EmitSignal(nameof(LobbyError), error);
    }


    private void _PeerConnected(int id)
    {
        GD.Print($"Welcome {id}");
    }

    private void _PeerDisconnected(int id)
    {
        GD.Print($"Goodbye {id}");
    }

    private void ConnectedToServer()
    {
        GD.Print("Successfully connected to the server");
        EmitSignal(nameof(ConnectionSucceeded));
    }

    private void _ConnectionFailed()
    {
        GetTree().NetworkPeer = null;

        GD.Print("Failed to connect.");

        EmitSignal(nameof(ConnectionFailed));
    }

    private void ServerDisconnected()
    {
        GD.Print($"Disconnected from the server");

        _LeaveGame();
    }

    private void _LeaveGame()
    {
        GD.Print("Leaving current game");

        Rpc(nameof(RemovePlayer), GetTree().GetNetworkUniqueId());

        EndGame();
    }


    public void EndGame()
    {
        if (NetworkPeer != null)
        {
            GD.Print("Disconnected from network");
            ((NetworkedMultiplayerENet) GetTree().NetworkPeer).CloseConnection();
            GetTree().NetworkPeer = null;
        }

        if (HasNode("/root/PaintRoot"))
        {
            GetNode("/root/PaintRoot").QueueFree();
        }

        Players.Clear();
        EmitSignal(nameof(GameEnded));
    }

    [Remote]
    public void EndGameRemote()
    {
    }

    public override void _Process(float delta)
    {
        if (NetworkPeer == null) return;
        if (NetworkPeer.GetConnectionStatus() == NetworkedMultiplayerPeer.ConnectionStatus.Connected ||
            NetworkPeer.GetConnectionStatus() == NetworkedMultiplayerPeer.ConnectionStatus.Connecting)
        {
            NetworkPeer.Poll();
        }
    }
}