using System;
using com.main;
using com.main.logger;
using com.services.telegram;
using com.services.youtrack.custom.events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.services.youtrack
{
	internal class YouTrackNotificationsReceiver
	{
		//TODO: сделать 1 эвент с энамом и базовым эвентом
		//TODO: это позволит избавиться от сложного внедрения новых событий
		//TODO: т.к. сейчас много событий и они дублируются в нескольких местах
		public event Action<YouTrackIssueSimpleFieldChangedEvent> OnIssueSimpleFieldChangeEventReceived = (YouTrackIssueSimpleFieldChangedEvent eventData) => { };
		public event Action<YouTrackIssueAssigneeChangedEvent> OnIssueAssigneeChangeEventReceived = (YouTrackIssueAssigneeChangedEvent eventData) => { };
		public event Action<YouTrackIssueSummaryChangedEvent> OnIssueSummaryChangeEventReceived = (YouTrackIssueSummaryChangedEvent eventData) => { };
		public event Action<YouTrackIssueDescriptionChangedEvent> OnIssueDescriptionChangeEventReceived = (YouTrackIssueDescriptionChangedEvent eventData) => { };
		public event Action<YouTrackIssueCreateEvent> OnIssueCreateEventReceived = (YouTrackIssueCreateEvent eventData) => { };
		public event Action<YouTrackIssueLinksAddedEvent> OnIssueLinksAddedEventReceived = (YouTrackIssueLinksAddedEvent eventData) => { };
		public event Action<YouTrackIssueLinksRemovedEvent> OnIssueLinksRemovedEventReceived = (YouTrackIssueLinksRemovedEvent eventData) => { };
		public event Action<YouTrackIssueCommentsEvent> OnIssueCommentsAddedEventReceived = (YouTrackIssueCommentsEvent eventData) => { };

		private WebService _webService;

		public YouTrackNotificationsReceiver(WebService webService)
		{
			_webService = webService;
			_webService.OnReceivedTextData += OnWebServiceReceivedTextData;
		}

		public void SimulateReceive(string json)
		{
			OnWebServiceReceivedTextData(json);
		}

		private void OnWebServiceReceivedTextData(string textData)
		{
			JObject jsonObject = null;

			try
			{
				jsonObject = JObject.Parse(textData);
			}
			catch (JsonReaderException e)
			{
				Logger.LogException(e);
			}

			if (jsonObject == null)
			{
				return;
			}

			JToken eventJsonToken = jsonObject.GetValue("event");
			JToken dataJsonToken = jsonObject.GetValue("data");

			if (eventJsonToken == null)
			{
				Logger.LogError($"Event field is not parsed in JSON: {jsonObject.ToString(Formatting.None)}");
				return;
			}

			if (dataJsonToken == null)
			{
				Logger.LogError($"Data field is not parsed in JSON: {jsonObject.ToString(Formatting.None)}");
				return;
			}

			YouTrackIssueEventType eventType = eventJsonToken.ToObject<YouTrackIssueEventType>();

			switch (eventType)
			{
				case YouTrackIssueEventType.AssigneeChanged:
				{
					YouTrackIssueAssigneeChangedEvent eventData = dataJsonToken.ToObject<YouTrackIssueAssigneeChangedEvent>();
					OnIssueAssigneeChangeEventReceived(eventData);
					break;
				}
				case YouTrackIssueEventType.SummaryChanged:
				{
					YouTrackIssueSummaryChangedEvent eventData = dataJsonToken.ToObject<YouTrackIssueSummaryChangedEvent>();
					OnIssueSummaryChangeEventReceived(eventData);
					break;
				}
				case YouTrackIssueEventType.DescriptionChanged:
				{
					YouTrackIssueDescriptionChangedEvent eventData = dataJsonToken.ToObject<YouTrackIssueDescriptionChangedEvent>();
					OnIssueDescriptionChangeEventReceived(eventData);
					break;
				}
				case YouTrackIssueEventType.SimpleFieldChanged:
				{
					YouTrackIssueSimpleFieldChangedEvent eventData = dataJsonToken.ToObject<YouTrackIssueSimpleFieldChangedEvent>();
					OnIssueSimpleFieldChangeEventReceived(eventData);
					break;
				}
				case YouTrackIssueEventType.Created:
				{
					YouTrackIssueCreateEvent eventData = dataJsonToken.ToObject<YouTrackIssueCreateEvent>();
					OnIssueCreateEventReceived(eventData);
					break;
				}
				case YouTrackIssueEventType.LinksAdded:
				{
					YouTrackIssueLinksAddedEvent eventData = dataJsonToken.ToObject<YouTrackIssueLinksAddedEvent>();
					OnIssueLinksAddedEventReceived(eventData);
					break;
				}
				case YouTrackIssueEventType.LinksRemoved:
				{
					YouTrackIssueLinksRemovedEvent eventData = dataJsonToken.ToObject<YouTrackIssueLinksRemovedEvent>();
					OnIssueLinksRemovedEventReceived(eventData);
					break;
				}
				case YouTrackIssueEventType.CommentsAdded:
				{
					YouTrackIssueCommentsEvent eventData = dataJsonToken.ToObject<YouTrackIssueCommentsEvent>();
					OnIssueCommentsAddedEventReceived(eventData);
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}

			Logger.Log($"deserialized event: {eventType}");
		}
	}
}