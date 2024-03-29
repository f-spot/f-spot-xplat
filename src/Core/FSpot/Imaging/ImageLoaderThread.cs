//
// ImageLoaderThread.cs
//
// Author:
//   Ruben Vermeersch <ruben@savanne.be>
//   Larry Ewing <lewing@novell.com>
//   Ettore Perazzoli <ettore@src.gnome.org>
//
// Copyright (C) 2003-2010 Novell, Inc.
// Copyright (C) 2009-2010 Ruben Vermeersch
// Copyright (C) 2003-2006 Larry Ewing
// Copyright (C) 2003 Ettore Perazzoli
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

using System;
using System.Collections.Generic;
using System.Threading;

using Hyena;
using SixLabors.ImageSharp;

namespace FSpot.Imaging
{
	public class ImageLoaderThread : IImageLoaderThread
	{
		static readonly List<ImageLoaderThread> instances = new List<ImageLoaderThread> ();

		/* The thread used to handle the requests.  */
		Thread worker_thread;

		/* The request queue; it's shared between the threads so it
		   needs to be locked prior to access.  */
		readonly List<RequestItem> queue;

		/* A dict of all the requests; note that the current request
		   isn't in the dict.  */
		readonly Dictionary<Uri, RequestItem> requestsByUri;

		/* Current request.  Request currently being handled by the
		   auxiliary thread.  Should be modified only by the auxiliary
		   thread (the GTK thread can only read it).  */
		RequestItem currentRequest;

		/* The queue of processed requests.  */
		readonly Queue<RequestItem> processedRequests;

		/* This is used by the helper thread to notify the main GLib
		   thread that there are pending items in the
		   `processed_requests' queue.  */
		//ThreadNotify pendingNotify;

		/* Whether a notification is pending on `pending_notify'
		   already or not.  */
		bool pendingNotifyNotified;
		volatile bool should_cancel;

		readonly IImageFileFactory imageFileFactory;

		public event ImageLoadedHandler OnPixbufLoaded;

		public ImageLoaderThread (IImageFileFactory imageFileFactory)
		{
			this.imageFileFactory = imageFileFactory;

			queue = new List<RequestItem> ();
			requestsByUri = new Dictionary<Uri, RequestItem> ();
			// requests_by_path = Hashtable.Synchronized (new Hashtable ());
			processedRequests = new Queue<RequestItem> ();

			//pendingNotify = new ThreadNotify (new Gtk.ReadyEvent (HandleProcessedRequests));

			instances.Add (this);
		}

		void StartWorker ()
		{
			if (worker_thread != null) {
				return;
			}

			should_cancel = false;
			worker_thread = new Thread (new ThreadStart (WorkerThread));
			worker_thread.Start ();
		}

		int block_count;

		public void PushBlock ()
		{
			Interlocked.Increment (ref block_count);
		}

		public void PopBlock ()
		{
			if (Interlocked.Decrement (ref block_count) == 0) {
				lock (queue) {
					Monitor.Pulse (queue);
				}
			}
		}

		public void Cleanup ()
		{
			should_cancel = true;
			if (worker_thread != null) {
				lock (queue) {
					Monitor.Pulse (queue);
				}
				worker_thread.Join ();
			}
			worker_thread = null;
		}

		public static void CleanAll ()
		{
			foreach (var thread in instances) {
				thread.Cleanup ();
			}
		}

		public void Request (Uri uri, int order)
		{
			Request (uri, order, 0, 0);
		}

		public virtual void Request (Uri uri, int order, int width, int height)
		{
			lock (queue) {
				if (InsertRequest (uri, order, width, height)) {
					Monitor.Pulse (queue);
				}
			}
		}

		public void Cancel (Uri uri)
		{
			lock (queue) {
				var r = requestsByUri [uri];
				if (r != null) {
					requestsByUri.Remove (uri);
					queue.Remove (r);
					r.Dispose ();
				}
			}
		}

		protected virtual void ProcessRequest (RequestItem request)
		{
			IImage originalImage;
			try {
				using (IImageFile img = imageFileFactory.Create (request.Uri)) {
					if (request.Width > 0) {
						originalImage = img.Load (request.Width, request.Height);
					} else {
						originalImage = img.Load ();
					}
				}
			} catch (Exception e) {
				Log.Exception (e);
				return;
			}

			if (originalImage == null) {
				return;
			}

			request.Result = originalImage;
		}

		/* Insert the request in the queue, return TRUE if the queue actually grew.
		   NOTE: Lock the queue before calling.  */
		bool InsertRequest (Uri uri, int order, int width, int height)
		{
			StartWorker ();

			/* Check if this is the same as the request currently being processed.  */
			lock (processedRequests) {
				if (currentRequest != null && currentRequest.Uri == uri) {
					return false;
				}
			}
			/* Check if a request for this path has already been queued.  */
			if (requestsByUri.TryGetValue (uri, out var existingRequest)) {
				/* FIXME: At least for now, this shouldn't happen.  */
				if (existingRequest.Order != order) {
					Log.Warning ($"BUG: Filing another request of order {order} (previously {existingRequest.Order}) for `{uri}'");
				}

				queue.Remove (existingRequest);
				queue.Add (existingRequest);
				return false;
			}

			/* New request, just put it on the queue with the right order.  */
			var new_request = new RequestItem (uri, order, width, height);

			queue.Add (new_request);

			lock (queue) {
				requestsByUri.Add (uri, new_request);
			}
			return true;
		}

		/* The worker thread's main function.  */
		void WorkerThread ()
		{
			Log.Debug (ToString (), "Worker starting");
			try {
				while (!should_cancel) {
					lock (processedRequests) {
						if (currentRequest != null) {
							processedRequests.Enqueue (currentRequest);

							if (! pendingNotifyNotified) {
								//pendingNotify.WakeupMain ();
								pendingNotifyNotified = true;
							}

							currentRequest = null;
						}
					}

					lock (queue) {

						while ((queue.Count == 0 || block_count > 0) && !should_cancel) {
							Monitor.Wait (queue);
						}

						if (should_cancel) {
							return;
						}

						int pos = queue.Count - 1;

						currentRequest = queue [pos];
						queue.RemoveAt (pos);
						requestsByUri.Remove (currentRequest.Uri);
					}

					ProcessRequest (currentRequest);
				}
			} catch (ThreadAbortException) {
				//Aborting
			}
		}

		protected virtual void EmitLoaded (Queue<RequestItem> results)
		{
			if (OnPixbufLoaded != null) {
				foreach (RequestItem r in results) {
					OnPixbufLoaded (this, r);
				}
			}
		}

		void HandleProcessedRequests ()
		{
			Queue<RequestItem> results;

			lock (processedRequests) {
				/* Copy the queued items out of the shared queue so we hold the lock for
				   as little time as possible.  */
				results = new Queue<RequestItem> (processedRequests);
				processedRequests.Clear ();

				pendingNotifyNotified = false;
			}

			EmitLoaded (results);

			foreach (RequestItem request in results){
				request.Dispose ();
			}
		}
	}
}
