using System;
using System.Collections.Generic;
using com.main;
using com.main.application;
using com.main.logger;
using com.services.youtrack;
using com.services.youtrack.custom.events;

namespace com.services.telegram.youtrack
{
	internal class YouTrackIssuesEventsBuffer
	{
		public event Action<YouTrackIssueSimpleFieldChangedEvent> OnIssueSimpleFieldChangeEventReceived = (YouTrackIssueSimpleFieldChangedEvent eventData) => { };
		public event Action<YouTrackIssueAssigneeChangedEvent> OnIssueAssigneeChangeEventReceived = (YouTrackIssueAssigneeChangedEvent eventData) => { };
		public event Action<YouTrackIssueSummaryChangedEvent> OnIssueSummaryChangeEventReceived = (YouTrackIssueSummaryChangedEvent eventData) => { };
		public event Action<YouTrackIssueDescriptionChangedEvent> OnIssueDescriptionChangeEventReceived = (YouTrackIssueDescriptionChangedEvent eventData) => { };
		public event Action<YouTrackIssueCreateEvent> OnIssueCreateEventReceived = (YouTrackIssueCreateEvent eventData) => { };
		public event Action<YouTrackIssueLinksAddedEvent> OnIssueLinksAddedEventReceived = (YouTrackIssueLinksAddedEvent eventData) => { };
		public event Action<YouTrackIssueLinksRemovedEvent> OnIssueLinksRemovedEventReceived = (YouTrackIssueLinksRemovedEvent eventData) => { };
		public event Action<YouTrackIssueCommentsEvent> OnIssueCommentsAddedEventReceived = (YouTrackIssueCommentsEvent eventData) => { };

		public event Action<YouTrackIssueSimpleFieldChangedEvent> OnIssueStateBecomeTestReceived = (YouTrackIssueSimpleFieldChangedEvent eventData) => { };

		private YouTrackNotificationsReceiver _receiver;
		private Dictionary<string, YouTrackIssueEventsBuffer> _issuesBuffers;

		public YouTrackIssuesEventsBuffer(YouTrackNotificationsReceiver receiver)
		{
			_issuesBuffers = new Dictionary<string, YouTrackIssueEventsBuffer>();

			_receiver = receiver;
			_receiver.OnIssueAssigneeChangeEventReceived += OnSomeEventReceived;
			_receiver.OnIssueSummaryChangeEventReceived += OnSomeEventReceived;
			_receiver.OnIssueDescriptionChangeEventReceived += OnSomeEventReceived;
			_receiver.OnIssueSimpleFieldChangeEventReceived += OnSomeEventReceived;
			_receiver.OnIssueCreateEventReceived += OnSomeEventReceived;
			_receiver.OnIssueLinksAddedEventReceived += OnSomeEventReceived;
			_receiver.OnIssueLinksRemovedEventReceived += OnSomeEventReceived;
			_receiver.OnIssueCommentsAddedEventReceived += OnSomeEventReceived;
		}

		private void OnSomeEventReceived(YouTrackIssueEventBase eventData)
		{
			DebugLog($"Handle input event: {eventData.GetType()}");
			GetIssueBuffer(eventData).Push(eventData);
		}

		private YouTrackIssueEventsBuffer GetIssueBuffer(YouTrackIssueEventBase issueEvent)
		{
			YouTrackIssueEventsBuffer issueEventsBuffer;

			lock (_issuesBuffers)
			{
				if (!_issuesBuffers.TryGetValue(issueEvent.Issue.Id, out issueEventsBuffer))
				{
					issueEventsBuffer = new YouTrackIssueEventsBuffer(issueEvent.Issue);
					issueEventsBuffer.OnCompleted += OnIssueEventsBufferCompleted;
					_issuesBuffers[issueEvent.Issue.Id] = issueEventsBuffer;
				}
			}

			return issueEventsBuffer;
		}

		private void OnIssueEventsBufferCompleted(YouTrackIssueEventsBuffer sender, Queue<YouTrackIssueEventBase> eventsQueue)
		{
			while (eventsQueue.Count > 0)
			{
				YouTrackIssueEventBase bufferedEvent = eventsQueue.Dequeue();

				//костыль...
				YouTrackIssueSimpleFieldChangedEvent fieldChangeEvent = bufferedEvent as YouTrackIssueSimpleFieldChangedEvent;
				bool isStateBecomeTest = fieldChangeEvent != null && fieldChangeEvent.FieldName == "State" && fieldChangeEvent.NewValue == "Test";
				if (isStateBecomeTest)
				{
					OnIssueStateBecomeTestReceived(fieldChangeEvent);
				}
				//...костыль

				if (!bufferedEvent.IsActual)
				{
					BufferDebugLog(sender, $"Event after buffering has become not actual: {bufferedEvent.GetType()}. Issue: {sender.LastIssueState.Id}");
					continue;
				}
				else
				{
					BufferDebugLog(sender, $"Handle actual event type: {bufferedEvent.GetType()}");
				}

				if (bufferedEvent is YouTrackIssueSimpleFieldChangedEvent)
				{
					OnIssueSimpleFieldChangeEventReceived(bufferedEvent as YouTrackIssueSimpleFieldChangedEvent);
				}
				else if (bufferedEvent is YouTrackIssueAssigneeChangedEvent)
				{
					OnIssueAssigneeChangeEventReceived(bufferedEvent as YouTrackIssueAssigneeChangedEvent);
				}
				else if (bufferedEvent is YouTrackIssueSummaryChangedEvent)
				{
					OnIssueSummaryChangeEventReceived(bufferedEvent as YouTrackIssueSummaryChangedEvent);
				}
				else if (bufferedEvent is YouTrackIssueDescriptionChangedEvent)
				{
					OnIssueDescriptionChangeEventReceived(bufferedEvent as YouTrackIssueDescriptionChangedEvent);
				}
				else if (bufferedEvent is YouTrackIssueCreateEvent)
				{
					OnIssueCreateEventReceived(bufferedEvent as YouTrackIssueCreateEvent);
				}
				else if (bufferedEvent is YouTrackIssueLinksAddedEvent)
				{
					OnIssueLinksAddedEventReceived(bufferedEvent as YouTrackIssueLinksAddedEvent);
				}
				else if (bufferedEvent is YouTrackIssueLinksRemovedEvent)
				{
					OnIssueLinksRemovedEventReceived(bufferedEvent as YouTrackIssueLinksRemovedEvent);
				}
				else if (bufferedEvent is YouTrackIssueCommentsEvent)
				{
					OnIssueCommentsAddedEventReceived(bufferedEvent as YouTrackIssueCommentsEvent);
				}
				else
				{
					BufferDebugLog(sender, $"Unsupported event type: {bufferedEvent.GetType()}");
				}
			}

			lock (_issuesBuffers)
			{
				_issuesBuffers.Remove(sender.FirstIssueState.Id);
			}

			sender.OnCompleted -= OnIssueEventsBufferCompleted;
			sender.Dispose();
		}

		private void BufferDebugLog(YouTrackIssueEventsBuffer buffer, string message)
		{
			DebugLog($"[BUFFER:{buffer.FirstIssueState.Id}] {message}");
		}

		private void DebugLog(string message)
		{
			if (App.Config.Logging.BufferingYoutrackNotifications)
			{
				Logger.Log($"[Buffers] {message}");
			}
		}
	}
}