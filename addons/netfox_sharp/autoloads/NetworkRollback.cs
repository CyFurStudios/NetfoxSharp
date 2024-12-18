using Godot;

namespace Netfox;

/// <summary><para>C# wrapper for Fox's Sake <see href="https://github.com/foxssake/netfox/">
/// Netfox</see> addon.</para>
/// 
/// <para>Orchestrates the network rollback loop.</para>
/// 
/// <para>See the <see href="https://foxssake.github.io/netfox/netfox/guides/network-rollback/">
/// NetworkRollback</see> Netfox guide for more information.</para></summary>
public partial class NetworkRollback : Node
{
    #region Public Variables

    /// <summary>Whether rollbacks are enabled.</summary>
    public bool Enabled
    {
        get { return (bool)_networkRollbackGd.Get(PropertyNameGd.Enabled); }
        set { _networkRollbackGd.Set(PropertyNameGd.Enabled, value); }
    }
    /// <summary><para>Whether diff states are enabled.</para>
    /// <para>Diff states send only the state properties that have changed.</para></summary>
    public bool EnableDiffStates
    {
        get { return (bool)_networkRollbackGd.Get(PropertyNameGd.EnableDiffStates); }
        set { _networkRollbackGd.Set(PropertyNameGd.EnableDiffStates, value); }
    }
    /// <summary><para>How many ticks to store as history.</para>
    /// <para>Rollback won't go further than this limit, regardless of inputs received.</para></summary>
    public long HistoryLimit { get { return (long)_networkRollbackGd.Get(PropertyNameGd.HistoryLimit); } }
    /// <summary><para>Offset into the past for display.</para>
    /// <para>After the rollback, we have the option to not display the absolute latest
    /// state of the game, but let's say the state two frames ago ( offset = 2 ).
    /// This can help with hiding latency, by giving more time for an up-to-date
    /// state to arrive before we try to display it.</para></summary>
    public long DisplayOffset { get { return (long)_networkRollbackGd.Get(PropertyNameGd.DisplayOffset); } }
    /// <summary><para>How many previous input frames to send along with the current one.</para>
    /// <para>With UDP - packets may be lost, arrive late or out of order.
    /// To mitigate this, we send the current and previous n ticks of input data.</para></summary>
    public long InputRedundancy { get { return (long)_networkRollbackGd.Get(PropertyNameGd.InputRedundancy); } }
    /// <summary>The current tick.</summary>
    public long Tick { get { return (long)_networkRollbackGd.Get(PropertyNameGd.Tick); } }
    #endregion

    /// <summary>Internal reference of the NetworkRollback GDScript autoload.</summary>
    GodotObject _networkRollbackGd;

    /// <summary>Internal constructor used by <see cref="NetfoxCore"/>. Should not be used elsewhere.</summary>
    /// <param name="networkTimeGd">The NetworkRollback GDScript autoload.</param>
    internal NetworkRollback(GodotObject networkTimeGd)
    {
        _networkRollbackGd = networkTimeGd;

        _networkRollbackGd.Connect(SignalNameGd.BeforeLoop, Callable.From(() => EmitSignal(SignalName.BeforeLoop)));
        _networkRollbackGd.Connect(SignalNameGd.OnPrepareTick, Callable.From((long tick) => EmitSignal(SignalName.OnPrepareTick, tick)));
        _networkRollbackGd.Connect(SignalNameGd.OnProcessTick, Callable.From((long tick) => EmitSignal(SignalName.OnProcessTick, tick)));
        _networkRollbackGd.Connect(SignalNameGd.OnRecordTick, Callable.From((long tick) => EmitSignal(SignalName.OnRecordTick, tick)));
        _networkRollbackGd.Connect(SignalNameGd.AfterLoop, Callable.From(() => EmitSignal(SignalName.AfterLoop)));
    }

