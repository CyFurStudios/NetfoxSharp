using Godot;
using Godot.Collections;

namespace Netfox;

public partial class StateSynchronizer : Node
{
    #region Exports
    [Export]
    Node root;
    [Export]
    Array<string> properties;
    #endregion

    GodotObject _stateSynchronizer;

    public override void _Ready()
    {
        _stateSynchronizer = (GodotObject)GD.Load<GDScript>("res://addons/netfox/rollback/rollback-synchronizer.gd").New();
        _stateSynchronizer.Set(PropertyNameGd.Name, "RollbackSynchronizer");
        _stateSynchronizer.Set(PropertyNameGd.Root, root);
        _stateSynchronizer.Set(PropertyNameGd.Properties, properties);

        AddChild((Node)_stateSynchronizer);
    }

    #region Methods
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