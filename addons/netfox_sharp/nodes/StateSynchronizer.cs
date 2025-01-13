using Godot;
using Godot.Collections;

namespace Netfox;

public partial class StateSynchronizer : Node
{
    #region Exports
    /// <summary>The node from which the <see cref="Properties"/> paths from.</summary>
    [Export]
    public Node Root
    {
        get { return _root; }
        set
        {
            _root = value;
            _stateSynchronizer?.Set(PropertyNameGd.Root, _root);
        }
    }
    Node _root;
    /// <summary>Properties to synchronize from the <see cref="Root"/> node.</summary>
    [Export]
    public Array<string> Properties
    {
        get { return _properties; }
        set
        {
            _properties = value;
            _stateSynchronizer?.Set(PropertyNameGd.Properties, _properties);
        }
    }
    Array<string> _properties;
    #endregion

    /// <summary>The GDScript script used to instance StateSynchronizer.</summary>
    static readonly GDScript _script;

    /// <summary>Internal reference of the StateSynchronizer GDScript node.</summary>
    GodotObject _stateSynchronizer;

    static StateSynchronizer()
    {
        _script = GD.Load<GDScript>("res://addons/netfox/state-synchronizer.gd");
    }

    public override void _Ready()
    {
        _stateSynchronizer = (GodotObject)_script.New();
        _stateSynchronizer.Set(PropertyNameGd.Name, "StateSynchronizer");
        _stateSynchronizer.Set(PropertyNameGd.Root, Root);
        _stateSynchronizer.Set(PropertyNameGd.Properties, Properties);

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