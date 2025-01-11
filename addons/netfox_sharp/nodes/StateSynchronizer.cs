using Godot;
using Godot.Collections;

namespace Netfox;

public partial class StateSynchronizer : Node
{
    #region Exports
    /// <summary>The node from which the <see cref="properties"/> paths from.</summary>
    [Export]
    Node root;
    /// <summary>Properties to synchronize from the <see cref="root"/> node.</summary>
    [Export]
    Array<string> properties;
    #endregion

    /// <summary>Internal reference of the StateSynchronizer GDScript node.</summary>
    GodotObject _stateSynchronizer;

    public override void _Ready()
    {
        _stateSynchronizer = (GodotObject)GD.Load<GDScript>("res://addons/netfox/state-synchronizer.gd").New();
        _stateSynchronizer.Set(PropertyNameGd.Name, "StateSynchronizer");
        _stateSynchronizer.Set(PropertyNameGd.Root, root);
        _stateSynchronizer.Set(PropertyNameGd.Properties, properties);

        AddChild((Node)_stateSynchronizer);
    }

    #region Methods
    /// <summary>Call this after any change to configuration.</summary>
    public void ProcessSettings() { _stateSynchronizer.Call(MethodNameGd.ProcessSettings); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            ProcessSettings = "process_settings";
    }

    static class PropertyNameGd
    {
        public static readonly StringName
            Name = "name",
            Root = "root",
            Properties = "state_properties";
    }
    #endregion
}