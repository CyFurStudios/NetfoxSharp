using Godot;
using Godot.Collections;

namespace Netfox;

/// <summary>Responsible for synchronizing data between players, with support for rollback.</summary>
public partial class RollbackSynchronizer : Node
{
    #region Exports
    /// <summary>The node from which the <see cref="inputProperties"/> and
    /// <see cref="stateProperties"/> paths from.</summary>
    [Export]
    Node root;

    /// <summary>State properties to roll back from the <see cref="root"/> node.</summary>
    [ExportGroup("States")]
    [Export]
    Array<string> stateProperties;

    /// <summary><para>Ticks to wait between sending full states.</para>
    /// <para>If set to 0, full states will never be sent. If set to 1, only full states
    /// will be sent. If set higher, full states will be sent regularly, but not
    /// for every tick.</para>
    /// <para>Only considered if <see cref="NetworkRollback.EnableDiffStates"/> is true.</para></summary>
    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    int fullStateInterval = 24;

    /// <summary><para>Ticks to wait between unreliably acknowledging diff states.</para>
    /// <para>This can reduce the amount of properties sent in diff states, due to clients
    /// more often acknowledging received states. To avoid introducing hickups, these
    /// are sent unreliably.</para>
    /// <para>If set to 0, diff states will never be acknowledged. If set to 1, all diff 
    /// states will be acknowledged. If set higher, ack's will be sent regularly, but
    /// not for every diff state.</para>
    /// <para>Only considered if <see cref="NetworkRollback.EnableDiffStates"/> is true.</para></summary>
    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    int diffAckInterval = 24;

    /// <summary>Input properties to roll back from the <see cref="root"/> node.</summary>
    [ExportGroup("Inputs")]
    [Export]
    Array<string> inputProperties;

    /// <summary>This will broadcast input to all peers, turning this off will limit to sending it
    /// to the server only. Recommended not to use unless needed due to bandwidth considerations.</summary>
    [Export]
    bool enableInputBroadcast = false;
    #endregion

    /// <summary>Internal reference of the RollbackSynchronizer GDScript node.</summary>
    GodotObject _rollbackSync;

    public override void _Ready()
    {
        _rollbackSync = (GodotObject)GD.Load<GDScript>("res://addons/netfox/rollback/rollback-synchronizer.gd").New();
        _rollbackSync.Set(PropertyNameGd.Name, "RollbackSynchronizer");
        _rollbackSync.Set(PropertyNameGd.Root, root);
        _rollbackSync.Set(PropertyNameGd.StateProperties, stateProperties);
        _rollbackSync.Set(PropertyNameGd.FullStateInterval, fullStateInterval);
        _rollbackSync.Set(PropertyNameGd.DiffAckInterval, diffAckInterval);
        _rollbackSync.Set(PropertyNameGd.InputProperties, inputProperties);
        _rollbackSync.Set(PropertyNameGd.EnableInputBroadcast, enableInputBroadcast);

        AddChild((Node)_rollbackSync);
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