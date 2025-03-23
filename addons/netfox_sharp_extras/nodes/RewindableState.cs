using Godot;
using System;

namespace Netfox.Extras;

[GlobalClass]
public abstract partial class RewindableState : Node
{
    public RewindableStateMachine StateMachine { get; private set; }

    public abstract void Tick(double delta, long tick, bool isFresh);
    public abstract void Enter(RewindableState previousState, long tick);
    public abstract void Exit(RewindableState nextState, long tick);
    public abstract bool CanEnter(RewindableState previousState);
    public abstract void DisplayEnter(RewindableState previousState, long tick);
    public abstract void DisplayExit(RewindableState nextState, long tick);
    public override string[] _GetConfigurationWarnings()
    {
        if (GetParent() is RewindableStateMachine)
            return Array.Empty<string>();
        return new string[] { "This state should be a child of a RewindableStateMachine." };
    }

    public override void _Notification(int what)
    {
        if (what == NotificationReady && StateMachine == null &&
            GetParent() is RewindableStateMachine parentStateMachine)
            StateMachine = parentStateMachine;
    }
}
