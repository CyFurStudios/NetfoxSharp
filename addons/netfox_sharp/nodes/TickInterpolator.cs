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
    /// <summary>The node from which the <see cref="Properties"/> paths from.</summary>
    public Node Root
    {
        get { return root; }
        set
        {
            root = value;
            _tickInterpolator?.Set(PropertyNameGd.Root, value);
        }
    }
    /// <summary>Whether the tick interpolator is enabled.</summary>
    public bool Enabled
    {
        get { return enabled; }
        set
        {
            enabled = value;
            _tickInterpolator?.Set(PropertyNameGd.Enabled, value);
        }
    }
    /// <summary>Properties to interpolate from the <see cref="Root"/> node.</summary>
    public Array<string> Properties
    {
        get { return properties; }
        set
        {
            properties = value;
            // NOTE: Does this also run when the array is modified?
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

    /// <summary>Internal reference of the TickInterpolator GDScript node.</summary>
    GodotObject _tickInterpolator;

    public override void _Ready()
    {
        _tickInterpolator = (GodotObject)GD.Load<GDScript>("res://addons/netfox/rollback/rollback-synchronizer.gd").New();
        _tickInterpolator.Set(PropertyNameGd.Name, "TickInterpolator");
        _tickInterpolator.Set(PropertyNameGd.Root, root);
        _tickInterpolator.Set(PropertyNameGd.Enabled, enabled);
        _tickInterpolator.Set(PropertyNameGd.Properties, properties);
        _tickInterpolator.Set(PropertyNameGd.RecordFirstState, recordFirstState);
        _tickInterpolator.Set(PropertyNameGd.EnableRecording, enableRecording);

        // NOTE: What do you think about making these nodes @tool scripts, and
        // ensuring the wrapped node is there from the editor ( on ready and/or
        // on scene save )? Pro is reduced instantiation cost for C# nodes,
        // cons is slightly more code.
        AddChild((Node)_tickInterpolator, false, InternalMode.Front);
    }

    #region Methods
    // NOTE: Maybe we could set the GD node's arrays to the ones known by the
    // C# class to make sure all changes are picked up? Similar for the rest of
    // the nodes.
    //
    /// <summary>Call this after any change to configuration.</summary>
    public void ProcessSettings() { _tickInterpolator.Call(MethodNameGd.ProcessSettings); }
    /// <summary><para>Check if interpolation can be done.</para>
    /// <para>Even if it's enabled, no interpolation will be done if there are no
    /// properties to interpolate.</para></summary>
    /// <returns>Whether the node is able to and has reason to interpolate.</returns>
    public bool CanInterpolate() { return (bool)_tickInterpolator.Call(MethodNameGd.CanInterpolate); }
    /// <summary><para>Record current state for interpolation.</para>
    /// <para>Note that this will rotate the states, so the previous target becomes the new
    /// starting point for the interpolation. This is automatically called if 
    /// <see cref="EnableRecording"/> is true.</para></summary>
    public void PushState() { _tickInterpolator.Call(MethodNameGd.PushState); }
    /// <summary>Record current state and transition without interpolation.</summary>
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
