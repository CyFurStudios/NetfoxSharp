﻿using Godot;
using Godot.Collections;

namespace Netfox.Logging;

public class NetfoxLogger
{
    static GodotObject _staticLogger;
    GodotObject _logger;

    const int DefaultLogLevel = 2;

    /// <summary>The minimum level needed to print a given log. Logs must also meet their
    /// respective <see cref="ModuleLogLevel"/>, if applicable.</summary>
    public static int LogLevel
    {
        get { return (int)_staticLogger?.Get(PropertyNameGd.LogLevel); ; }
        set { _staticLogger?.Set(PropertyNameGd.LogLevel, value); }
    }
    /// <summary>The minimum level needed to print the log of a specific module. Logs must also
    /// meet the minimum <see cref="LogLevel"/>.</summary>
    public static Dictionary ModuleLogLevel
    {
        get { return (Dictionary)_staticLogger?.Get(PropertyNameGd.ModuleLogLevel); ; }
        set { _staticLogger?.Set(PropertyNameGd.ModuleLogLevel, value); }
    }
    // NOTE: Maybe using "belongs to" instead of "comes from" is slightly more accurate.
    /// <summary>The addon/module that the logger comes from.</summary>
    public string Module
    {
        get { return (string)_logger.Get(PropertyNameGd.Module); ; }
        set { _logger.Set(PropertyNameGd.Module, value); }
    }
    // NOTE: Name is just a generic logger name, not tied to node or any real object
    /// <summary>The name of the logger.</summary>
    public string Name
    {
        get { return (string)_logger.Get(PropertyNameGd.Name); ; }
        set { _logger.Set(PropertyNameGd.Name, value); }
    }

    /// <summary>Internal constructor for static instantiation.</summary>
    static NetfoxLogger()
    {
        _staticLogger = (GodotObject)GD.Load<GDScript>("res://addons/netfox.internals/logger.gd").New("Static Logger", "Static Logger");
    }

    // NOTE: This could be kept public imo; using these loggers during gamedev
    // can be useful as messages contain networking data
    /// <summary>Internal constructor for instance instantiation.</summary>
    NetfoxLogger(string module, string name)
    {
        _logger = (GodotObject)GD.Load<GDScript>("res://addons/netfox.internals/logger.gd").New(module, name);
    }

    /// <summary>Creates a new logger for the netfox module.</summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>The logger instance.</returns>
    public static NetfoxLogger ForNetfox(string name) { return new("netfox", name); }
    /// <summary>Creates a new logger the netfox.noray module.</summary>
    /// <param name="name">The name of the logger.</param>
    /// <returns>The logger instance.</returns>
    public static NetfoxLogger ForNoray(string name) { return new("netfox.noray", name); }
    /// <summary>Creates a new logger that uses netfox.extras as the module.</summary>
    /// <param name="name">The name of the node this logger is used for.</param>
    /// <returns>The logger instance.</returns>
    public static NetfoxLogger ForExtras(string name) { return new("netfox.extras", name); }
    public static Dictionary MakeSetting(string name) { return (Dictionary)_staticLogger.Call(MethodNameGd.MakeSetting, name); }
    public static void RegisterTag(Callable tag, int priority = 0) { _staticLogger.Call(MethodNameGd.RegisterTag, tag, priority); }

    /// <summary><para>Logs a message as a Trace.</para>
    /// <para>Traces should be "I'm here!" messages, nothing more.</para></summary>
    /// <param name="message">The message to be logged.</param>
    public void LogTrace(string message) { _logger.Call(MethodNameGd.Trace, message, new Array()); }
    // NOTE: Debug logging is not just about showing variables
    /// <summary><para>Logs a message as a Debug.</para>
    /// <para>Debugs should be showing variables.</para></summary>
    /// <param name="message">The message to be logged.</param>
    public void LogDebug(string message) { _logger.Call(MethodNameGd.Debug, message, new Array()); }
    /// <summary><para>Logs a message as Info.</para>
    /// <para>Info should be letting you know of changes, IE "Connected to Server".</para></summary>
    /// <param name="message">The message to be logged.</param>
    public void LogInfo(string message) { _logger.Call(MethodNameGd.Info, message, new Array()); }
    /// <summary><para>Logs a message as a Warning.</para>
    /// <para>Warnings should be letting you know of issues that you can recover from.</para></summary>
    /// <param name="message">The warning to be logged.</param>
    public void LogWarning(string message) { _logger.Call(MethodNameGd.Warning, message, new Array()); }
    /// <summary><para>Logs a message as an Error.</para>
    /// <para>Errors should be letting you know of issues that you can't recover from.</para></summary>
    /// <param name="message">The error to be logged.</param>
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
