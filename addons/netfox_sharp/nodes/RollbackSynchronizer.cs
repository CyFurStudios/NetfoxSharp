using Godot;
using Godot.Collections;

namespace Netfox;

public partial class RollbackSynchronizer : Node
{
    #region Exports
    [Export]
    Node root;

    [ExportGroup("States")]
    [Export]
    Array<string> stateProperties;

    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    int fullStateInterval = 24;

    [Export(PropertyHint.Range, "0,128,1,or_greater")]
    int diffAckInterval = 24;

    [ExportGroup("Inputs")]
    [Export]
    Array<string> inputProperties;

    [Export]
    bool enableInputBroadcast = false;
    #endregion

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
    public void ProcessSettings() { _rollbackSync.Call(MethodNameGd.ProcessSettings); }
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