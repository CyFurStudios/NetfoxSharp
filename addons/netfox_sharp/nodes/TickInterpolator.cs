using Godot;
using Godot.Collections;
using Netfox.Logging;

namespace Netfox;

/// <summary><para>C# wrapper for Fox's Sake Studio's
/// <see href="https://github.com/foxssake/netfox/"> netfox</see> addon.</para>
/// <para>Responsible for interpolating fields between network ticks, resulting
/// in smoother motion.</para></summary>
[Tool]
public partial class TickInterpolator : Node
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
            _tickInterpolator?.Set(PropertyNameGd.Root, value);
        }
    }
    Node _root;

    /// <summary>Whether the tick interpolator is enabled.</summary>
    [Export]
    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            _tickInterpolator?.Set(PropertyNameGd.Enabled, value);
        }
    }
    bool _enabled;

    /// <summary>Properties to interpolate from the <see cref="Root"/> node.</summary>
    [Export]
    public Array<string> Properties
    {
        get { return _properties; }
        set
        {
            _properties = value;
            _tickInterpolator?.Set(PropertyNameGd.Properties, value);
        }
    }
    Array<string> _properties;

    [Export]
    public bool RecordFirstState
    {
        get { return _recordFirstState; }
        set
        {
            _enabled = value;
            _tickInterpolator?.Set(PropertyNameGd.RecordFirstState, value);
        }
    }
    bool _recordFirstState;

    [Export]
    public bool EnableRecording
    {
        get { return _enableRecording; }
        set
        {
            _enabled = value;
            _tickInterpolator?.Set(PropertyNameGd.EnableRecording, value);
        }
    }
    bool _enableRecording;
    #endregion

    /// <summary>The GDScript script used to instance TickInterpolator.</summary>
    static readonly GDScript _script;
    /// <summary>The name for the internal GDScript netfox node.</summary>
    static readonly StringName _proxyName = "InternalTickInterpolator";
    /// <summary>Internal reference of the TickInterpolator GDScript node.</summary>
    Node _tickInterpolator;

    static readonly NetfoxLogger _logger = new("NetfoxSharp", "TickInterpolator");

    static TickInterpolator()
    {
        _script = GD.Load<GDScript>("res://addons/netfox/tick-interpolator.gd");
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
            _logger.LogWarning($"Root is not set! Setting as owner ({Owner?.Name})");
            Root = Owner;
        }

        _tickInterpolator = FindChild(_proxyName, owned: false);
        if (_tickInterpolator != null)
            return;

        _tickInterpolator = (Node)_script.New();

        _tickInterpolator.Set(PropertyNameGd.Name, _proxyName);
        _tickInterpolator.Set(PropertyNameGd.Root, Root);
        _tickInterpolator.Set(PropertyNameGd.Enabled, Enabled);
        _tickInterpolator.Set(PropertyNameGd.Properties, Properties);
        _tickInterpolator.Set(PropertyNameGd.RecordFirstState, RecordFirstState);
        _tickInterpolator.Set(PropertyNameGd.EnableRecording, EnableRecording);

        CallDeferred(MethodName.AddProxyNode);
    }

    private void AddProxyNode()
    {
        if (FindChild(_proxyName) != null)
            return;

        AddChild(_tickInterpolator, forceReadableName: true, @internal: InternalMode.Back);
        _tickInterpolator.Owner = Owner;
    }


    #region Methods
    /// <summary>Call this after any change to configuration.</summary>
    public void ProcessSettings() { _tickInterpolator.Call(MethodNameGd.ProcessSettings); }
    public void AddProperty(Variant node, string property)
    {
        _tickInterpolator.Call(MethodNameGd.AddProperty, node, property);
#if TOOLS
        Properties = (Array<string>)_tickInterpolator.Get(PropertyNameGd.Properties);
#endif
    }
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
            AddProperty = "add_property",
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