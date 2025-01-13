using Godot;
using Godot.Collections;

namespace Netfox;

/// <summary>Responsible for synchronizing data between players, with support for rollback.</summary>
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

    /// <summary>Internal reference of the RollbackSynchronizer GDScript node.</summary>
    GodotObject _rollbackSync;

    static RollbackSynchronizer()
    {
        _script = GD.Load<GDScript>("res://addons/netfox/rollback/rollback-synchronizer.gd");
    }

    public override void _Ready()
    {
        _rollbackSync = (GodotObject)_script.New();
        _rollbackSync.Set(PropertyNameGd.Name, "InternalRollbackSync");
        _rollbackSync.Set(PropertyNameGd.Root, Root);
        _rollbackSync.Set(PropertyNameGd.StateProperties, StateProperties);
        _rollbackSync.Set(PropertyNameGd.FullStateInterval, FullStateInterval);
        _rollbackSync.Set(PropertyNameGd.DiffAckInterval, DiffAckInterval);
        _rollbackSync.Set(PropertyNameGd.InputProperties, InputProperties);
        _rollbackSync.Set(PropertyNameGd.EnableInputBroadcast, EnableInputBroadcast);

        AddChild((Node)_rollbackSync, @internal: InternalMode.Back);
    }

    #region Methods
    /// <summary>Call this after any change to configuration and updates based on authority.
    /// Internally calls <see cref="ProcessAuthority"/>.</summary>
    public void ProcessSettings() { _rollbackSync.Call(MethodNameGd.ProcessSettings); }
    /// <summary>Call this whenever the authority of any of the nodes managed by
    /// this node changes. Make sure to do this at the
    /// same time on all peers.</summary>
    public void ProcessAuthority() { _rollbackSync.Call(MethodNameGd.ProcessAuthority); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            ProcessSettings = "process_settings",
            ProcessAuthority = "process_authority";
    }

    static class PropertyNameGd
    {
        public static readonly StringName
            Name = "name",
            Root = "root",
            StateProperties = "state_properties",
            FullStateInterval = "full_state_interval",
            DiffAckInterval = "diff_ack_interval",
            InputProperties = "input_properties",
            EnableInputBroadcast = "enable_input_broadcast";
    }
    #endregion
}