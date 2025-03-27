using Godot;
using System;

namespace Netfox.Extras;

/// <summary><para>Base class for states to be used with
/// <see cref="RewindableStateMachine"/>.</para>
/// <para>Provides multiple callback methods for a state's lifecycle, which can
/// be overridden by extending classes.</para>
/// <para><b>NOTE:</b> Must have a <see cref="RewindableStateMachine"/> as a
/// parent.</para>
/// </summary>
[GlobalClass]
public abstract partial class RewindableState : Node
{
    /// <summary>The <see cref="RewindableStateMachine"/> this state belongs
    /// to.</summary>
    public RewindableStateMachine StateMachine { get; private set; }
    /// <summary><para>Callback to run a single tick.</para>
    /// <para>This method is called by the <see cref="RewindableStateMachine"/>
    /// during the rollback tick loop to update game state.</para>
    /// </summary>
    /// <param name="delta">The time delta.</param>
    /// <param name="tick">The simulated tick.</param>
    /// <param name="isFresh">Whether this is the first time this tick is being
    /// processed.</param>
    public abstract void Tick(double delta, long tick, bool isFresh);
    /// <summary><para>Callback for entering the state.</para>
    /// <para>This method is called whenever the state machine enters this
    /// state.</para>
    /// <para>It is best practice to only modify game state here, IE
    /// properties that are configured as state in a
    /// <see cref="RollbackSynchronizer"/>.</para></summary>
    /// <param name="previousState">The state that was just left.</param>
    /// <param name="tick">The simulated tick.</param>
    public abstract void Enter(RewindableState previousState, long tick);
    /// <summary><para>Callback for exiting the state.</para>
    /// <para>This method is called whenever the state machine exits this
    /// state.</para>
    /// <para>It is best practice to only modify game state here, IE
    /// properties that are configured as state in a
    /// <see cref="RollbackSynchronizer"/>.</para></summary>
    /// <param name="nextState">The state about to be entered.</param>
    /// <param name="tick">The simulated tick.</param>
    public abstract void Exit(RewindableState nextState, long tick);
    /// <summary><para>Callback for validating state transitions.</para>
    /// <para>Whenever the <see cref="RewindableStateMachine"/> attempts to
    /// enter this state, it will call this method to ensure that the
    /// transition is valid.</para>
    /// <para>If this method returns true, the transition is valid and the
    /// state machine will enter this state. Otherwise, the transition is
    /// invalid, and nothing happens.</para></summary>
    /// <param name="previousState">The state that was just left.</param>
    public abstract bool CanEnter(RewindableState previousState);
    /// <summary><para>Callback for displaying the state.</para>
    /// <para>After each tick loop, the <see cref="RewindableStateMachine"/>
    /// checks the final state, IE the state that will be active until the next
    /// tick loop. If that state has changed <b>to</b> this one, the
    /// <see cref="RewindableStateMachine"/> will call this method.</para>
    /// </summary>
    /// <param name="previousState">The state that was just left.</param>
    /// <param name="tick">The simulated tick.</param>
    public abstract void DisplayEnter(RewindableState previousState, long tick);
    /// <summary><para>Callback for displaying a different state.</para>
    /// <para>After each tick loop, the <see cref="RewindableStateMachine"/>
    /// checks the final state, IE the state that will be active until the next
    /// tick loop. If that state has changed <b>from</b> this one, the
    /// <see cref="RewindableStateMachine"/> will call this method.</para>
    /// </summary>
    /// <param name="nextState">The state about to be entered.</param>
    /// <param name="tick">The simulated tick.</param>
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
