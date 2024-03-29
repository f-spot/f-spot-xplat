//
// Animation.cs
//
// Author:
//   Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (C) 2009 Novell, Inc.
// Copyright (C) 2009 Stephane Delcroix
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// TODO: send the progresschanged event
// TODO: Use IProgress/Progress instead


using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace FSpot.Bling
{
	public abstract class Animation<T>
	{
		enum AnimationState {
			NotRunning = 0,
			Running,
			Paused,
		}

		TimeSpan pausedAfter;
		DateTimeOffset startTime;
		readonly Action<T> action;
		AnimationState state;
		CancellationTokenSource cts = new CancellationTokenSource();

		protected Animation ()
		{
			From = default;
			To = default;
			Duration = TimeSpan.Zero;
			action = null;
			state = AnimationState.NotRunning;
		}

		protected Animation (T from, T to, TimeSpan duration, Action<T> action)
		{
			From = from;
			To = to;
			Duration = duration;
			this.action = action;
			state = AnimationState.NotRunning;
		}

		public void Pause ()
		{
			if (state == AnimationState.Paused)
				return;

			if (state != AnimationState.NotRunning)
				throw new InvalidOperationException ("Can't Pause () a non running animation.");

			cts.Cancel ();
			pausedAfter = DateTimeOffset.Now - startTime;
			state = AnimationState.Paused;
		}

		public void Resume ()
		{
			if (state == AnimationState.Running)
				return;

			if (state != AnimationState.Paused)
				throw new InvalidOperationException ("Can't Resume () a non running animation.");

			startTime = DateTimeOffset.Now - pausedAfter;
			state = AnimationState.Running;
			cts = new CancellationTokenSource ();
			UpdateProgress (cts.Token);
		}

		public async void Start ()
		{
			if (state != AnimationState.NotRunning)
				throw new InvalidOperationException ("Can't Start () a running or paused animation.");
			startTime = DateTimeOffset.Now;
			state = AnimationState.Running;
			await UpdateProgress (cts.Token);
		}

		public void Stop ()
		{
			if (state == AnimationState.NotRunning)
				return;

			cts.Cancel ();
			action (To);
			Completed?.Invoke (this, EventArgs.Empty);
			state = AnimationState.NotRunning;
		}

		public void Restart ()
		{
			if (state == AnimationState.NotRunning) {
				Start ();
				return;
			}

			if (state == AnimationState.Paused) {
				Resume ();
				return;
			}

			startTime = DateTimeOffset.Now;
		}

		protected virtual double Ease (double normalizedTime)
		{
			return normalizedTime;
		}

		async Task UpdateProgress (CancellationToken token)
		{
			double percentage = 0.0;
			while (percentage < 1.0) {
				token.ThrowIfCancellationRequested ();

				percentage = (double)(DateTimeOffset.Now - startTime).Ticks / (double)Duration.Ticks;
				ProgressChanged?.Invoke (this, new ProgressChangedEventArgs ((int)(100 * percentage), null));

				action (Interpolate (From, To, Ease (Math.Min (percentage, 1.0))));
				await Task.Delay (40, token);
			}

			if (percentage >= 1.0) {
				Completed?.Invoke (this, EventArgs.Empty);
				state = AnimationState.NotRunning;
			}
		}

		protected abstract T Interpolate (T from, T to, double progress);

// Do not error out on this not being used
#pragma warning disable 67
		public event EventHandler Completed;
#pragma warning restore 67
		public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

		public bool IsRunning => state == AnimationState.Running;

		public bool IsPaused => state == AnimationState.Paused;

		public TimeSpan Duration { get; set; }

		public T From { get; set; }

		public T To { get; set; }
	}
}
