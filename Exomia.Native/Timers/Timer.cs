#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Native.Timers
{
    /// <summary>
    ///     A timer. This class cannot be inherited.
    /// </summary>
    public sealed class Timer : IDisposable
    {
        /// <summary>
        ///     called when the interval elapses.
        /// </summary>
        /// <param name="sender"> . </param>
        public delegate void ElapsedEventHandler(Timer sender);

        /// <summary>
        ///     Bitfield of flags for specifying EventType.
        /// </summary>
        [Flags]
        public enum EventType : uint
        {
            /// <summary>
            ///     Event occurs once, after uDelay milliseconds.
            /// </summary>
            Oneshot = 0,

            /// <summary>
            ///     Event occurs periodic, after uDelay milliseconds.
            /// </summary>
            Periodic = 1
        }

        /// <summary>
        ///     The winmm.
        /// </summary>
        private const string WINMM = "winmm.dll";

        /// <summary>
        ///     Occurs when the interval elapses.
        /// </summary>
        public event ElapsedEventHandler Elapsed
        {
            add { _elapsed += value; }

            // ReSharper disable once DelegateSubtraction
            remove { _elapsed -= value; }
        }

        /// <summary>
        ///     this lock.
        /// </summary>
        private readonly object _thisLock = new object();

        /// <summary>
        ///     The delay.
        /// </summary>
        private readonly uint _delay;

        /// <summary>
        ///     Type of the event.
        /// </summary>
        private readonly EventType _eventType;

        /// <summary>
        ///     The elapsed.
        /// </summary>
        private ElapsedEventHandler _elapsed;

        /// <summary>
        ///     Identifier for the timer.
        /// </summary>
        private uint _timerId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        /// <param name="delay">     The delay. </param>
        /// <param name="eventType"> (Optional) Type of the event. </param>
        public Timer(uint delay, EventType eventType = EventType.Periodic)
        {
            _delay     = delay;
            _eventType = eventType;
        }

        /// <summary>
        ///     Time begin period.
        /// </summary>
        /// <param name="uPeriod"> The period. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(WINMM, EntryPoint = "timeBeginPeriod", CharSet = CharSet.Auto)]
        private static extern int TimeBeginPeriod(uint uPeriod);

        /// <summary>
        ///     Time end period.
        /// </summary>
        /// <param name="uPeriod"> The period. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(WINMM, EntryPoint = "timeEndPeriod", CharSet = CharSet.Auto)]
        private static extern int TimeEndPeriod(uint uPeriod);

        /// <summary>
        ///     Time set event.
        /// </summary>
        /// <param name="uDelay">      The delay. </param>
        /// <param name="uResolution"> The resolution. </param>
        /// <param name="lpTimeProc">  The time proc. </param>
        /// <param name="dwUser">      The user. </param>
        /// <param name="fuEvent">     The fu event. </param>
        /// <returns>
        ///     An uint.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(WINMM, EntryPoint = "timeSetEvent", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint TimeSetEvent(uint    uDelay, uint uResolution, TimerEventHandler lpTimeProc,
                                                UIntPtr dwUser, uint fuEvent);

        /// <summary>
        ///     Time kill event.
        /// </summary>
        /// <param name="id"> The identifier. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(WINMM, EntryPoint = "timeKillEvent", CharSet = CharSet.Auto)]
        private static extern int TimeKillEvent(uint id);

        /// <summary>
        ///     Start the current timer instance.
        /// </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        public void Start()
        {
            lock (_thisLock)
            {
                if (_timerId == 0)
                {
                    TimeBeginPeriod(1);
                    _timerId = TimeSetEvent(_delay, 0, TimerCallback, UIntPtr.Zero, (uint)_eventType);
                    if (_timerId == 0)
                    {
                        throw new Exception("TimeSetEvent error");
                    }
                }
            }
        }

        /// <summary>
        ///     Stop the current timer instance (if any)
        /// </summary>
        public void Stop()
        {
            lock (_thisLock)
            {
                if (_timerId != 0)
                {
                    TimeKillEvent(_timerId);
                    TimeEndPeriod(1);
                    _timerId = 0;
                }
            }
        }

        /// <summary>
        ///     Restart the current timer instance.
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        ///     Callback, called when the timer.
        /// </summary>
        /// <param name="uTimerID"> Identifier for the timer. </param>
        /// <param name="uMsg">     The message. </param>
        /// <param name="dwUser">   The user. </param>
        /// <param name="dw1">      The first dw. </param>
        /// <param name="dw2">      The second dw. </param>
        private void TimerCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
        {
            _elapsed?.Invoke(this);
        }

        /// <summary>
        ///     Delegate for handling Timer events.
        /// </summary>
        /// <param name="uTimerID"> Identifier for the timer. </param>
        /// <param name="uMsg">     The message. </param>
        /// <param name="dwUser">   The user. </param>
        /// <param name="dw1">      The first dw. </param>
        /// <param name="dw2">      The second dw. </param>
        private delegate void TimerEventHandler(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Native.Timers.Timer and optionally
        ///     releases the managed resources.
        /// </summary>
        /// <param name="disposing"> True to release both managed and unmanaged resources; false to
        ///                          release only unmanaged resources. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) { Stop(); }
                _disposed = true;
            }
        }

        /// <inheritdoc/>
        ~Timer()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}