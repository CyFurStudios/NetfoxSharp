using Godot;
using Gd = Godot.Collections;

namespace Netfox;

/// <summary><para><b>NOTE:</b> For internal use only.</para>
/// 
/// <para>C# wrapper for Fox's Sake Studio's
/// <see href="https://github.com/foxssake/netfox/">netfox</see> addon.</para>
/// 
/// <para>Similar in how <see cref="MultiplayerSynchronizer"/> handles visibility. It decides peer 
/// visibility based on individual overrides and filters.</para>
/// 
/// <para>By default, each peer's visibility is determined by  <see cref="DefaultVisibility"/>
/// </para>
/// 
/// <para>The default visibility can be overridden for individual peers using 
/// <see cref="SetVisibilityFor"/> and <see cref="UnsetVisibilityFor"/>.</para>
/// 
/// <para>Individual overrides can still be rejected by <i>filters</i>, which are callables that 
/// can dynamically determine the visibility for each peer. If any of the registered filters return
/// <see langword="false"/>, the peer will not be visible. Filters can be managed using 
/// <see cref="AddVisibilityFilter"/> and <see cref="RemoveVisibilityFilter"/>.</para>
/// 
/// <para>To avoid taking up too much CPU time, visibilities are only recalculated on a peer join 
/// or peer leave event by default. This can be changed by setting <see cref="UpdateModes"/>.
/// Visibilities can also be manually updated using <see cref="UpdateVisibility"/>.</para>
/// </summary>
public partial class PeerVisibilityFilter : Node
{
	/// <summary>Contains different options for when to automatically update visibility.</summary>
	public enum UpdateModes
	{
		/// <summary>Only update visibility when manually triggered.</summary>
		Never = 0,
		/// <summary>Update visibility when a peer joins or leaves.</summary>
		OnPeer = 1,
		/// <summary>Update visibility before each tick loop.</summary>
		PerTickLoop = 2,
		/// <summary>Update visibility before each network tick.</summary>
		PerTick = 3,
		/// <summary>Update visibility after each rollback tick.</summary>
		PerRollbackTick = 4
	}

	#region Public Variables
	/// <summary>Make all peers visible by default if true.</summary>
	public bool DefaultVisibility
	{
		get
		{
			if (_peerVisibilityFilter == null)
				return _defaultVisibility;
			else
				return (bool)_peerVisibilityFilter.Get(PropertyNameGd.DefaultVisibility);
		}
		set
		{
			_defaultVisibility = value;
			_peerVisibilityFilter?.Set(PropertyNameGd.DefaultVisibility, _defaultVisibility);
		}
	}
	private bool _defaultVisibility = true;
	/// <summary>Sets whether and when automatic visibility updates should happen.</summary>
	public UpdateModes UpdateMode
	{
		get
		{
			if (_peerVisibilityFilter == null)
				return _updateMode;
			else
				return (UpdateModes)(int)_peerVisibilityFilter.Get(PropertyNameGd.UpdateMode);
		}
		set
		{
			_updateMode = value;
			_peerVisibilityFilter?.Set(PropertyNameGd.UpdateMode, (int)_updateMode);
		}
	}
	private UpdateModes _updateMode = UpdateModes.OnPeer;
	#endregion

	private Node _peerVisibilityFilter;

	internal void AssignVisibilityFilter(Node peerVisibilityFilter)
	{
		_peerVisibilityFilter = peerVisibilityFilter;
		_peerVisibilityFilter.Set(PropertyNameGd.UpdateMode, (int)_updateMode);
		_peerVisibilityFilter.Set(PropertyNameGd.DefaultVisibility, _defaultVisibility);
	}

