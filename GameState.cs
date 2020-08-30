using Godot;
using System;
using System.Collections.Generic;
using Google.Protobuf;

public class GameState : Node
{
    private readonly int _defaultPort = 8080;

    public PlayerData Player = new PlayerData
    {
        Name = "Test"
    };

    public readonly Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();

    private ISet<int> _playersReady = new HashSet<int>();

    private PackedScene _paintLoader;
    private PackedScene _lobbyLoader;

    [Signal]
    public delegate void PlayerListChanged();

    [Signal]
    public delegate void ConnectionSucceeded();

    [Signal]
    public delegate void ConnectionFailed();

    [Signal]
    public delegate void GameEnded();

    public override void _Ready()
    {
        _paintLoader = GD.Load<PackedScene>("res://paint_root.tscn");
        _lobbyLoader = GD.Load<PackedScene>("res://lobby.tscn");

        GetTree().Connect("network_peer_connected", this, nameof(_PlayerConnected));
        GetTree().Connect("network_peer_disconnected", this, nameof(_PlayerDisconnected));
        GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
        GetTree().Connect("connection_failed", this, nameof(_ConnectionFailed));
        GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
    }

    public void HostGame()
    {
        var peer = new NetworkedMultiplayerENet();
        peer.CreateServer(_defaultPort, maxClients: 8);
        GetTree().NetworkPeer = peer;

        GD.Print("You are now hosting.");

        Players[GetTree().GetNetworkUniqueId()] = Player;
        EmitSignal(nameof(PlayerListChanged));
    }

    public void JoinGame()
    {
        var address = "localhost";

        GD.Print($"Joining game with address {address}");

        var clientPeer = new NetworkedMultiplayerENet();
        clientPeer.CreateClient(address, _defaultPort);

        GetTree().NetworkPeer = clientPeer;
    }

    private void _LeaveGame()
    {
        GD.Print("Leaving current game");

        Rpc(nameof(RemovePlayer), GetTree().GetNetworkUniqueId());

        EndGame();
    }

    private void _PlayerConnected(int id)
    {
        // TODO set player name

        GD.Print($"Welcome {Player.Name}");

        RpcId(id, nameof(RegisterPlayer), Player.ToByteArray());
    }

    private void _PlayerDisconnected(int id)
    {
        GD.Print("Player disconnected");

        RemovePlayer(id);
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

    [Remote]
    private void RegisterPlayer(byte[] playerBytes)
    {
        PlayerData player = PlayerData.Parser.ParseFrom(playerBytes);
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
    private void RemovePlayer(int id)
    {
        if (!Players.ContainsKey(id)) return;
        Players.Remove(id);
        EmitSignal(nameof(PlayerListChanged));
    }

    public void EndGame()
    {
        if (GetTree().NetworkPeer != null)
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
}