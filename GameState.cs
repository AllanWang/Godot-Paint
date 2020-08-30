using Godot;
using System;
using System.Collections.Generic;

public class GameState : Node
{
    private readonly int _defaultPort = 8080;

    public struct Player
    {
        public string Name;
    }

    private Player _player = new Player
    {
        Name = "Test"
    };

    private Dictionary<int, Player> _players = new Dictionary<int, Player>();

    private ISet<int> _playersReady = new HashSet<int>();

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
    }

    public void JoinGame()
    {
        var address = "localhost";

        GD.Print($"Joining game with address {address}");

        var clientPeer = new NetworkedMultiplayerENet();
        var result = clientPeer.CreateClient(address, _defaultPort);

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

        GD.Print($"Welcome {_player.Name}");

        RpcId(id, nameof(RegisterPlayer), _player.Name);
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
    private void RegisterPlayer(Player player)
    {
        var id = GetTree().GetRpcSenderId();

        _players.Add(id, player);

        GD.Print($"Add {id}: {player}");
    }

    public void RequestStartGame()
    {
        // Trace.Assert(GetTree().IsNetworkServer());

        foreach (var playerId in _players.Keys)
        {
            RpcId(playerId, nameof(PreStartGame));
        }

        PreStartGame();
    }

    [Remote]
    private void PreStartGame()
    {
        // if (HasNode("/root/PaintRoot")) return; // TODO

        var paintRoot = (PaintRoot) GD.Load<PackedScene>("res://paint_root.tscn").Instance();
        GetTree().Root.AddChild(paintRoot);

        _playersReady.Clear();

        if (!GetTree().IsNetworkServer())
        {
            RpcId(1, nameof(ReadyToStart), GetTree().GetNetworkUniqueId());
        }
        else if (_players.Count == 0)
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
        if (!_playersReady.SetEquals(_players.Keys)) return;
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
        if (!_players.ContainsKey(id)) return;
        _players.Remove(id);
        EmitSignal(nameof(PlayerListChanged));
    }

    private void EndGame()
    {
        if (GetTree().NetworkPeer != null)
        {
            ((NetworkedMultiplayerENet) GetTree().NetworkPeer).CloseConnection();
            GetTree().NetworkPeer = null;
        }

        if (HasNode("/root/PaintRoot"))
        {
            GetNode("/root/PaintRoot").QueueFree();
        }

        _players.Clear();
        EmitSignal(nameof(GameEnded));
    }
}