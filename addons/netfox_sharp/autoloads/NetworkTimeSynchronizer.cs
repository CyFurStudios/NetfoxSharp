using Godot;

namespace Netfox;

public partial class NetworkTimeSynchronizer : Node
{
    #region Public Variables
    public double SyncInterval { get { return (double)_networkTimeGd.Get(PropertyNameGd.SyncInterval); } }
    public long SyncSamples { get { return (long)_networkTimeGd.Get(PropertyNameGd.SyncSamples); } }
    public long AdjustSteps { get { return (long)_networkTimeGd.Get(PropertyNameGd.AdjustSteps); } }
    public double PanicThreshold { get { return (double)_networkTimeGd.Get(PropertyNameGd.PanicThreshold); } }
    public double Rtt { get { return (double)_networkTimeGd.Get(PropertyNameGd.Rtt); } }
    public double RttJitter { get { return (double)_networkTimeGd.Get(PropertyNameGd.RttJitter); } }
    public double RemoteOffset { get { return (double)_networkTimeGd.Get(PropertyNameGd.RemoteOffset); } }
    #endregion

    GodotObject _networkTimeGd;

    public NetworkTimeSynchronizer(GodotObject networkTimeGd)
    {
        _networkTimeGd = networkTimeGd;

        _networkTimeGd.Connect(SignalNameGd.OnInitialSync, Callable.From(() => EmitSignal(SignalName.OnInitialSync)));
        _networkTimeGd.Connect(SignalNameGd.OnPanic, Callable.From((double offset) => EmitSignal(SignalName.OnPanic, offset)));
    }

    #region Signals
    [Signal]
    public delegate void OnInitialSyncEventHandler();
    [Signal]
    public delegate void OnPanicEventHandler(double offset);
    #endregion

    #region Methods
    public void Start() { _networkTimeGd.Call(MethodNameGd.Start); }
    public void Stop() { _networkTimeGd.Call(MethodNameGd.Stop); }
    public double GetTime() { return (double)_networkTimeGd.Call(MethodNameGd.GetTime); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            Start = "start",
            Stop = "stop",
            GetTime = "get_time";
    }
    static class PropertyNameGd
    {
        public static readonly StringName
            SyncInterval = "sync_interval",
            SyncSamples = "sync_samples",
            AdjustSteps = "adjust_steps",
            PanicThreshold = "panic_threshold",
            Rtt = "rtt",
            RttJitter = "rtt_jitter",
            RemoteOffset = "remote_offset";
    }
    static class SignalNameGd
    {
        public static readonly StringName
            OnInitialSync = "on_initial_sync",
            OnPanic = "on_panic";
    }
    #endregion
}
