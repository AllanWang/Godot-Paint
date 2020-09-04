#nullable enable
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

    private readonly bool _useNetwork = OS.HasFeature("paint_network");

    public PlayerData PlayerData = new PlayerData();

    private PackedScene _paintLoader = null!;
    private PackedScene _lobbyLoader = null!;

    string Address = "127.0.0.1";

    [Signal]
    public delegate void ConnectionSucceeded();

    [Signal]
    public delegate void ConnectionFailed();

    [Signal]
    public delegate void LobbyError(string error);

    public override void _Ready()
    {
        base._Ready();

        _paintLoader = GD.Load<PackedScene>("res://client/PaintRoot.tscn");
        _lobbyLoader = GD.Load<PackedScene>("res://client/Lobby.tscn");

        GetTree().Connect("network_peer_connected", this, nameof(_PeerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_PeerDisconnected));
        GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
        GetTree().Connect("connection_failed", this, nameof(_ConnectionFailed));
        GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));

        if (_useNetwork)
        {
            Connect();
        }
    }

    private bool Connect()
    {
        if (NetworkPeer != null) return true;
        var client = new WebSocketClient();
        var error = client.ConnectToUrl($"ws://{Address}:{DefaultPort}", gdMpApi: true);
        if (error != Error.Ok)
        {
            GD.PrintErr("Client connection error");
            return false;
        }

        NetworkPeer = client;
        GD.Print("Connected client to network");
        return true;
    }

    private void Disconnect()
    {
        if (NetworkPeer == null) return;
        GD.Print("Disconnected from network");
        ((NetworkedMultiplayerENet) GetTree().NetworkPeer).CloseConnection();
        GetTree().NetworkPeer = null;
    }

    /**
     * To facilitate testing, we will ignore rpc calls when the peer is disconnected.
     */
    private void RpcServer(string method, params object[] args)
    {
        if (NetworkPeer == null) return;
        GD.Print($"RpcServer {method}");
        RpcId(1, method, args);
    }

    protected override int ClientId()
    {
        return NetworkPeer != null ? GetTree().GetNetworkUniqueId() : -1;
    }

    public void HostGame(PlayerData data)
    {
        GD.Print("Request host game");
        PlayerData = data;
        RpcServer(nameof(ServerNetwork.RequestHostGame), data.ToByteArray());
    }

    public void JoinGame(PlayerData data)
    {
        if (!Connect()) return;
        GD.Print("Request join game");
        PlayerData = data;
        RpcServer(nameof(ServerNetwork.RequestJoinGame), data.ToByteArray());
    }

    public void StartGame()
    {
        if (!IsCallerHost())
        {
            GD.Print("Can't start game when caller is not host");
            return;
        }

        RpcServer(nameof(ServerNetwork.RequestStartGame));
    }

    [Remote]
    public void SetLobbyRemote(byte[] lobbyBytes)
    {
        Lobby = lobbyBytes.ToLobbyData();
        GD.Print($"Set lobby {Lobby}");
        EmitSignal(nameof(LobbyChanged));
    }

    [Remote]
    public void LobbyErrorRemote(string error)
    {
        GD.Print($"Lobby error {error}");
        EmitSignal(nameof(LobbyError), error);
    }

    [Remote]
    public void PreStartGame()
    {
        // if (HasNode("/root/PaintRoot")) return; // TODO

        GetNode("/root/Lobby").QueueFree();
        var paintRoot = (PaintRoot) _paintLoader.Instance();
        GetTree().Root.AddChild(paintRoot);

        RpcServer(nameof(ServerNetwork.StartGameReady));
    }

    [Remote]
    public void PostStartGame()
    {
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


    public override void EndGame()
    {
        Disconnect();

        if (HasNode("/root/PaintRoot"))
        {
            GetNode("/root/PaintRoot").QueueFree();
        }

        EmitSignal(nameof(GameEnded));
    }

    [Remote]
    public void EndGameRemote()
    {
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (NetworkPeer == null) return;
        if (NetworkPeer.GetConnectionStatus() == NetworkedMultiplayerPeer.ConnectionStatus.Connected ||
            NetworkPeer.GetConnectionStatus() == NetworkedMultiplayerPeer.ConnectionStatus.Connecting)
        {
            NetworkPeer.Poll();
        }
    }
}