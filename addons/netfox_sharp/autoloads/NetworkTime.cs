using Godot;

namespace Netfox;

public partial class NetworkTime : Node
{
    #region Public Variables
    public long TickRate { get { return (long)_networkTimeGd.Get(PropertyNameGd.TickRate); } }
    public bool SyncToPhysics { get { return (bool)_networkTimeGd.Get(PropertyNameGd.SyncToPhysics); } }
    public long MaxTicksPerFrame { get { return (long)_networkTimeGd.Get(PropertyNameGd.MaxTicksPerFrame); } }
    public double Time { get { return (double)_networkTimeGd.Get(PropertyNameGd.Time); } }
    public long Tick { get { return (long)_networkTimeGd.Get(PropertyNameGd.Tick); } }
    public double RecalibrateThreshold { get { return (long)_networkTimeGd.Get(PropertyNameGd.RecalibrateThreshold); } }
    public long RemoteTick { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteTick); } }
    public double RemoteTime { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteTime); } }
    public double RemoteRtt { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteRtt); } }
    public long LocalTick { get { return (long)_networkTimeGd.Get(PropertyNameGd.LocalTick); } }
    public double LocalTime { get { return (long)_networkTimeGd.Get(PropertyNameGd.LocalTime); } }
    public double TickTime { get { return (long)_networkTimeGd.Get(PropertyNameGd.TickTime); } }
    public double TickFactor { get { return (long)_networkTimeGd.Get(PropertyNameGd.TickFactor); } }
    public double PhysicsFactor { get { return (long)_networkTimeGd.Get(PropertyNameGd.PhysicsFactor); } }
    public double ClockStretchMax { get { return (long)_networkTimeGd.Get(PropertyNameGd.ClockStretchMax); } }
    public double ClockStretchFactor { get { return (long)_networkTimeGd.Get(PropertyNameGd.ClockStretchFactor); } }
    public double ClockOffset { get { return (long)_networkTimeGd.Get(PropertyNameGd.ClockOffset); } }
    public double RemoteClockOffset { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteClockOffset); } }
    #endregion

    GodotObject _networkTimeGd;

    public NetworkTime(GodotObject networkTimeGd)
    {
        _networkTimeGd = networkTimeGd;

        _networkTimeGd.Connect(SignalNameGd.BeforeTickLoop, Callable.From(() => EmitSignal(SignalName.BeforeTickLoop)));
        _networkTimeGd.Connect(SignalNameGd.BeforeTick, Callable.From((double delta, long tick) => EmitSignal(SignalName.BeforeTick, delta, tick)));
        _networkTimeGd.Connect(SignalNameGd.OnTick, Callable.From((double delta, long tick) => EmitSignal(SignalName.OnTick, delta, tick)));
        _networkTimeGd.Connect(SignalNameGd.AfterTick, Callable.From((double delta, long tick) => EmitSignal(SignalName.AfterTick, delta, tick)));
        _networkTimeGd.Connect(SignalNameGd.AfterTickLoop, Callable.From(() => EmitSignal(SignalName.AfterTickLoop)));
        _networkTimeGd.Connect(SignalNameGd.AftereSync, Callable.From(() => EmitSignal(SignalName.AfterSync)));
        _networkTimeGd.Connect(SignalNameGd.AfterClientSync, Callable.From((long peerId) => EmitSignal(SignalName.AfterClientSync, peerId)));
    }

    #region Signals
    [Signal]
    public delegate void BeforeTickLoopEventHandler();
    [Signal]
    public delegate void BeforeTickEventHandler(double delta, long tick);
    [Signal]
    public delegate void OnTickEventHandler(double delta, long tick);
    [Signal]
    public delegate void AfterTickEventHandler(double delta, long tick);
    [Signal]
    public delegate void AfterTickLoopEventHandler();
    [Signal]
    public delegate void AfterSyncEventHandler();
    [Signal]
    public delegate void AfterClientSyncEventHandler(long peerId);
    #endregion

    #region Methods
    public void Start() { _networkTimeGd.Call(MethodNameGd.Start); }
    public void Stop() { _networkTimeGd.Call(MethodNameGd.Stop); }
    public bool IsInitialSyncDone() { return (bool)_networkTimeGd.Call(MethodNameGd.IsInitialSyncDone); }
    public bool IsClientSynced(long peerId) { return (bool)_networkTimeGd.Call(MethodNameGd.IsClientSynced, peerId); }
    public double TicksToSeconds(long ticks) { return (double)_networkTimeGd.Call(MethodNameGd.TicksToSeconds, ticks); }
    public long SecondsToTicks(double seconds) { return (long)_networkTimeGd.Call(MethodNameGd.SecondsToTicks, seconds); }
    public double SecondsBetween(long fromTick, long toTick) { return (double)_networkTimeGd.Call(MethodNameGd.SecondsBetween, fromTick, toTick); }
    public long TicksBetween(double fromSecond, double toSecond) { return (long)_networkTimeGd.Call(MethodNameGd.TicksBetween, fromSecond, toSecond); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            Start = "start",
            Stop = "stop",
            IsInitialSyncDone = "is_initial_sync_done",
            IsClientSynced = "is_client_synced",
            TicksToSeconds = "ticks_to_seconds",
            SecondsToTicks = "seconds_to_ticks",
            SecondsBetween = "seconds_between",
            TicksBetween = "ticks_between";

    }
    static class PropertyNameGd
    {
        public static readonly StringName
            TickRate = "tickrate",
            SyncToPhysics = "sync_to_physics",
            MaxTicksPerFrame = "max_ticks_per_frame",
            Time = "time",
            Tick = "tick",
            RecalibrateThreshold = "recalibrate_threshold",
            RemoteTick = "remote_tick",
            RemoteTime = "remote_time",
            RemoteRtt = "remote_rtt",
            LocalTick = "local_tick",
            LocalTime = "local_time",
            TickTime = "ticktime",
            TickFactor = "tick_Factor",
            PhysicsFactor = "physics_factor",
            ClockStretchMax = "clock_stretch_max",
            ClockStretchFactor = "clock_stretch_factor",
            ClockOffset = "clock_offset",
            RemoteClockOffset = "remote_clock_offset";
    }
    static class SignalNameGd
    {
        public static readonly StringName
            BeforeTickLoop = "before_tick_loop",
            BeforeTick = "before_tick",
            OnTick = "on_tick",
            AfterTick = "after_tick",
            AfterTickLoop = "after_tick_loop",
            AftereSync = "after_sync",
            AfterClientSync = "after_client_sync";
    }
    #endregion
}
