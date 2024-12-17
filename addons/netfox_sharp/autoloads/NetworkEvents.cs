using Godot;

namespace Netfox;

public partial class NetworkEvents : Node
{
    #region Public Variables
    public bool Enabled
    {
        get { return (bool)_networkTimeGd.Get(PropertyNameGd.Enabled); }
        set { _networkTimeGd.Set(PropertyNameGd.Enabled, value); }
    }
    #endregion

    GodotObject _networkTimeGd;

    public NetworkEvents(GodotObject networkTimeGd)
    {
        _networkTimeGd = networkTimeGd;

        _networkTimeGd.Connect(SignalNameGd.OnMultiplayerChange, Callable.From((MultiplayerApi oldApi, MultiplayerApi newApi) => EmitSignal(SignalName.OnMultiplayerChange, oldApi, newApi)));
        _networkTimeGd.Connect(SignalNameGd.OnServerStart, Callable.From(() => EmitSignal(SignalName.OnServerStart)));
        _networkTimeGd.Connect(SignalNameGd.OnServerStop, Callable.From(() => EmitSignal(SignalName.OnServerStop)));
        _networkTimeGd.Connect(SignalNameGd.OnClientStart, Callable.From((long clientId) => EmitSignal(SignalName.OnClientStart, clientId)));
        _networkTimeGd.Connect(SignalNameGd.OnClientStop, Callable.From(() => EmitSignal(SignalName.OnClientStop)));
        _networkTimeGd.Connect(SignalNameGd.OnPeerJoin, Callable.From((long clientId) => EmitSignal(SignalName.OnPeerJoin, clientId)));
        _networkTimeGd.Connect(SignalNameGd.OnPeerLeave, Callable.From((long clientId) => EmitSignal(SignalName.OnPeerLeave, clientId)));
    }

    #region Signals
    [Signal]
    public delegate void OnMultiplayerChangeEventHandler(MultiplayerApi oldApi, MultiplayerApi newApi);
    [Signal]
    public delegate void OnServerStartEventHandler();
    [Signal]
    public delegate void OnServerStopEventHandler();
    [Signal]
    public delegate void OnClientStartEventHandler(long clientId);
    [Signal]
    public delegate void OnClientStopEventHandler();
    [Signal]
    public delegate void OnPeerJoinEventHandler(long peerId);
    [Signal]
    public delegate void OnPeerLeaveEventHandler(long peerId);
    #endregion

    #region Methods
    public bool IsServer() { return (bool)_networkTimeGd.Call(MethodNameGd.IsServer); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            IsServer = "is_server";
    }
    static class PropertyNameGd
    {
        public static readonly StringName
            Enabled = "enabled";
    }
    static class SignalNameGd
    {
        public static readonly StringName
            OnMultiplayerChange = "on_multiplayer_change",
            OnServerStart = "on_server_start",
            OnServerStop = "on_server_stop",
            OnClientStart = "on_client_start",
            OnClientStop = "on_client_stop",
            OnPeerJoin = "on_peer_join",
            OnPeerLeave = "on_peer_leave";
    }
    #endregion
}
