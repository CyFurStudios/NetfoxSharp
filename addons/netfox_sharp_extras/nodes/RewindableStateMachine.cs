using Godot;
using Netfox.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Netfox.Extras;

/// <summary><para>A state machine that can be used with rollback.</para>
/// <para>It relies on <see cref="RollbackSynchronizer"/> to manage its state.
/// State transitions are only triggered by gameplay code, and not by rollback
/// reverting to an earlier state.</para>
/// <para>For this node to work correctly, a <see cref="RollbackSynchronizer"/>
/// must be added as a sibling, and it must have the
/// <see cref="CurrentState"/>'s property configured as a state property.
/// </para>
/// <para>To implement states, extend the <see cref="RewindableState"/> class
/// and add it as a child node.</para></summary>
[Tool]
[GlobalClass]
public partial class RewindableStateMachine : Node
{
    /// <summary><para>Name of the current state.</para>
    /// <para>Can be an empty string if no state is active. Only modify directly if you
    /// need to skip <see cref="Transition"/>'s callbacks.</para></summary>
    StringName State
    {
        get
        {
            if (CurrentState != null)
                return CurrentState.Name;
            return "";
        }
        set => SetState(value);
    }

    /// <summary><para>Current state.</para>
    /// <para>Can be null if no state is active.</para></summary>
    [Export]
    RewindableState CurrentState;

    /// <summary><para>Emitted during state transitions.</para>
    /// <para>This signal can be used to run gameplay code on state changes.
    /// </para>
    /// <para>This signal is emitted whenever a transition happens during
    /// rollback, which means it may be emitted multiple times for the same
    /// transition if it gets resimulated during rollback.</para></summary>
    /// <param name="oldState">The state that was just left.</param>
    /// <param name="newState">The state that was just entered.</param>
    [Signal]
    public delegate void OnStateChangedEventHandler(RewindableState oldState, RewindableState newState);
    /// <summary><para>Emitted after the displayed state has changed.</para>
    /// <para>This signal can be used to update visuals on state changes.
    /// </para>
    /// <para>This signal is emitted whenever the state after a tick loop has
    /// changed.</para></summary>
    /// <param name="oldState">The state that was just left.</param>
    /// <param name="newState">The state that was just entered.</param>
    [Signal]
    public delegate void OnDisplayStateChangedEventHandler(RewindableState oldState, RewindableState newState);

    static NetfoxLogger _logger = NetfoxLogger.ForExtras("RewindableStateMachine");

    RewindableState _previousStateObject;
    Dictionary<StringName, RewindableState> _availableStates = new();


## Upon transitioning, [method RewindableState.exit] is called on the old state,
## and [method RewindableState.enter] is called on the new state. In addition,
## [signal on_state_changed] is emitted.
## [br][br]
## Does nothing if transitioning to the currently active state. Emits a warning
## and does nothing when transitioning to an unknown state.
    /// <summary><para>Transitions to a new state.</para>
    /// <para>Finds the given state by name and transitions to it if possible.
    /// The new state's <see cref="RewindableState.CanEnter(RewindableState)"/>
    /// callback decides if it can be entered from the current state.</para>
    /// <para>Upon transitioning, [method RewindableState.exit] is called on
    /// the old state, and
    /// <see cref="RewindableState.Enter(RewindableState, long)"/> is called on
    /// the new state. In addition, <see cref="OnStateChanged"/> is emitted.
    /// </para></summary>
    /// <param name="newStateName">The name of the new state to enter.</param>
    public void Transition(StringName newStateName)
    {
        if (State.Equals(newStateName))
            return;

        if (!_availableStates.ContainsKey(newStateName))
        {
            _logger.LogWarning($"Attempted to transition from state {State} to" +
                $"unknown state {newStateName}");
            return;
        }

        RewindableState newState = _availableStates[newStateName];

        if (CurrentState != null)
        {
            if (!newState.CanEnter(CurrentState))
                return;

            CurrentState.Exit(newState, NetworkRollback.Tick);
        }

        RewindableState oldState = CurrentState;
        CurrentState = newState;
        EmitSignal(SignalName.OnStateChanged, oldState, newState);
        CurrentState.Enter(oldState, NetworkRollback.Tick);
    }

    public override void _Notification(int what)
    {
        if (Engine.IsEditorHint())
            return;

        if (what == NotificationReady)
        {
            foreach (RewindableState state in GetChildren().OfType<RewindableState>())
            {
                if (!_availableStates.ContainsKey(state.Name))
                    _availableStates.Add(state.Name, state);
            }

            NetfoxSharp.NetworkTime.Connect(NetworkTime.SignalName.AfterTickLoop, Callable.From(AfterTickLoop));
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        IEnumerable<RollbackSynchronizer> nodes = Owner.FindChildren("*").OfType<RollbackSynchronizer>();
        int rollbackNodes = 0;

        foreach (Node node in nodes)
            if (node.Owner == Owner)
                rollbackNodes++;

        List<string> warnings = new();

        if (rollbackNodes == 0)
            return ["This node is not managed by a RollbackSynchronizer!"];
        if (rollbackNodes > 1)
            warnings.Add("Multiple RollbackSynchronizers detected!");
        if (nodes.First().Root == null)
            warnings.Add("RollbackSynchronizer configuration is invalid, " +
                "it can't manage this state machine!\nNote: You may need to reload " +
                "this scene after fixing for this warning to disappear.");

        return warnings.ToArray();
    }

    public void _rollback_tick(double delta, long tick, bool isFresh)
    {
        if (CurrentState != null)
            CurrentState.Tick(delta, tick, isFresh);
    }

    private void AfterTickLoop()
    {
        if (CurrentState != _previousStateObject)
        {
            EmitSignal(SignalName.OnDisplayStateChanged, _previousStateObject, CurrentState);

            if (_previousStateObject != null)
                _previousStateObject.DisplayExit(CurrentState, NetworkTime.Tick);

            CurrentState.DisplayEnter(_previousStateObject, NetworkTime.Tick);
            _previousStateObject = CurrentState;
        }
    }

    private void SetState(string newState)
    {
        if (string.IsNullOrWhiteSpace(newState))
            return;

        if (!_availableStates.ContainsKey(newState))
        {
            _logger.LogWarning($"Attempted to set unknown state: {newState}");
            return;
        }

        CurrentState = _availableStates[newState];
    }
}
