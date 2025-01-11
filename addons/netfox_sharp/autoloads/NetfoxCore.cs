using Godot;
using Netfox.Logging;

namespace Netfox;

/// <summary>Used in place of godot autoloads for the netfox plugin.</summary>
public partial class NetfoxCore : Node
{
    /// <summary>Reference to the netfox autoload NetworkTime.</summary>
    public static NetworkTime NetworkTime;
    /// <summary>Reference to the netfox autoload NetworkTimeSynchronizer.</summary>
    public static NetworkTimeSynchronizer NetworkTimeSynchronizer;
    /// <summary>Reference to the netfox autoload NetworkRollback.</summary>
    public static NetworkRollback NetworkRollback;
    /// <summary>Reference to the netfox autoload NetworkEvents.</summary>
    public static NetworkEvents NetworkEvents;
    /// <summary>Reference to the netfox autoload NetworkPerformance.</summary>
    public static NetworkPerformance NetworkPerformance;
    /// <summary>An instance of NetfoxLogger.</summary>
    public static NetfoxLogger Logger;

    public override void _EnterTree()
    {
        // TODO: Test to see if we need to AddChild() to get signals working.
        NetworkTime = new(GetNode("/root/NetworkTime"));
        NetworkTimeSynchronizer = new(GetNode("/root/NetworkTimeSynchronizer"));
        NetworkRollback = new(GetNode("/root/NetworkRollback"));
        NetworkEvents = new(GetNode("/root/NetworkEvents"));
        NetworkPerformance = new(GetNode("/root/NetworkPerformance"));
        Logger = NetfoxLogger.ForNetfox("NetfoxCore");
    }
}

