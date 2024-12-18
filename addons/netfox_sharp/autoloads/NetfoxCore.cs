using Godot;
using Netfox.Logging;

namespace Netfox;

/// <summary>Used in place of godot autoloads for the Netfox plugin.</summary>
public partial class NetfoxCore : Node
{
    /// <summary>Reference to the Netfox autoload NetworkTime.</summary>
    public static NetworkTime NetworkTime;
    /// <summary>Reference to the Netfox autoload NetworkTimeSynchronizer.</summary>
    public static NetworkTimeSynchronizer NetworkTimeSynchronizer;
    /// <summary>Reference to the Netfox autoload NetworkRollback.</summary>
    public static NetworkRollback NetworkRollback;
    /// <summary>Reference to the Netfox autoload NetworkEvents.</summary>
    public static NetworkEvents NetworkEvents;
    /// <summary>Reference to the Netfox autoload NetworkPerformance.</summary>
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

