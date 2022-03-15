using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using com.config;
using com.main;
using com.main.application;
using com.main.logger;
using com.services.youtrack.custom.events;
using com.services.youtrack.custom.issue;

namespace com.services.telegram.youtrack
{
	internal class YouTrackIssueEventsBuffer : IDisposable
	{
		public event Action<YouTrackIssueEventsBuffer, Queue<YouTrackIssueEventBase>> OnCompleted = (YouTrackIssueEventsBuffer sender, Queue<YouTrackIssueEventBase> eventsQueue) => { };

		public YouTrackIssue FirstIssueState { get; private set; }
		public YouTrackIssue LastIssueState { get; private set; }

		private Dictionary<Type, LinkedList<YouTrackIssueEventBase>> _eventsDictionary { get; set; }
		private LinkedList<YouTrackIssueEventBase> _eventsList;

		private YouTrackBufferingConfig _bufferingConfig;
		private int _remainingMilliseconds;

		public YouTrackIssueEventsBuffer(YouTrackIssue firstState)
		{
			_bufferingConfig = App.Config.YouTrack.Buffering;

			FirstIssueState = firstState;
			LastIssueState = firstState;

			_eventsDictionary = new Dictionary<Type, LinkedList<YouTrackIssueEventBase>>();
			_eventsList = new LinkedList<YouTrackIssueEventBase>();

			TimerTask();
		}

		public void Dispose()
		{
			FirstIssueState = null;
			LastIssueState = null;

			_eventsList.Clear();
			_eventsList = null;

			_eventsDictionary.Clear();
			_eventsDictionary = null;

			_bufferingConfig = null;

			OnCompleted = null;
		}

		private async Task TimerTask()
		{
			_remainingMilliseconds = (int) (_bufferingConfig.FirstDuration * 1000);

			Stopwatch sw = new Stopwatch();
			sw.Start();

			while (_remainingMilliseconds > 0)
			{
				int currentMilliseconds = _remainingMilliseconds;
				DebugLog($"SW Start delay: {currentMilliseconds}");
				await Task.Delay(currentMilliseconds);
				_remainingMilliseconds -= currentMilliseconds;
			}

			sw.Stop();
			DebugLog($"SW Stop: {sw.ElapsedMilliseconds}");

			OnTimerElapsed();
		}

		private void OnTimerElapsed()
		{
			Queue<YouTrackIssueEventBase> result = new Queue<YouTrackIssueEventBase>(_eventsList.Count);
			foreach (YouTrackIssueEventBase issueEvent in _eventsList)
			{
				issueEvent.UpdateIssue(LastIssueState);
				result.Enqueue(issueEvent);
			}

			DebugLog($"Life timer elapsed! Events count: {_eventsList.Count}");

			OnCompleted(this, result);
		}

		public void Push<T>(T eventData) where T : YouTrackIssueEventBase
		{
			PushAndStackEvent<T>(eventData);
		}

		private void PushAndStackEvent<T>(T eventData) where T : YouTrackIssueEventBase
		{
			LastIssueState = eventData.Issue;

			Type eventType = typeof(T);
			LinkedList<YouTrackIssueEventBase> eventsForType;

			YouTrackIssueEventBase targetIssueEvent;
			if (!_eventsDictionary.ContainsKey(eventType))
			{
				targetIssueEvent = eventData;
				eventsForType = new LinkedList<YouTrackIssueEventBase>();
				eventsForType.AddLast(eventData);
				_eventsDictionary[eventType] = eventsForType;
				DebugLog($"First event at type {eventData.GetType()} added!");
			}
			else
			{
				eventsForType = _eventsDictionary[eventType];
				
				if (TryGetSimilarIssueEvent(eventsForType, eventData, out targetIssueEvent))
				{
					targetIssueEvent.Stack(eventData);
					_eventsList.Remove(targetIssueEvent);
					DebugLog($"Events at type {eventData.GetType()} stacked!");
				}
				else
				{
					targetIssueEvent = eventData;
					eventsForType.AddLast(eventData);
					DebugLog($"Some event at type {eventData.GetType()} added!");
				}
			}

			_eventsList.AddLast(targetIssueEvent);

			_remainingMilliseconds += (int) (_bufferingConfig.FollowingDuration * 1000f);

			DebugLog($"Change interval");
		}

		private bool TryGetSimilarIssueEvent(LinkedList<YouTrackIssueEventBase> events, YouTrackIssueEventBase potentialSimilarEvent, out YouTrackIssueEventBase result)
		{
			foreach (var storagedEvent in events)
			{
				if (storagedEvent.IsSimilar(potentialSimilarEvent))
				{
					result = storagedEvent;
					return true;
				}
			}

			result = null;
			return false;
		}

		private void DebugLog(string message)
		{
			if (App.Config.Logging.BufferingYoutrackNotifications)
			{
				Logger.Log($"[Buffer][{FirstIssueState.Id}][REMAINING MS: {_remainingMilliseconds}] {message}");
			}
		}
	}
}