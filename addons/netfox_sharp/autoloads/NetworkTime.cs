﻿using Godot;

namespace Netfox;

/// <summary><para>C# wrapper for Fox's Sake <see href="https://github.com/foxssake/netfox/">
/// Netfox</see> addon.</para>
/// 
/// <para>Tracks shared network time between players, and provides an event loop for
/// synchronized game updates.</para>
/// 
/// <para>See the <see href="https://foxssake.github.io/netfox/netfox/guides/network-time/">
/// NetworkTime</see> Netfox guide for more information.</para></summary>
public partial class NetworkTime : Node
{
    #region Public Variables
    /// <summary>Number of ticks per second.</summary>
    public long TickRate { get { return (long)_networkTimeGd.Get(PropertyNameGd.TickRate); } }
    /// <summary><para>Whether to sync the network ticks to physics updates.</para>
    /// <para>When set to true, tickrate will be the same as the physics ticks per second, 
    /// and the network tick loop will be run inside the physics update process.</para></summary>
    public bool SyncToPhysics { get { return (bool)_networkTimeGd.Get(PropertyNameGd.SyncToPhysics); } }
    /// <summary><para>Maximum number of ticks to simulate per frame.</para>
    /// <para>If the game itself runs slower than the configured tickrate, multiple ticks
    /// will be run in a single go. However, to avoid an endless feedback loop of
    /// running too many ticks in a frame, which makes the game even slower, which
    /// results in even more ticks and so on, this setting is an upper limit on how
    /// many ticks can be simulated in a single go.</para></summary>summary>
    public long MaxTicksPerFrame { get { return (long)_networkTimeGd.Get(PropertyNameGd.MaxTicksPerFrame); } }
    /// <summary><para>Current network time in seconds.</para>
    /// <para>Time is measured from the start of NetworkTime, in practice this is often the
    /// time from the server's start.</para>
    /// <para>Use this value in cases where timestamps need to be shared with the server.</para>
    /// <para><b>NOTE:</b> Time is continuously synced with the server. If the difference 
    /// between local and server time is above a certain threshold, this value will
    /// be adjusted.</para></summary>
    public double Time { get { return (double)_networkTimeGd.Get(PropertyNameGd.Time); } }
    /// <summary><para>Current network time in ticks.</para>
    /// <para>Ticks are measured from the start of NetworkTime, in practice this is often the
    /// time from the server's start.</para>
    /// <para>Use this value in cases where timestamps need to be shared with the server.</para>
    /// <para><b>NOTE:</b> Time is continuously synced with the server. If the difference 
    /// between local and server time is above a certain threshold, this value will
    /// be adjusted.</para></summary>
    public long Tick { get { return (long)_networkTimeGd.Get(PropertyNameGd.Tick); } }
    /// <summary><para>Threshold before recalibrating <see cref="Tick"/> and <see cref="Time"/>.</para>
    /// <para>Time is continuously synced to the server. In case the time difference is 
    /// excessive between local and the server, both <see cref="Tick"/> and
    /// <see cref="Time"/> will be reset to the estimated server values.</para>
    /// <para>This property determines the difference threshold in seconds for
    /// recalibration.</para></summary>
    public double RecalibrateThreshold { get { return (long)_networkTimeGd.Get(PropertyNameGd.RecalibrateThreshold); } }
    /// <summary><para>Current network time in ticks on the server.</para>
    /// <para>This value is an estimate of the server's tick, and is regularly updated. This means that 
    /// this value can and probably will change depending on network conditions.</para></summary>
    public long RemoteTick { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteTick); } }
    /// <summary><para>Current network time in seconds on the server.</para>
    /// <para>This value is an estimate of the server's time, and is regularly updated. This means that 
    /// this value can and probably will change depending on network conditions.</para></summary>
    public double RemoteTime { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteTime); } }
    /// <summary><para>Estimated roundtrip time (ping) to server.</para>
    /// <para>This value is updated regularly, during server time sync. Latency can be 
    /// estimated as half of the roundtrip time. Returns the same as 
    /// <see cref="NetworkTimeSynchronizer.Rtt"/>.</para>
    /// <para><b>NOTE:</b> Will always be 0 on the server.</para></summary>
    public double RemoteRtt { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteRtt); } }
    /// <summary><para>Current network time in ticks.</para>
    /// <para>On clients, this value is synced to the server <b>once</b> when joining
    /// the game. After that, it will increase monotonically, incrementing every 
    /// single tick.</para>
    /// <para>When hosting, this value is simply the number of ticks since game start.</para>
    /// <para>This property can be used for things that require a timer that is guaranteed
    /// to be linear, IE no jumps in time.</para></summary>
    public long LocalTick { get { return (long)_networkTimeGd.Get(PropertyNameGd.LocalTick); } }
    /// <summary><para>Current network time in seconds.</para>
    /// <para>On clients, this value is synced to the server <b>once</b> when joining
    /// the game. After that, it will increase monotonically, incrementing every 
    /// single tick.</para>
    /// <para>When hosting, this value is simply the seconds elapsed since game start.</para>
    /// <para>This property can be used for things that require a timer that is guaranteed
    /// to be linear, IE no jumps in time.</para></summary>
    public double LocalTime { get { return (long)_networkTimeGd.Get(PropertyNameGd.LocalTime); } }
    /// <summary><para>Amount of time a single tick takes, in seconds.</para>
    /// <para>This is the inverse of tickrate.</para></summary>
    public double TickTime { get { return (long)_networkTimeGd.Get(PropertyNameGd.TickTime); } }
    /// <summary><para>Progress towards the next tick from 0 - 1, where 0 is the start
    /// of the current tick and 1 is the start of the next tick.</para></summary>
    public double TickFactor { get { return (long)_networkTimeGd.Get(PropertyNameGd.TickFactor); } }
    /// <summary><para>Multiplier to get from physics process speeds to tick speeds.</para>
    /// <para>Some methods, like CharacterBody's move_and_slide take velocity in units/sec
    /// and figure out the time delta on their own. However, they are not aware of 
    /// netfox's time, so motion is all wrong in a network tick. For example, the
    /// network ticks run at 30 fps, while the game is running at 60fps, thus 
    /// move_and_slide will also assume that it's running on 60fps, resulting in
    /// slower than expected movement.</para>
    /// <para>To circument this, you can multiply any velocities with this variable, and 
    /// get the desired speed. Don't forget to then divide by this value if it's a
    /// persistent variable ( e.g. CharacterBody's velocity ).</para>
    /// <para><b>NOTE:</b> This works correctly both in regular and in physics frames, but may
    /// yield different values.</para></summary>
    public double PhysicsFactor { get { return (long)_networkTimeGd.Get(PropertyNameGd.PhysicsFactor); } }
    /// <summary><para>The maximum clock stretch factor allowed.</para>
    /// <para>For more context on clock stretch, see [member clock_stretch_factor]. The 
    /// minimum allowed clock stretch factor is derived as 1.0 / clock_stretch_max. 
    /// Setting this to larger values will allow for quicker clock adjustment at the 
    /// cost of bigger deviations in game speed.</para></summary>
    public double ClockStretchMax { get { return (long)_networkTimeGd.Get(PropertyNameGd.ClockStretchMax); } }
    /// <summary><para>The currently used clock stretch factor.</para>
    /// <para>As the game progresses, the simulation clock may be ahead of, or behind the
    /// host's remote clock. To compensate, whenever the simulation clock is ahead of
    /// the remote clock, the game will slightly slow down, to allow the remote clock
    /// to catch up. When the remote clock is ahead of the simulation clock, the game
    /// will run slightly faster to catch up with the remote clock.</para>
    /// <para>This value indicates the current clock speed multiplier. Values over 1.0 
    /// indicate speeding up, under 1.0 indicate slowing down.</para>
    /// <para>See <see cref="ClockStretchMax"/> for more clock stretch bounds.</para>
    /// <para>See <see cref="ClockStretchFactor"/> for more on the simulation clock.</para></summary>
    public double ClockStretchFactor { get { return (long)_networkTimeGd.Get(PropertyNameGd.ClockStretchFactor); } }
    /// <summary><para>The current estimated offset between the reference clock and the simulation
    /// clock.</para>
    /// <para>Positive values mean the simulation clock is behind, and needs to run
    /// slightly faster to catch up. Negative values mean the simulation clock is
    /// ahead, and needs to slow down slightly.</para>
    /// <para>See <see cref="ClockStretchFactor"/> for more clock speed adjustment.</para></summary>
    public double ClockOffset { get { return (long)_networkTimeGd.Get(PropertyNameGd.ClockOffset); } }
    /// <summary>The current estimated offset between the reference clock and the remote
    /// clock.
    /// <para>Positive values mean the reference clock is behind the remote clock,
    /// Negative values mean the reference clock is ahead of the remote clock.</para>
    /// <para>Returns the same as <see cref="NetworkTimeSynchronizer.RemoteOffset"/>.</para></summary>
    public double RemoteClockOffset { get { return (long)_networkTimeGd.Get(PropertyNameGd.RemoteClockOffset); } }
    #endregion

    /// <summary>Internal reference of the NetworkTime GDScript autoload.</summary>
    GodotObject _networkTimeGd;

    /// <summary>Internal constructor used by <see cref="NetfoxCore"/>. Should not be used elsewhere.</summary>
    /// <param name="networkTimeGd">The NetworkRollback GDScript autoload.</param>
    public NetworkTime(GodotObject networkTimeGd)
    {
        _networkTimeGd = networkTimeGd;

        _networkTimeGd.Connect(SignalNameGd.BeforeTickLoop, Callable.From(() => EmitSignal(SignalName.BeforeTickLoop)));
        _networkTimeGd.Connect(SignalNameGd.BeforeTick, Callable.From((double delta, long tick) => EmitSignal(SignalName.BeforeTick, delta, tick)));
        _networkTimeGd.Connect(SignalNameGd.OnTick, Callable.From((double delta, long tick) => EmitSignal(SignalName.OnTick, delta, tick)));
        _networkTimeGd.Connect(SignalNameGd.AfterTick, Callable.From((double delta, long tick) => EmitSignal(SignalName.AfterTick, delta, tick)));
        _networkTimeGd.Connect(SignalNameGd.AfterTickLoop, Callable.From(() => EmitSignal(SignalName.AfterTickLoop)));
        _networkTimeGd.Connect(SignalNameGd.AftereSync, Callable.From(() => EmitSignal(SignalName.AfterSync)));
        _networkTimeGd.Connect(SignalNameGd.AfterClientSync, Callable.From((long peerId) => EmitSignal(SignalName.AfterClientSync, peerId)));
    }

    #region Signals
    /// <summary>Emitted before a tick loop is run.</summary>
    [Signal]
    public delegate void BeforeTickLoopEventHandler();
    /// <summary>Emitted before a tick is run.</summary>
    /// <param name="delta">The time delta.</param>
    /// <param name="tick">The tick to be run.</param>
    [Signal]
    public delegate void BeforeTickEventHandler(double delta, long tick);
    /// <summary>Emitted for every network tick.</summary>
    /// <param name="delta">The time delta.</param>
    /// <param name="tick">The tick to be run.</param>
    [Signal]
    public delegate void OnTickEventHandler(double delta, long tick);
    /// <summary>Emitted after every network tick.</summary>
    /// <param name="delta">The time delta.</param>
    /// <param name="tick">The tick to be run.</param>
    [Signal]
    public delegate void AfterTickEventHandler(double delta, long tick);
    /// <summary>Emitted after the tick loop is run.</summary>
    [Signal]
    public delegate void AfterTickLoopEventHandler();
    /// <summary><para>Emitted after time is synchronized.</para>
    /// <para>This happens once the NetworkTime is started, and the first time sync process
    /// concludes. When running as server, this is emitted instantly after started.</para></summary>
    [Signal]
    public delegate void AfterSyncEventHandler();
    /// <summary><para>Emitted after a client synchronizes their time.</para>
    /// <para>This is only emitted on the server, and is emitted when the client concludes
    /// their time sync process. This is useful as this event means that the client
    /// is ticking and gameplay has started on their end.</para></summary>
    [Signal]
    public delegate void AfterClientSyncEventHandler(long peerId);
    #endregion

    #region Methods
    /// <summary><para>Starts NetworkTime.</para>
    /// <para>Once this is called, time will be synchronized and ticks will be consistently
    /// emitted.</para>
    /// <para>On clients, the initial time sync must complete before any ticks are emitted.</para>
    /// <para>To check if this initial sync is done, see <see cref="IsInitialSyncDone"/>. If
    /// you need a signal, see <see cref="AfterSync"/>.</para></summary>
    public void Start() { _networkTimeGd.Call(MethodNameGd.Start); }
    /// <summary><para>Stops NetworkTime.</para>
    /// <para>This will stop the time sync in the background, and no more ticks will be 
    /// emitted until the next start.</para></summary>
    public void Stop() { _networkTimeGd.Call(MethodNameGd.Stop); }
    /// <summary>Check if the initial time sync is done.</summary>
    /// <returns>Whether the initial sync is done.</returns>
    public bool IsInitialSyncDone() { return (bool)_networkTimeGd.Call(MethodNameGd.IsInitialSyncDone); }
    /// <summary><para>Check if client's time sync is complete.</para>
    /// <para><b>NOTE: </b> Using this from a client is considered an error.</para></summary>
    /// <param name="peerId">The id of the client to check.</param>
    /// <returns>Whether the client's time sync is complete.</returns>
    public bool IsClientSynced(long peerId) { return (bool)_networkTimeGd.Call(MethodNameGd.IsClientSynced, peerId); }
    /// <summary>Converts a duration of ticks to seconds.</summary>
    /// <param name="ticks">The number of ticks to convert.</param>
    /// <returns>The number of seconds the specified ticks represent.</returns>
    public double TicksToSeconds(long ticks) { return (double)_networkTimeGd.Call(MethodNameGd.TicksToSeconds, ticks); }
    /// <summary>Converts a duration of seconds to ticks.</summary>
    /// <param name="ticks">The number of seconds to convert.</param>
    /// <returns>The number of ticks the specified seconds represent.</returns>
    public long SecondsToTicks(double seconds) { return (long)_networkTimeGd.Call(MethodNameGd.SecondsToTicks, seconds); }
    /// <summary>Calculate the duration between two ticks in seconds.</summary>
    /// <param name="fromTick">The tick to start counting from.</param>
    /// <param name="toTick">The tick to count to.</param>
    /// <returns>The difference in seconds between the values. Returns a negative number if
    /// toTick is smaller than fromTick.</returns>
    public double SecondsBetween(long fromTick, long toTick) { return (double)_networkTimeGd.Call(MethodNameGd.SecondsBetween, fromTick, toTick); }
    /// <summary>Calculate the duration between two times in ticks.</summary>
    /// <param name="fromSecond">The second to start counting from.</param>
    /// <param name="toSecond">The second to count to.</param>
    /// <returns>The difference in ticks between the values. Returns a negative number if
    /// toSecond is smaller than fromSecond.</returns>
    public long TicksBetween(double fromSecond, double toSecond) { return (long)_networkTimeGd.Call(MethodNameGd.TicksBetween, fromSecond, toSecond); }
    #endregion

    #region StringName Constants
    static class MethodNameGd
    {
        public static readonly StringName
            Start = "start",
            Stop = "stop",
            IsInitialSyncDone = "is_initial_sync_done",
            IsClientSynced = "is_client_synced",
            TicksToSeconds = "ticks_to_seconds",
            SecondsToTicks = "seconds_to_ticks",
            SecondsBetween = "seconds_between",
            TicksBetween = "ticks_between";

    }
    static class PropertyNameGd
    {
        public static readonly StringName
            TickRate = "tickrate",
            SyncToPhysics = "sync_to_physics",
            MaxTicksPerFrame = "max_ticks_per_frame",
            Time = "time",
            Tick = "tick",
            RecalibrateThreshold = "recalibrate_threshold",
            RemoteTick = "remote_tick",
            RemoteTime = "remote_time",
            RemoteRtt = "remote_rtt",
            LocalTick = "local_tick",
            LocalTime = "local_time",
            TickTime = "ticktime",
            TickFactor = "tick_Factor",
            PhysicsFactor = "physics_factor",
            ClockStretchMax = "clock_stretch_max",
            ClockStretchFactor = "clock_stretch_factor",
            ClockOffset = "clock_offset",
            RemoteClockOffset = "remote_clock_offset";
    }
    static class SignalNameGd
    {
        public static readonly StringName
            BeforeTickLoop = "before_tick_loop",
            BeforeTick = "before_tick",
            OnTick = "on_tick",
            AfterTick = "after_tick",
            AfterTickLoop = "after_tick_loop",
            AftereSync = "after_sync",
            AfterClientSync = "after_client_sync";
    }
    #endregion
}
