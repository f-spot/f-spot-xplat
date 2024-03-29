//
// QueuePipelineElement.cs
//
// Author:
//   Aaron Bockover <abockover@novell.com>
//
// Copyright (C) 2008 Novell, Inc.
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
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Threading;
using System.Collections.Generic;

namespace Hyena.Collections
{
    class ElementProcessCanceledException : ApplicationException
    {
    }

    public abstract class QueuePipelineElement<T> where T : class
    {
        #pragma warning disable 0067
        // FIXME: This is to mute gmcs: https://bugzilla.novell.com/show_bug.cgi?id=360455
        public event EventHandler Finished;
        public event EventHandler ProcessedItem;
#pragma warning restore 0067

		readonly object monitor = new object ();
        AutoResetEvent thread_wait;
        bool processing = false;
        bool threaded = true;
        bool canceled = false;

        public int ProcessedCount { get; private set; }

        public int TotalCount { get; private set; }

        protected abstract T ProcessItem (T item);

        protected virtual void OnFinished ()
        {
            lock (this) {
                canceled = false;
            }

            lock (Queue) {
                TotalCount = 0;
                ProcessedCount = 0;
            }

			Finished?.Invoke (this, EventArgs.Empty);
		}

        protected void OnProcessedItem ()
        {
			ProcessedItem?.Invoke (this, EventArgs.Empty);
		}

        protected virtual void OnCanceled ()
        {
            lock (Queue) {
                Queue.Clear ();
                TotalCount = 0;
                ProcessedCount = 0;
            }
        }

        public virtual void Enqueue (T item)
        {
            lock (this) {
                lock (Queue) {
                    Queue.Enqueue (item);
                    TotalCount++;
                }

                if (!threaded) {
                    Processor (null);
                    return;
                }

                if (thread_wait == null) {
                    thread_wait = new AutoResetEvent (false);
                }

                if (Monitor.TryEnter (monitor)) {
                    Monitor.Exit (monitor);
                    ThreadPool.QueueUserWorkItem (Processor);
                    thread_wait.WaitOne ();
                }
            }
        }

        protected virtual void EnqueueDownstream (T item)
        {
            if (NextElement != null && item != null) {
                NextElement.Enqueue (item);
            }
        }

        void Processor (object state)
        {
            lock (monitor) {
                if (threaded) {
                    thread_wait.Set ();
                }

                lock (this) {
                    processing = true;
                }

                try {
                    while (Queue.Count > 0) {
                        CheckForCanceled ();

                        T item = null;
                        lock (Queue) {
                            item = Queue.Dequeue ();
                            ProcessedCount++;
                        }

                        EnqueueDownstream (ProcessItem (item));
                        OnProcessedItem ();
                    }
                } catch (ElementProcessCanceledException) {
                    OnCanceled ();
                }

                lock (this) {
                    processing = false;
                }

                if (threaded) {
                    thread_wait.Close ();
                    thread_wait = null;
                }

                OnFinished ();
            }
        }

        protected virtual void CheckForCanceled ()
        {
            lock (this) {
                if (canceled) {
                    throw new ElementProcessCanceledException ();
                }
            }
        }

        public void Cancel ()
        {
            lock (this) {
                if (processing) {
                    canceled = true;
                }

                if (NextElement != null) {
                    NextElement.Cancel ();
                }
            }
        }

        public bool Processing {
            get { lock (this) { return processing; } }
        }

        public bool Threaded {
            get { return threaded; }
            set {
                if (processing) {
                    throw new InvalidOperationException ("Cannot change threading model while the element is processing");
                }

                threaded = value;
            }
        }

        protected Queue<T> Queue { get; } = new Queue<T>();

		public QueuePipelineElement<T> NextElement { get; set; }
    }
}
