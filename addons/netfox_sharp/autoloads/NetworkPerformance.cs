using Godot;
using Godot.Collections;

namespace Netfox;

/// <summary><para>C# wrapper for Fox's Sake Studio's <see href="https://github.com/foxssake/netfox/">
/// netfox</see> addon.</para>
/// 
/// <para>Provides custom monitors for measuring networking performance.</para>
/// 
/// <para>See the <see href="https://foxssake.github.io/netfox/latest/netfox/guides/network-performance/">
/// NetworkPerformance</see> netfox guide for more information.</para></summary>
public partial class NetworkPerformance : Node
{
    /// <summary>Internal reference of the NetworkTime GDScript autoload.</summary>
    GodotObject _networkPerformanceGd;

    /// <summary>Internal constructor used by <see cref="NetfoxSharp"/>.
    /// Should not be used elsewhere.</summary>
    /// <param name="networkTimeGd">The NetworkTime GDScript autoload.</param>
    internal NetworkPerformance(GodotObject networkTimeGd)
    {
        _networkPerformanceGd = networkTimeGd;
    }
    #region Methods

    /// <summary>Checks to see if network performance monitoring is enabled.</summary>
    /// <returns>Whether network performance monitoring is enabled.</returns>
    public bool IsEnabled() { return (bool)_networkPerformanceGd.Call(MethodNameGd.IsEnabled); }
    /// <summary>Get time spent in the last network tick loop, including time spent
    /// in the rollback tick loop.</summary>
    /// <returns>The total time, in msec.</returns>
    private double GetNetworkLoopDurationMs() { return (double)_networkPerformanceGd.Call(MethodNameGd.GetNetworkLoopDurationMs); }
    /// <summary>Get the number of ticks simulated in the last network tick loop.</summary>
    /// <returns>The number of ticks simulated.</returns>
    private long GetNetworkTicks() { return (long)_networkPerformanceGd.Call(MethodNameGd.GetNetworkTicks); }
    /// <summary>Get time spent in the last rollback tick loop.</summary>
    /// <returns>The total time, in msec.</returns>
    private double GetRollbackLoopDurationMs() { return (double)_networkPerformanceGd.Call(MethodNameGd.GetRollbackLoopDurationMs); }
    /// <summary>Get the number of ticks resimulated in the last rollback tick loop.</summary>
    /// <returns>The number of ticks resimulated.</returns>
    private long GetRollbackTicks() { return (long)_networkPerformanceGd.Call(MethodNameGd.GetRollbackTicks); }
    /// <summary>Get the average amount of time spent in a rollback tick during the last
    /// rollback loop.</summary>
    /// <returns>The average time, in msec.</returns>
    private double GetRollbackTickDurationMs() { return (double)_networkPerformanceGd.Call(MethodNameGd.GetRollbackTickDurationMs); }
    /// <summary>Get the number of properties in the full state recorded during the last tick
    /// loop.</summary>
    /// <returns>The number of properties in the full state.</returns>
    private long GetFullStatePropsCount() { return (long)_networkPerformanceGd.Call(MethodNameGd.GetFullStatePropsCount); }
    /// <summary>Get the number of properties actually sent during the last tick loop.</summary>
    /// <returns>The number of properties sent.</returns>
    private long GetSentStatePropsCount() { return (long)_networkPerformanceGd.Call(MethodNameGd.GetSentStatePropsCount); }
    /// <summary>Get the ratio of <see cref="GetSentStatePropsCount"/> to <see cref="GetFullStatePropsCount"/>.</summary>
    /// <returns>Ratio of sent properties count to full properties count.</returns>
    private double GetSentStatePropsRatio() { return (double)_networkPerformanceGd.Call(MethodNameGd.GetSentStatePropsRatio); }
    private void PushFullState(Dictionary dictionary) { _networkPerformanceGd.Call(MethodNameGd.PushFullState, dictionary); }
    private void PushFullStateBroadcast(Dictionary dictionary) { _networkPerformanceGd.Call(MethodNameGd.PushFullStateBroadcast, dictionary); }
    private void PushSentState(Dictionary dictionary) { _networkPerformanceGd.Call(MethodNameGd.PushSentState, dictionary); }
    private void PushSentStateBroadcast(Dictionary dictionary) { _networkPerformanceGd.Call(MethodNameGd.PushSentStateBroadcast, dictionary); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            IsEnabled = "is_enabled",
            GetNetworkLoopDurationMs = "get_network_loop_duration_ms",
            GetNetworkTicks = "get_network_ticks",
            GetRollbackLoopDurationMs = "get_rollback_loop_duration_ms",
            GetRollbackTicks = "get_rollback_ticks",
            GetRollbackTickDurationMs = "get_rollback_tick_duration_ms",
            GetFullStatePropsCount = "get_full_state_props_count",
            GetSentStatePropsCount = "get_sent_state_props_count",
            GetSentStatePropsRatio = "get_sent_state_props_ratio",
            PushFullState = "push_full_state",
            PushFullStateBroadcast = "push_full_state_broadcast",
            PushSentState = "push_sent_state",
            PushSentStateBroadcast = "push_sent_state_broadcast";

    }
    #endregion
}
