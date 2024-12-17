using Godot;
using Godot.Collections;

namespace Netfox.Logging;

public class NetfoxLogger
{
    static GodotObject _staticLogger;
    GodotObject _logger;

    const int DefaultLogLevel = 2;

    public static int LogLevel
    {
        get { return (int)_staticLogger?.Get(PropertyNameGd.LogLevel); ; }
        set { _staticLogger?.Set(PropertyNameGd.LogLevel, value); }
    }
    public static Dictionary ModuleLogLevel
    {
        get { return (Dictionary)_staticLogger?.Get(PropertyNameGd.ModuleLogLevel); ; }
        set { _staticLogger?.Set(PropertyNameGd.ModuleLogLevel, value); }
    }
    public string Module
    {
        get { return (string)_logger.Get(PropertyNameGd.Module); ; }
        set { _logger.Set(PropertyNameGd.Module, value); }
    }
    public string Name
    {
        get { return (string)_logger.Get(PropertyNameGd.Name); ; }
        set { _logger.Set(PropertyNameGd.Name, value); }
    }
    string name;

    static NetfoxLogger()
    {
        _staticLogger = (GodotObject)GD.Load<GDScript>("res://addons/netfox.internals/logger.gd").New("Static Logger", "Static Logger");
    }

    NetfoxLogger(string module, string name)
    {
        _logger = (GodotObject)GD.Load<GDScript>("res://addons/netfox.internals/logger.gd").New(module, name);
    }

    public static NetfoxLogger ForNetfox(string nodeName) { return new("netfox", nodeName); }
    public static NetfoxLogger ForNoray(string nodeName) { return new("netfox.noray", nodeName); }
    public static NetfoxLogger ForExtras(string nodeName) { return new("netfox.extras", nodeName); }
    public static Dictionary MakeSetting(string name) { return (Dictionary)_staticLogger.Call(MethodNameGd.MakeSetting, name); }
    public static void RegisterTag(Callable tag, int priority = 0) { _staticLogger.Call(MethodNameGd.RegisterTag, tag, priority); }

    public void LogTrace(string message) { _logger.Call(MethodNameGd.Trace, message, new Array()); }
    public void LogDebug(string message) { _logger.Call(MethodNameGd.Debug, message, new Array()); }
    public void LogInfo(string message) { _logger.Call(MethodNameGd.Info, message, new Array()); }
    public void LogWarning(string message) { _logger.Call(MethodNameGd.Warning, message, new Array()); }
    public void LogError(string message) { _logger.Call(MethodNameGd.Error, message, new Array()); }

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            MakeSetting = "make_setting",
            RegisterTag = "register_tag",
            Trace = "trace",
            Debug = "debug",
            Info = "info",
            Warning = "warning",
            Error = "error";
    }
    static class PropertyNameGd
    {
        public static readonly StringName
            LogLevel = "log_level",
            ModuleLogLevel = "module_log_level",
            Module = "module",
            Name = "name";
    }
    #endregion
}