	#region Methods
	/// <summary>
	/// <para>Register a visibility filter.</para>
	/// <para>The <see href="filter"/> must take a single <see langword="long"/> representing the 
	/// peer ID as a parameter, and return <see langword="true"/> if the given peer should be 
	/// visible. The same <see href="filter"/> won't be added multiple times.</para></summary>
	/// <param name="filter">The filter to add.</param>
	public void AddVisibilityFilter(Callable filter) =>
		_peerVisibilityFilter.Call(MethodNameGd.AddVisibilityFilter, filter);
	/// <summary>
	/// <para>Remove a visibility filter.</para>
	/// <para>If the visibility filter wasn't already registered, nothing happens.</para></summary>
	/// <param name="filter">The filter to remove.</param>
	public void RemoveVisibilityFilter(Callable filter) =>
		_peerVisibilityFilter.Call(MethodNameGd.RemoveVisibilityFilter, filter);
	/// <summary><para>Remove all previously registered visibility filters.</para></summary>
	public void ClearVisibilityFilters() =>
		_peerVisibilityFilter.Call(MethodNameGd.ClearVisibilityFilters);
	/// <summary>Gets the visibility for the specified peer.</summary>
	/// <param name="peer">The peer ID.</param>
	/// <returns><see langword="true"/> if the peer is visible.</returns>
	public bool GetVisibilityFor(long peer) =>
		(bool)_peerVisibilityFilter.Call(MethodNameGd.GetVisibilityFor, peer);
	/// <summary>Set visibility override for a given <see href="peer"/>.</summary>
	/// <param name="peer">The peer ID to override.</param>
	/// <param name="visibility">The value to override.</param>
	/// <returns><see langword="true"/> if the peer is visible.</returns>
	public void SetVisibilityFor(long peer, bool visibility) =>
		_peerVisibilityFilter.Call(MethodNameGd.SetVisibilityFor, peer, visibility);
	/// <summary>Unset visibility override for a given <see href="peer"/>.</summary>
	/// <param name="peer">The peer ID to remove the override of.</param>
	public void UnsetVisibilityFor(long peer) =>
		_peerVisibilityFilter.Call(MethodNameGd.UnsetVisibilityFor, peer);
	/// <summary>Recalculates visibility for each known peer.</summary>
	/// <param name="peers">The list of peers to update the visibility of.</param>
	public void UpdateVisibility(Gd.Array<int> peers) =>
		_peerVisibilityFilter.Call(MethodNameGd.UpdateVisibility, peers);
	/// <summary><para>Return a list of visible peers.</para>
	/// 
	/// <para>This list is only recalculated when <see cref="UpdateVisibility"/> runs, either by 
	/// calling it manually, or via <see cref="UpdateMode"/>.</para></summary>
	/// <returns>List of peers that are currently visible.</returns>
	public Gd.Array<int> GetVisiblePeers() =>
		(Gd.Array<int>)_peerVisibilityFilter.Call(MethodNameGd.GetVisiblePeers);
	/// <summary><para>Return a list of visible peers for use with RPCs.</para>
	/// <para>In contrast to <see href="GetVisiblePeers"/>, this method will utilize Godot's RPC 
	/// target peer rules to produce a shorter list if possible. For example, if all peers are 
	/// visible, it will simply return 0, indicating a broadcast.</para>
	/// <para>This list will never explicitly include the local peer.</para></summary>
	/// <returns></returns>
	public Gd.Array<int> GetRpcTargetPeers() =>
		(Gd.Array<int>)_peerVisibilityFilter.Call(MethodNameGd.GetRpcTargetPeers);
	/// <summary>Sets the update mode.</summary>
	/// <param name="mode">The new update mode.</param>
	public void SetUpdateMode(UpdateModes mode) => UpdateMode = mode;
	/// <summary>Gets the update mode.</summary>
	/// <returns>The new update mode.</returns>
	public UpdateModes GetUpdateMode() => UpdateMode;
	#endregion

	#region StringName Constants
	static class MethodNameGd
	{
		public static readonly StringName
			AddVisibilityFilter = "add_visibility_filter",
			RemoveVisibilityFilter = "remove_visibility_filter",
			ClearVisibilityFilters = "clear_visibility_filters",
			GetVisibilityFor = "get_visibility_for",
			SetVisibilityFor = "set_visibility_for",
			UnsetVisibilityFor = "unset_visibility_for",
			UpdateVisibility = "update_visibility",
			GetVisiblePeers = "get_visible_peers",
			GetRpcTargetPeers = "get_rpc_target_peers";
	}
	static class PropertyNameGd
	{
		public static readonly StringName
			Name = "name",
			DefaultVisibility = "default_visibility",
			UpdateMode = "update_mode";
	}
	#endregion
}
