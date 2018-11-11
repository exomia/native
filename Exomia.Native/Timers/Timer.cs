#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Runtime.InteropServices;

namespace Exomia.Native.Timers
{
    /// <inheritdoc />
    public sealed class Timer : IDisposable
    {
        /// <summary>
        ///     called when the interval elapses
        /// </summary>
        /// <param name="sender"></param>
        public delegate void ElapsedEventHandler(Timer sender);

        /// <summary>
        ///     EventType
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
        ///     Occurs when the interval elapses
        /// </summary>
        public event ElapsedEventHandler Elapsed
        {
            add { _elapsed += value; }

            // ReSharper disable once DelegateSubtraction
            remove { _elapsed -= value; }
        }

        private readonly object _thisLock = new object();

        private readonly uint _delay;
        private readonly EventType _eventType;
        private ElapsedEventHandler _elapsed;
        private uint _timerId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        public Timer(uint delay, EventType eventType = EventType.Periodic)
        {
            _delay     = delay;
            _eventType = eventType;
        }

        [DllImport(WinApi.WINMM, EntryPoint = "timeBeginPeriod", CharSet = CharSet.Auto)]
        private static extern int TimeBeginPeriod(uint uPeriod);

        [DllImport(WinApi.WINMM, EntryPoint = "timeEndPeriod", CharSet = CharSet.Auto)]
        private static extern int TimeEndPeriod(uint uPeriod);

        [DllImport(WinApi.WINMM, EntryPoint = "timeSetEvent", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint TimeSetEvent(uint uDelay, uint uResolution, TimerEventHandler lpTimeProc,
            UIntPtr dwUser, uint fuEvent);

        [DllImport(WinApi.WINMM, EntryPoint = "timeKillEvent", CharSet = CharSet.Auto)]
        private static extern int TimeKillEvent(uint id);

        /// <summary>
        ///     Start the current timer instance
        /// </summary>
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
        ///     Restart the current timer instance
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        private void TimerCallback(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2)
        {
            _elapsed?.Invoke(this);
        }

        private delegate void TimerEventHandler(uint uTimerID, uint uMsg, UIntPtr dwUser, UIntPtr dw1, UIntPtr dw2);

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) { Stop(); }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~Timer()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}