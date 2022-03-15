using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class YouTrackIssueStringValueChangedEvent : YouTrackIssueEventBase
	{
		[JsonProperty("updater")]
		public YouTrackUser Updater { get; private set; }

		[JsonProperty("old_value")]
		public string OldValue { get; private set; }

		[JsonProperty("new_value")]
		public string NewValue { get; private set; }

		protected override void StackImpl(YouTrackIssueEventBase otherEvent)
		{
			YouTrackIssueStringValueChangedEvent similarEvent = (YouTrackIssueStringValueChangedEvent) otherEvent;
			NewValue = similarEvent.NewValue;
			Updater = similarEvent.Updater;
		}

		protected override bool IsActualImpl()
		{
			return Updater.Login != Issue.Assignee.Login && OldValue != NewValue;
		}
	}
}