    #region Signals
    /// <summary>Event emitted before running the network rollback loop.</summary>
    [Signal]
    public delegate void BeforeLoopEventHandler();
    /// <summary><para>Event emitted in preparation of each rollback tick.</para>
    /// <para>Handlers should apply the state and input corresponding to the given tick.</para></summary>
    /// <param name="tick">The tick to be prepared.</param>
    [Signal]
    public delegate void OnPrepareTickEventHandler(long tick);
    /// <summary><para>Event emitted to process the given rollback tick.</para>
    /// <para>Handlers should check if they <b>need</b> to resimulate the given tick, and if so,
    /// generate the next state based on the current data (applied in the prepare
    /// tick phase).</para></summary>
    /// <param name="tick">The tick to be processed.</param>
    [Signal]
    public delegate void OnProcessTickEventHandler(long tick);
    /// <summary><para>Event emitted to record the given rollback tick.</para>
    /// <para>By this time, the tick is advanced from the simulation, handlers should save
    /// their resulting states for the given tick.</para></summary>
    /// <param name="tick">The tick to be processed.</param>
    [Signal]
    public delegate void OnRecordTickEventHandler(long tick);
    /// <summary>Event emitted after running the network rollback loop.</summary>
    [Signal]
    public delegate void AfterLoopEventHandler();
    #endregion

    #region Methods
    /// <summary><para>Submit the resimulation start tick for the current loop.</para>
    /// <para>This is used to determine the resimulation range during each loop.</para></summary>
    /// <param name="tick">The tick to resimulate from, at least.</param>
    public void NotifyResimulationStart(long tick) { _networkRollbackGd.Call(MethodNameGd.NotifyResimulationStart, tick); }
    /// <summary><para>Submit node for simulation.</para>
    /// <para>This is used mostly internally by <see cref="RollbackSynchronizer"/>. The idea is to 
    /// submit each affected node while preparing the tick, and then run only the
    /// nodes that need to be resimulated.</para></summary>
    /// <param name="node"></param>
    public void NotifySimulated(Node node) { _networkRollbackGd.Call(MethodNameGd.NotifySimulated, node); }
    /// <summary><para>Check if node was submitted for simulation.</para>
    /// <para>This is used mostly internally by <see cref="RollbackSynchronizer"/>. The idea is to 
    /// submit each affected node while preparing the tick, and then use
    /// <see cref="IsSimulated(Node)"/> to run only the nodes that need to be resimulated.</para></summary>
    /// <param name="node">The node you want to check is being simulated</param>
    /// <returns>Whether the node is being simulated.</returns>
    public bool IsSimulated(Node node) { return (bool)_networkRollbackGd.Call(MethodNameGd.IsSimulated, node); }
    /// <summary>Check if a network rollback is currently active.</summary>
    /// <returns>Whether the network rollback is currently active.</returns>
    public bool IsRollback() { return (bool)_networkRollbackGd.Call(MethodNameGd.IsRollback); }
    /// <summary><para>Checks if a given object is rollback-aware, IE has a
    /// method named _rollback_tick implemented.</para>
    /// <para>This is used by <see cref="RollbackSynchronizer"/> to see if it should simulate the
    /// given object during rollback.</para></summary>
    /// <param name="what">The object to be checked.</param>
    /// <returns>Whether the object has a method named _rollback_tick</returns>
    public bool IsRollbackAware(GodotObject what) { return (bool)_networkRollbackGd.Call(MethodNameGd.IsRollbackAware, what); }
    /// <summary><para>Calls the _rollback_tick method on the target, running its
    /// simulation for the given rollback tick.</para>
    /// <para>This is used by <see cref="RollbackSynchronizer"/> to resimulate ticks during rollback.
    /// While the _rollback_tick method could be called directly as well, this method exists to
    /// future-proof the code a bit, so the method name is not repeated all over the place.</para>
    /// <para><b>NOTE:</b> Make sure to check if the target is rollback-aware, because if
    /// it's not, this method will run into an error.</para></summary>
    /// <param name="target">The target rollback-aware node.</param>
    /// <param name="delta">The time delta.</param>
    /// <param name="tick">The simulated tick.</param>
    /// <param name="isFresh">Whether this is the first time this tick is being processed.</param>
    public void ProcessRollback(GodotObject target, double delta, long tick, bool isFresh) { _networkRollbackGd.Call(MethodNameGd.ProcessRollback, target, delta, tick, isFresh); }
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
