using Godot;

namespace Netfox;

public partial class NetworkRollback : Node
{
    #region Public Variables
    public bool Enabled
    {
        get { return (bool)_networkTimeGd.Get(PropertyNameGd.Enabled); }
        set { _networkTimeGd.Set(PropertyNameGd.Enabled, value); }
    }
    public bool EnableDiffStates
    {
        get { return (bool)_networkTimeGd.Get(PropertyNameGd.EnableDiffStates); }
        set { _networkTimeGd.Set(PropertyNameGd.EnableDiffStates, value); }
    }
    public long HistoryLimit { get { return (long)_networkTimeGd.Get(PropertyNameGd.HistoryLimit); } }
    public long DisplayOffset { get { return (long)_networkTimeGd.Get(PropertyNameGd.DisplayOffset); } }
    public long InputRedundancy { get { return (long)_networkTimeGd.Get(PropertyNameGd.InputRedundancy); } }
    public long Tick { get { return (long)_networkTimeGd.Get(PropertyNameGd.Tick); } }
    #endregion

    GodotObject _networkTimeGd;

    public NetworkRollback(GodotObject networkTimeGd)
    {
        _networkTimeGd = networkTimeGd;

        _networkTimeGd.Connect(SignalNameGd.BeforeLoop, Callable.From(() => EmitSignal(SignalName.BeforeLoop)));
        _networkTimeGd.Connect(SignalNameGd.OnPrepareTick, Callable.From((long tick) => EmitSignal(SignalName.OnPrepareTick, tick)));
        _networkTimeGd.Connect(SignalNameGd.OnProcessTick, Callable.From((long tick) => EmitSignal(SignalName.OnProcessTick, tick)));
        _networkTimeGd.Connect(SignalNameGd.OnRecordTick, Callable.From((long tick) => EmitSignal(SignalName.OnRecordTick, tick)));
        _networkTimeGd.Connect(SignalNameGd.AfterLoop, Callable.From(() => EmitSignal(SignalName.AfterLoop)));
    }

    #region Signals
    [Signal]
    public delegate void BeforeLoopEventHandler();
    [Signal]
    public delegate void OnPrepareTickEventHandler(long tick);
    [Signal]
    public delegate void OnProcessTickEventHandler(long tick);
    [Signal]
    public delegate void OnRecordTickEventHandler(long tick);
    [Signal]
    public delegate void AfterLoopEventHandler();
    #endregion

    #region Methods
    public void NotifyResimulationStart(long tick) { _networkTimeGd.Call(MethodNameGd.NotifyResimulationStart, tick); }
    public void NotifySimulated(Node node) { _networkTimeGd.Call(MethodNameGd.NotifySimulated, node); }
    public bool IsSimulated(Node node) { return (bool)_networkTimeGd.Call(MethodNameGd.IsSimulated, node); }
    public bool IsRollback() { return (bool)_networkTimeGd.Call(MethodNameGd.IsRollback); }
    public bool IsRollbackAware(GodotObject what) { return (bool)_networkTimeGd.Call(MethodNameGd.IsRollbackAware, what); }
    public void ProcessRollback(GodotObject target, double delta, long tick, bool isFresh) { _networkTimeGd.Call(MethodNameGd.ProcessRollback, target, delta, tick, isFresh); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            NotifyResimulationStart = "notify_resimulation_start",
            NotifySimulated = "notify_simulated",
            IsSimulated = "is_simulated",
            IsRollback = "is_rollback",
            IsRollbackAware = "is_rollback_aware",
            ProcessRollback = "process_rollback";
    }
    static class PropertyNameGd
    {
        public static readonly StringName
            Enabled = "enabled",
            EnableDiffStates = "enable_diff_states",
            HistoryLimit = "history_limit",
            DisplayOffset = "display_offset",
            InputRedundancy = "input_redundancy",
            Tick = "tick";
    }
    static class SignalNameGd
    {
        public static readonly StringName
            BeforeLoop = "before_loop",
            OnPrepareTick = "on_prepare_tick",
            OnProcessTick = "on_process_tick",
            OnRecordTick = "on_record_tick",
            AfterLoop = "after_loop";
    }
    #endregion
}
