using Godot;
using Netfox.Logging;

namespace Netfox;

public partial class NetfoxCore : Node
{
    public static NetworkTime NetworkTime;
    public static NetworkTimeSynchronizer NetworkTimeSynchronizer;
    public static NetworkRollback NetworkRollback;
    public static NetworkEvents NetworkEvents;
    public static NetworkPerformance NetworkPerformance;

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

