using Godot;
using Godot.Collections;
using Netfox.Logging;

namespace Netfox;


/// <summary><para>C# wrapper for Fox's Sake Studio's
/// <see href="https://github.com/foxssake/netfox/"> netfox</see> addon.</para>
/// <para>Responsible for synchronizing data between players, with support for
/// rollback.</para></summary>
[Tool]
public partial class RollbackSynchronizer : Node
{
    #region Exports
    /// <summary>The node from which the <see cref="InputProperties"/> and
    /// <see cref="StateProperties"/> paths from.</summary>
    [Export]
    public Node Root
    {
        get { return _root; }
        set
        {
            _root = value;
            _rollbackSync?.Set(PropertyNameGd.Root, _root);
        }
    }
    Node _root;

    [Export]
    public bool EnablePrediction
    {
        get { return _enablePrediction; }
        set
        {
            _enablePrediction = value;
            _rollbackSync?.Set(PropertyNameGd.EnablePrediction, _enablePrediction);
        }
    }
    bool _enablePrediction;

    /// <summary>State properties to roll back from the <see cref="Root"/> node.</summary>
    [ExportGroup("States")]
    [Export]
    public Array<string> StateProperties
    {
        get { return _stateProperties; }
        set
        {
            _stateProperties = value;
            _rollbackSync?.Set(PropertyNameGd.StateProperties, _stateProperties);
        }
    }
    Array<string> _stateProperties;

    /// <summary><para>Ticks to wait between sending full states.</para>
    /// <para>If set to 0, full states will never be sent. If set to 1, only full states
    /// will be sent. If set higher, full states will be sent regularly, but not
    /// for every tick.</para>
    /// <para>Only considered if <see cref="NetworkRollback.EnableDiffStates"/> is true.</para></summary>
    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    public int FullStateInterval
    {
        get { return _fullStateInterval; }
        set
        {
            _fullStateInterval = value;
            _rollbackSync?.Set(PropertyNameGd.FullStateInterval, _fullStateInterval);
        }
    }
    int _fullStateInterval = 24;

    /// <summary><para>Ticks to wait between unreliably acknowledging diff states.</para>
    /// <para>This can reduce the amount of properties sent in diff states, due to clients
    /// more often acknowledging received states. To avoid introducing hickups, these
    /// are sent unreliably.</para>
    /// <para>If set to 0, diff states will never be acknowledged. If set to 1, all diff 
    /// states will be acknowledged. If set higher, ack's will be sent regularly, but
    /// not for every diff state.</para>
    /// <para>Only considered if <see cref="NetworkRollback.EnableDiffStates"/> is true.</para></summary>
    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    public int DiffAckInterval
    {
        get { return _diffAckInterval; }
        set
        {
            _diffAckInterval = value;
            _rollbackSync?.Set(PropertyNameGd.DiffAckInterval, _diffAckInterval);
        }
    }
    int _diffAckInterval = 24;

    /// <summary>Input properties to roll back from the <see cref="Root"/> node.</summary>
    [ExportGroup("Inputs")]
    [Export]
    public Array<string> InputProperties
    {
        get { return _inputProperties; }
        set
        {
            _inputProperties = value;
            _rollbackSync?.Set(PropertyNameGd.InputProperties, _inputProperties);
        }
    }
    Array<string> _inputProperties;

    /// <summary>This will broadcast input to all peers, turning this off will limit to sending it
    /// to the server only. Recommended not to use unless needed due to bandwidth considerations.</summary>
    [Export]
    public bool EnableInputBroadcast
    {
        get { return _enableInputBroadcast; }
        set
        {
            _enableInputBroadcast = value;
            _rollbackSync?.Set(PropertyNameGd.EnableInputBroadcast, _enableInputBroadcast);
        }
    }
    bool _enableInputBroadcast = false;
    #endregion

    /// <summary>The GDScript script used to instance RollbackSynchronizer.</summary>
    static readonly GDScript _script;
    /// <summary>The name for the internal GDScript netfox node.</summary>
    static readonly StringName _proxyName = "InternalRollbackSynchronizer";
    /// <summary>Internal reference of the RollbackSynchronizer GDScript node.</summary>
    Node _rollbackSync;

    static readonly NetfoxLogger _logger = new("NetfoxSharp", "RollbackSynchronzier");

