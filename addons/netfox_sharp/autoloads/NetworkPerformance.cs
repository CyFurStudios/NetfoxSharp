using Godot;
using Godot.Collections;

namespace Netfox;

public partial class NetworkPerformance : Node
{
    GodotObject _networkTimeGd;

    public NetworkPerformance(GodotObject networkTimeGd)
    {
        _networkTimeGd = networkTimeGd;
    }

    #region Methods
    public bool IsEnabled() { return (bool)_networkTimeGd.Call(MethodNameGd.IsEnabled); }
    public double GetNetworkLoopDurationMs() { return (double)_networkTimeGd.Call(MethodNameGd.GetNetworkLoopDurationMs); }
    public long GetNetworkTicks() { return (long)_networkTimeGd.Call(MethodNameGd.GetNetworkTicks); }
    public double GetRollbackLoopDurationMs() { return (double)_networkTimeGd.Call(MethodNameGd.GetRollbackLoopDurationMs); }
    public long GetRollbackTicks() { return (long)_networkTimeGd.Call(MethodNameGd.GetRollbackTicks); }
    public double GetRollbackTickDurationMs() { return (double)_networkTimeGd.Call(MethodNameGd.GetRollbackTickDurationMs); }
    public long GetFullStatePropsCount() { return (long)_networkTimeGd.Call(MethodNameGd.GetFullStatePropsCount); }
    public long GetSentStatePropsCount() { return (long)_networkTimeGd.Call(MethodNameGd.GetSentStatePropsCount); }
    public double GetSentStatePropsRatio() { return (double)_networkTimeGd.Call(MethodNameGd.GetSentStatePropsRatio); }
    public void PushFullState(Dictionary dictionary) { _networkTimeGd.Call(MethodNameGd.PushFullState, dictionary); }
    public void PushFullStateBroadcast(Dictionary dictionary) { _networkTimeGd.Call(MethodNameGd.PushFullStateBroadcast, dictionary); }
    public void PushSentState(Dictionary dictionary) { _networkTimeGd.Call(MethodNameGd.PushSentState, dictionary); }
    public void PushSentStateBroadcast(Dictionary dictionary) { _networkTimeGd.Call(MethodNameGd.PushSentStateBroadcast, dictionary); }
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
