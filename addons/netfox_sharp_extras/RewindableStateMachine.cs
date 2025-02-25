using Godot;
using Netfox.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using Gd = Godot.Collections;

namespace Netfox.Extras;

[GlobalClass]
public partial class RewindableStateMachine : Node
{
    /// <summary><para>Name of the current state.</para>
    /// <para>Can be an empty string if no state is active. Only modify directly if you
    /// need to skip <see cref="Transition"/>'s callbacks.</para></summary>
    [Export]
    StringName State
    {
        get
        {
            if (_stateObject != null)
                return _stateObject.Name;
            return "";
        }
        set => SetState(value);
    }

    [Signal]
    public delegate void OnStateChangedEventHandler(RewindableState oldState, RewindableState newState);

    [Signal]
    public delegate void OnDisplayStateChangedEventHandler(RewindableState oldState, RewindableState newState);

    static NetfoxLogger _logger = NetfoxLogger.ForExtras("RewindableStateMachine");

    RewindableState _stateObject;
    RewindableState _previousStateObject;
    Dictionary<StringName, RewindableState> _availableStates = new();

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

        if (_stateObject != null)
        {
            if (!newState.CanEnter(_stateObject))
                return;

            _stateObject.Exit(newState, NetfoxSharp.NetworkRollback.Tick);
        }

        RewindableState oldState = _stateObject;
        _stateObject = newState;
        EmitSignal(SignalName.OnStateChanged, oldState, newState);
        _stateObject.Enter(oldState, NetfoxSharp.NetworkRollback.Tick);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationReady)
        {
            foreach (RewindableState state in GetChildren().OfType<RewindableState>())
            {
                _availableStates.Add(state.Name, state);
            }

            NetfoxSharp.NetworkTime.Connect(NetworkTime.SignalName.AfterTickLoop, Callable.From(AfterTickLoop));
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        Gd.Array<Node> nodes = GetParent().GetChildren();
        RollbackSynchronizer rollbackSync = null;

        foreach (Node node in nodes)
        {
            if (node is RollbackSynchronizer sync)
            {
                rollbackSync = sync;
                break;
            }
        }

        if (rollbackSync == null)
            return new string[] { "RewindableStateMachine is not managed by a" +
                "RollbackSynchronizer! Add it as a sibling node to fix this." };

        if (rollbackSync.Root == null)
            return new string[] { "RollbackSynchronizer configuration is invalid, " +
                "it can't manage this state machine!\nNote: You may need to reload " +
                "this scene after fixing for this warning to disappear."};

        // TODO: Add error parsing for PropertyEntry

        return Array.Empty<string>();
    }

    public void _rollback_tick(double delta, long tick, bool isFresh)
    {
        if (_stateObject != null)
            _stateObject.Tick(delta, tick, isFresh);
    }

    private void AfterTickLoop()
    {
        if (_stateObject != _previousStateObject)
        {
            EmitSignal(SignalName.OnDisplayStateChanged, _previousStateObject, _stateObject);

            if (_previousStateObject != null)
                _previousStateObject.DisplayExit(_stateObject, NetfoxSharp.NetworkTime.Tick);

            _stateObject.DisplayEnter(_previousStateObject, NetfoxSharp.NetworkTime.Tick);
            _previousStateObject = _stateObject;
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

        _stateObject = _availableStates[newState];
    }
}
