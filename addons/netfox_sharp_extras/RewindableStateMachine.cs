using Godot;
using Netfox.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Netfox.Extras;

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
            if (_currentState != null)
                return _currentState.Name;
            return "";
        }
        set => SetState(value);
    }

    [Export]
    RewindableState _currentState;

    [Signal]
    public delegate void OnStateChangedEventHandler(RewindableState oldState, RewindableState newState);

    [Signal]
    public delegate void OnDisplayStateChangedEventHandler(RewindableState oldState, RewindableState newState);

    static NetfoxLogger _logger = NetfoxLogger.ForExtras("RewindableStateMachine");

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

        if (_currentState != null)
        {
            if (!newState.CanEnter(_currentState))
                return;

            _currentState.Exit(newState, NetworkRollback.Tick);
        }

        RewindableState oldState = _currentState;
        _currentState = newState;
        EmitSignal(SignalName.OnStateChanged, oldState, newState);
        _currentState.Enter(oldState, NetworkRollback.Tick);
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
        if (_currentState != null)
            _currentState.Tick(delta, tick, isFresh);
    }

    private void AfterTickLoop()
    {
        if (_currentState != _previousStateObject)
        {
            EmitSignal(SignalName.OnDisplayStateChanged, _previousStateObject, _currentState);

            if (_previousStateObject != null)
                _previousStateObject.DisplayExit(_currentState, NetworkTime.Tick);

            _currentState.DisplayEnter(_previousStateObject, NetworkTime.Tick);
            _previousStateObject = _currentState;
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

        _currentState = _availableStates[newState];
    }
}
