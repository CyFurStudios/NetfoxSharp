using Godot;

namespace Netfox;

/// <summary><para>C# wrapper for Fox's Sake Studio's <see href="https://github.com/foxssake/netfox/">
/// netfox</see> addon.</para>
/// 
/// <para>Synchronizes time to the host remote.</para>
/// 
/// <para>Synchronization is done by continuously pinging the host remote, and using these
/// samples to figure out clock difference and network latency. These are then used to
/// gradually adjust the local clock to keep in sync.</para>
/// 
/// <para>See the <see href="https://foxssake.github.io/netfox/latest/netfox/guides/network-rollback/">
/// NetworkRollback</see> netfox guide for more information.</para></summary>
public partial class NetworkTimeSynchronizer : Node
{
    #region Public Variables
    /// <summary>Time between sync samples, in seconds.</summary>
    public static double SyncInterval { get { return (double)_networkTimeSynchronizerGd.Get(PropertyNameGd.SyncInterval); } }
    /// <summary>Number of measurements (samples) to use for time synchronization.</summary>
    public static long SyncSamples { get { return (long)_networkTimeSynchronizerGd.Get(PropertyNameGd.SyncSamples); } }
    /// <summary><para>Number of iterations to nudge towards the host's remote clock.</para>
    /// <para>Lower values result in more aggressive changes in clock and may be more 
    /// sensitive to jitter. Larger values may end up approaching the remote clock
    /// too slowly.</para></summary>
    public static long AdjustSteps { get { return (long)_networkTimeSynchronizerGd.Get(PropertyNameGd.AdjustSteps); } }
    /// <summary><para>Largest tolerated offset from the host's remote clock before panicking.</para>
    /// <para>Once this threshold is reached, the clock will be reset to the remote clock's 
    /// value, and the nudge process will start from scratch.</para></summary>
    public static double PanicThreshold { get { return (double)_networkTimeSynchronizerGd.Get(PropertyNameGd.PanicThreshold); } }
    /// <summary><para>Measured roundtrip time measured to the host.</para>
    /// <para>This value is calculated from multiple samples. The actual roundtrip times 
    /// can be anywhere in the <see cref="Rtt"/> +/- <see cref="RttJitter"/> range.</para></summary>
    public static double Rtt { get { return (double)_networkTimeSynchronizerGd.Get(PropertyNameGd.Rtt); } }
    /// <summary><para>Measured jitter in the roundtrip time to the host remote.</para>
    /// <para>This value is calculated from multiple samples. The actual roundtrip times 
    /// can be anywhere in the <see cref="Rtt"/> +/- <see cref="RttJitter"/> range.</para></summary>
    public static double RttJitter { get { return (double)_networkTimeSynchronizerGd.Get(PropertyNameGd.RttJitter); } }
    /// <summary><para>Estimated offset from the host's remote clock.</para>
    /// <para>Positive values mean that the host's remote clock is ahead of ours, while
    /// negative values mean that our clock is behind the host's remote.</para></summary>
    public static double RemoteOffset { get { return (double)_networkTimeSynchronizerGd.Get(PropertyNameGd.RemoteOffset); } }
    #endregion

    /// <summary>Internal reference of the NetworkTimeSynchronizer GDScript autoload.</summary>
    static GodotObject _networkTimeSynchronizerGd;

    /// <summary>Internal constructor used by <see cref="NetfoxSharp"/>. Should not be used elsewhere.</summary>
    /// <param name="networkTimeGd">The NetworkTimeSynchronizer GDScript autoload.</param>
    internal NetworkTimeSynchronizer(GodotObject networkTimeGd)
    {
        _networkTimeSynchronizerGd = networkTimeGd;

        _networkTimeSynchronizerGd.Connect(SignalNameGd.OnInitialSync, Callable.From(() => EmitSignal(SignalName.OnInitialSync)));
        _networkTimeSynchronizerGd.Connect(SignalNameGd.OnPanic, Callable.From((double offset) => EmitSignal(SignalName.OnPanic, offset)));
    }

    #region Signals
    /// <summary><para>Emitted after the initial time sync.</para>
    /// <para>At the start of the game, clients request an initial timestamp to kickstart 
    /// their time sync loop. This event is emitted once that initial timestamp is 
    /// received.</para></summary>
    [Signal]
    public delegate void OnInitialSyncEventHandler();
    /// <summary><para>Emitted when clocks get overly out of sync and a time sync panic occurs.</para>
    /// <para>Panic means that the difference between clocks is too large. The time sync 
    /// will reset the clock to the remote clock's time and restart the time sync loop 
    /// from there.</para>
    /// <para>Use this event in case you need to react to clock changes in your game.</para></summary>
    /// <param name="offset"></param>
    [Signal]
    public delegate void OnPanicEventHandler(double offset);
    #endregion

    #region Methods
    /// <summary><para>Starts the NetworkTimeSynchronizer.</para></summary>
    public static void Start() { _networkTimeSynchronizerGd.Call(MethodNameGd.Start); }
    /// <summary><para>Stops the NetworkTimeSynchronizer.</para></summary>
    public static void Stop() { _networkTimeSynchronizerGd.Call(MethodNameGd.Stop); }
    /// <summary>Get the current time from the reference clock.</summary>
    /// <returns>The time, in seconds.</returns>
    public static double GetTime() { return (double)_networkTimeSynchronizerGd.Call(MethodNameGd.GetTime); }
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
