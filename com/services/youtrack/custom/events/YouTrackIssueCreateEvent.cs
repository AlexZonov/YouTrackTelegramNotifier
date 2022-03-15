using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueCreateEvent : YouTrackIssueEventBase
	{
		[JsonProperty("updater")]
		public YouTrackUser Updater { get; private set; }

		[JsonProperty("assignee")]
		public YouTrackUser Assignee { get; private set; }

		protected override void StackImpl(YouTrackIssueEventBase otherEvent)
		{
			YouTrackIssueCreateEvent similarEvent = (YouTrackIssueCreateEvent) otherEvent;
			Updater = similarEvent.Updater;
			Assignee = similarEvent.Assignee;
		}

		protected override bool IsActualImpl()
		{
			return Updater.Login != Assignee.Login;
		}
	}
}