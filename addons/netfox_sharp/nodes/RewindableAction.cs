using Godot;
using Netfox.Logging;

namespace Netfox;

/// <summary><para>C# wrapper for Fox's Sake Studio's
/// <see href="https://github.com/foxssake/netfox/"> netfox</see> addon.</para>
/// <para>Represents actions that may or may not happen, in a way
/// compatible with rollback.</para>
/// <para><b>NOTE:</b> This class is <u><i>experimental</i></u>. While
/// experimental, breaking changes may be introduced at any time.</para>
/// </summary>
[Tool]
public partial class RewindableAction : Node
{
    /// <summary>The GDScript script used to instance RollbackSynchronizer.</summary>
    static readonly GDScript _script;
    /// <summary>The name for the internal GDScript netfox node.</summary>
    static readonly StringName _proxyName = "InternalRewindableAction";
    /// <summary>Internal reference of the RollbackSynchronizer GDScript node.</summary>
    Node _rewindableAction;

    static readonly NetfoxLogger _logger = new("NetfoxSharp", "RewindableAction");

    static RewindableAction()
    {
        _script = GD.Load<GDScript>("res://addons/netfox/rewindable-action.gd");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationReady ||
            what == NotificationEditorPreSave)
            Initialize();
    }

    private void Initialize()
    {
        _rewindableAction = (Node)_script.New();

        CallDeferred(MethodName.AddProxyNode);
    }

    private void AddProxyNode()
    {
        if (FindChild(_proxyName) != null)
            return;

        AddChild(_rewindableAction, forceReadableName: true, @internal: InternalMode.Back);
        _rewindableAction.Owner = Owner;
    }

    #region Methods
    /// <summary>Sets if the action is active for a given tick.</summary>
    /// <param name="active">If the action is active.</param>
    /// <param name="tick">The tick to set the action for.</param>
    public void SetActive(bool active, long tick = -1)
    {
        if (tick == -1)
            tick = NetworkRollback.Tick;
        _rewindableAction.Call(MethodNameGd.SetActive, active, tick);
    }
    /// <summary>Check if the action is happening for the given tick.</summary>
    public bool IsActive(long tick = -1)
    {
        if (tick == -1)
            tick = NetworkRollback.Tick;
        return (bool)_rewindableAction.Call(MethodNameGd.IsActive, tick);
    }
    /// <summary><para>Check the action's status for the given tick.</para>
    /// </summary>
    /// <param name="tick"></param>
    /// <returns><para><see cref="ActionStatus.Active"/> if the action is
    /// happening</para>
    /// <para><see cref="ActionStatus.Inactive"/> if the action is not
    /// happening.</para>
    /// <para><see cref="ActionStatus.Confirming"/> if the action was
    /// previously not happening, but now is.</para>
    /// <para><see cref="ActionStatus.Cancelling"/> if the action was
    /// previously happening, but now isn't.</para>
    /// <para><b>NOTE:</b> <see cref="ActionStatus.Confirming"/> and
    /// <see cref="ActionStatus.Cancelling"/> statuses may occur if the action
    /// was just toggled, or data was received from the action's authority.
    /// </para></returns>
    public ActionStatus GetStatus(long tick = -1)
    {
        if (tick == -1)
            tick = NetworkRollback.Tick;
        return (ActionStatus)(int)_rewindableAction.Call(MethodNameGd.GetStatus, tick);
    }
    /// <summary>Checks if the action has confirmed.</summary>
    /// <returns><see langword="true"/> if the action has been in
    /// <see cref="ActionStatus.Confirming"/> status during the last tick loop.
    /// </returns>
    public bool HasConfirmed() { return (bool)_rewindableAction.Call(MethodNameGd.HasConfirmed); }
    /// <summary>Checks if the action has cancelled.</summary>
    /// <returns><see langword="true"/> if the action has been in
    /// <see cref="ActionStatus.Cancelling"/> status during the last tick loop.
    /// </returns>
    public bool HasCancelled() { return (bool)_rewindableAction.Call(MethodNameGd.HasCancelled); }
    /// <summary>Checks if the action has context for the given tick.
    /// </summary>
    /// <returns><see langword="true"/> if the action has context for the
    /// given tick.
    /// </returns>
    public bool HasContext(long tick = -1)
    {
        if (tick == -1)
            tick = NetworkRollback.Tick;
        return (bool)_rewindableAction.Call(MethodNameGd.HasContext, tick);
    }
    /// <summary>Gets the context stored for the given tick, or null.</summary>
    /// <returns>The context stored for the given tick, or null if no context
    /// exists.</returns>
    public Variant GetContext(long tick = -1)
    {
        if (tick == -1)
            tick = NetworkRollback.Tick;
        return (Variant)_rewindableAction.Call(MethodNameGd.GetContext, tick);
    }
    /// <summary>Sets the context stored for the given tick, or null.</summary>
    public void SetContext(Variant value, long tick = -1)
    {
        if (tick == -1)
            tick = NetworkRollback.Tick;
        _rewindableAction.Call(MethodNameGd.SetContext, value, tick);
    }
    /// <summary>Erases the context for the given tick.</summary>
    public void EraseContext(long tick = -1)
    {
        if (tick == -1)
            tick = NetworkRollback.Tick;
        _rewindableAction.Call(MethodNameGd.EraseContext, tick);
    }
    /// <summary><para>Whenever the action happens, mutate the target object.
    /// </para>
    /// <para>See also <see cref="NetworkRollback.Mutate(GodotObject, long)"/>.
    /// </para>
    /// </summary>
    /// <param name="target"></param>
    public void Mutate(GodotObject target) { _rewindableAction.Call(MethodNameGd.Mutate, target); }
    /// <summary><para>Remove the target from the list of objects to mutate.
    /// </para>
    /// <para>See also <see cref="NetworkRollback.Mutate(GodotObject, long)"/>.
    /// </para>
    /// </summary>
    /// <param name="target"></param>
    public void DontMutate(GodotObject target) { _rewindableAction.Call(MethodNameGd.DontMutate, target); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            SetActive = "set_active",
            IsActive = "is_active",
            GetStatus = "get_status",
            HasConfirmed = "has_confirmed",
            HasCancelled = "has_cancelled",
            GetStatusString = "get_status_string",
            HasContext = "has_context",
            GetContext = "get_context",
            SetContext = "set_context",
            EraseContext = "erase_context",
            Mutate = "mutate",
            DontMutate = "dont_mutate";
    }
    #endregion

    /// <summary>Status of the rewindable action.</summary>
    public enum ActionStatus
    {
        Inactive,
        Confirming,
        Active,
        Cancelling
    }
}