    static RollbackSynchronizer()
    {
        _script = GD.Load<GDScript>("res://addons/netfox/rollback/rollback-synchronizer.gd");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationReady ||
            what == NotificationEditorPreSave)
            Initialize();
    }

    private void Initialize()
    {
        if (Root == null)
        {
            _logger.LogWarning($"Root is not set! Setting as owner ({Owner.Name})");
            Root = Owner;
        }

        _rollbackSync = FindChild(_proxyName, owned: false);
        if (_rollbackSync != null)
            return;

        _rollbackSync = (Node)_script.New();

        _rollbackSync.Set(PropertyNameGd.Name, _proxyName);
        _rollbackSync.Set(PropertyNameGd.Root, Root);
        _rollbackSync.Set(PropertyNameGd.StateProperties, StateProperties);
        _rollbackSync.Set(PropertyNameGd.FullStateInterval, FullStateInterval);
        _rollbackSync.Set(PropertyNameGd.DiffAckInterval, DiffAckInterval);
        _rollbackSync.Set(PropertyNameGd.InputProperties, InputProperties);
        _rollbackSync.Set(PropertyNameGd.EnableInputBroadcast, EnableInputBroadcast);

        CallDeferred(MethodName.AddProxyNode);
    }

    private void AddProxyNode()
    {
        if (FindChild(_proxyName) != null)
            return;

        AddChild(_rollbackSync, forceReadableName: true, @internal: InternalMode.Back);
        _rollbackSync.Owner = Owner;
    }

    #region Methods
    /// <summary>Call this after any change to configuration and updates based on authority.
    /// Internally calls <see cref="ProcessAuthority"/>.</summary>
    public void ProcessSettings() { _rollbackSync.Call(MethodNameGd.ProcessSettings); }
    /// <summary>Call this whenever the authority of any of the nodes managed by
    /// this node changes. Make sure to do this at the
    /// same time on all peers.</summary>
    public void ProcessAuthority() { _rollbackSync.Call(MethodNameGd.ProcessAuthority); }
    /// <summary><para>Add a state property.</para>
    /// <para>If the given property is already tracked, this method does nothing.</para>
    /// <para><b>NOTE:</b> Functionality differs between netfox in that the
    /// NetfoxSharp version doesn't currently support tooling/automatic updating.</para></summary>
    /// <param name="node">A string, a <see cref="NodePath"/> pointing to a node, or a <see cref="Node"/> instance.</param>
    /// <param name="property">the property to be added.</param>
    public void AddState(Variant node, string property)
    {
        _rollbackSync.Call(MethodNameGd.AddState, node, property);
#if TOOLS
        StateProperties = (Array<string>)_rollbackSync.Get(PropertyNameGd.StateProperties);
#endif
    }
    /// <summary><para>Add an input property.</para>
    /// <para>If the given property is already tracked, this method does nothing.</para>
    /// <para><b>NOTE:</b> Functionality differs between netfox in that the
    /// NetfoxSharp version doesn't currently support tooling/automatic updating.</para></summary>
    /// <param name="node">A string, a <see cref="NodePath"/> pointing to a node, or a <see cref="Node"/> instance.</param>
    /// <param name="property">the property to be added.</param>
    public void AddInput(Variant node, string property)
    {
        _rollbackSync.Call(MethodNameGd.AddInput, node, property);
#if TOOLS
        InputProperties = (Array<string>)_rollbackSync.Get(PropertyNameGd.InputProperties);
#endif
    }
    /// <summary><para>Check if input is available for the current tick.</para>
    /// <para>This input is not always current, it may be from multiple ticks ago.</para>
    /// <returns>True if input is available.</returns>
    public bool HasInput() { return (bool)_rollbackSync.Call(MethodNameGd.HasInput); }
    /// <summary><para>Get the age of currently available input in ticks.</para>
    /// <para>The available input may be from the current tick, or from multiple ticks ago.
    /// This number of tick is the input's age.</para>
    /// <para>Calling this when <see cref="HasInput"/> is false will yield an error.</para></summary>
    /// <returns>How many ticks elapsed since the input tick.</returns>
    public long GetInputAge() { return (long)_rollbackSync.Call(MethodNameGd.GetInputAge); }
    /// <summary><para>Check if the current tick is predicted.</para>
    /// <para>A tick becomes predicted if there's no up-to-date input available. It will be
    /// simulated and recorded, but will not be broadcast, nor considered
    /// authoritative.</para></summary>
    /// <returns>If the current tick is being predicted.</returns>
    public bool IsPredicting() { return (bool)_rollbackSync.Call(MethodNameGd.IsPredicting); }
    /// <summary><para>Ignore a node's prediction for the current rollback tick.</para>
    /// <para>Call this when the input is too old to base predictions on. This call is
    /// ignored if <see cref="EnablePrediction"/> is false.</para></summary>
    /// <param name="node"></param>
    public void IgnorePrediction(Node node) { _rollbackSync.Call(MethodNameGd.IgnorePrediction, node); }
    /// <summary><para>Get the tick of the last known input.</para>
    /// <para>This is the latest tick where input information is available. If there's
    /// locally owned input for this instance ( e.g. running as client ), this value
    /// will be the current tick. Otherwise, this will be the latest tick received
    /// from the input owner.</para>
    /// <para>If <see cref="EnableInputBroadcast"/> is false, there may be no input available
    /// for peers who own neither state nor input.</para></summary>
    /// <returns>Last known input.</returns>
    public long GetLastKnownInput() { return (long)_rollbackSync.Call(MethodNameGd.GetLastKnownInput); }
    /// <summary><para>Get the tick of the last known state.</para>
    /// <para>This is the latest tick where information is available for state. For state
    /// owners ( usually the host ), this is the current tick. Note that even this
    /// data may change as new input arrives. For peers that don't own state, this
    /// will be the tick of the latest state received from the state owner.</para>
    /// <para>If <see cref="EnableInputBroadcast"/> is false, there may be no input available
    /// for peers who own neither state nor input.</para></summary>
    /// <returns>Last known state.</returns>
    public long GetLastKnownState() { return (long)_rollbackSync.Call(MethodNameGd.GetLastKnownState); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            ProcessSettings = "process_settings",
            ProcessAuthority = "process_authority",
            AddState = "add_state",
            AddInput = "add_input",
            HasInput = "has_input",
            GetInputAge = "get_input_age",
            IsPredicting = "is_predicting",
            IgnorePrediction = "ignore_prediction",
            GetLastKnownInput = "get_last_known_input",
            GetLastKnownState = "get_last_known_state";
    }

    static class PropertyNameGd
    {
        public static readonly StringName
            Name = "name",
            Root = "root",
            EnablePrediction = "enable_prediction",
            StateProperties = "state_properties",
            FullStateInterval = "full_state_interval",
            DiffAckInterval = "diff_ack_interval",
            InputProperties = "input_properties",
            EnableInputBroadcast = "enable_input_broadcast";
    }
    #endregion
}