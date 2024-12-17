using Godot;
using Godot.Collections;

namespace Netfox;

public partial class TickInterpolator : Node
{
    #region Exports
    [Export]
    Node root;
    [Export]
    bool enabled;
    [Export]
    Array<string> properties;
    [Export]
    bool recordFirstState;
    [Export]
    bool enableRecording;
    #endregion

    #region Public Variables
    public Node Root
    {
        get { return root; }
        set
        {
            root = value;
            _tickInterpolator?.Set(PropertyNameGd.Root, value);
        }
    }
    public bool Enabled
    {
        get { return enabled; }
        set
        {
            enabled = value;
            _tickInterpolator?.Set(PropertyNameGd.Enabled, value);
        }
    }
    public Array<string> Properties
    {
        get { return properties; }
        set
        {
            properties = value;
            _tickInterpolator?.Set(PropertyNameGd.Properties, value);
        }
    }
    public bool RecordFirstState
    {
        get { return recordFirstState; }
        set
        {
            enabled = value;
            _tickInterpolator?.Set(PropertyNameGd.RecordFirstState, value);
        }
    }
    public bool EnableRecording
    {
        get { return enableRecording; }
        set
        {
            enabled = value;
            _tickInterpolator?.Set(PropertyNameGd.EnableRecording, value);
        }
    }
    #endregion

    GodotObject _tickInterpolator;

    public override void _Ready()
    {
        _tickInterpolator = (GodotObject)GD.Load<GDScript>("res://addons/netfox/rollback/rollback-synchronizer.gd").New();
        _tickInterpolator.Set(PropertyNameGd.Name, "RollbackSynchronizer");
        _tickInterpolator.Set(PropertyNameGd.Root, root);
        _tickInterpolator.Set(PropertyNameGd.Enabled, enabled);
        _tickInterpolator.Set(PropertyNameGd.Properties, properties);
        _tickInterpolator.Set(PropertyNameGd.RecordFirstState, recordFirstState);
        _tickInterpolator.Set(PropertyNameGd.EnableRecording, enableRecording);

        AddChild((Node)_tickInterpolator);
    }

    #region Methods
    public void ProcessSettings() { _tickInterpolator.Call(MethodNameGd.ProcessSettings); }
    public bool CanInterpolate() { return (bool)_tickInterpolator.Call(MethodNameGd.CanInterpolate); }
    public void PushState() { _tickInterpolator.Call(MethodNameGd.PushState); }
    public void Teleport() { _tickInterpolator.Call(MethodNameGd.Teleport); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            ProcessSettings = "process_settings",
            CanInterpolate = "can_interpolate",
            PushState = "push_state",
            Teleport = "teleport";
    }

    static class PropertyNameGd
    {
        public static readonly StringName
            Name = "name",
            Root = "root",
            Enabled = "enabled",
            Properties = "properties",
            RecordFirstState = "record_first_state",
            EnableRecording = "enable_recording";
    }
    #endregion
}