using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace com.services.telegram
{
	//https://stackoverflow.com/questions/4672010/multi-threading-with-net-httplistener
	internal class MultiThreadHttpServer : IDisposable
	{
		public event Action<HttpListenerContext> ProcessRequest;

		private readonly HttpListener _listener;
		private readonly Thread _listenerThread;
		private readonly Thread[] _workers;
		private readonly ManualResetEvent _stop, _ready;
		private Queue<HttpListenerContext> _queue;

		public MultiThreadHttpServer(int maxThreads)
		{
			_workers = new Thread[maxThreads];
			_queue = new Queue<HttpListenerContext>();
			_stop = new ManualResetEvent(false);
			_ready = new ManualResetEvent(false);
			_listener = new HttpListener();
			_listenerThread = new Thread(HandleRequests);
		}

		public string Start(string address, int port, string urlPath = "")
		{
			//https://stackoverflow.com/questions/9459656/difference-between-http-8080-and-http-8080
			string postfix = String.IsNullOrEmpty(urlPath) ? "" : $"{urlPath}/";
			string prefix = String.Format(@"http://{0}:{1}/{2}", address, port, postfix);
			_listener.Prefixes.Clear();
			_listener.Prefixes.Add(prefix);
			_listener.Start();
			_listenerThread.Start();

			for (int i = 0; i < _workers.Length; i++)
			{
				_workers[i] = new Thread(Worker);
				_workers[i].Start();
			}

			return prefix;
		}

		public void Dispose()
		{
			Stop();
		}

		public void Stop()
		{
			_stop.Set();
			_listenerThread.Join();

			for (int i = 0; i < _workers.Length; i++)
			{
				Thread worker = _workers[i];
				worker.Join();
			}

			_listener.Stop();
		}

		private void HandleRequests()
		{
			while (_listener.IsListening)
			{
				var context = _listener.BeginGetContext(ContextReady, null);

				if (0 == WaitHandle.WaitAny(new[] {_stop, context.AsyncWaitHandle}))
				{
					return;
				}
			}
		}

		private void ContextReady(IAsyncResult ar)
		{
			try
			{
				lock (_queue)
				{
					_queue.Enqueue(_listener.EndGetContext(ar));
					_ready.Set();
				}
			}
			catch { return; }
		}

		private void Worker()
		{
			WaitHandle[] wait = new[] { _ready, _stop };
			while (0 == WaitHandle.WaitAny(wait))
			{
				HttpListenerContext context;
				lock (_queue)
				{
					if (_queue.Count > 0)
					{
						context = _queue.Dequeue();
					}
					else
					{
						_ready.Reset();
						continue;
					}
				}

				try
				{
					ProcessRequest(context);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine(e);
				}
			}
		}
	}
}