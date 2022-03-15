using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueAssigneeChangedEvent : YouTrackIssueEventBase
	{
		[JsonProperty("updater")]
		public YouTrackUser Updater { get; private set; }

		[JsonProperty("old_value")]
		public YouTrackUser OldValue { get; private set; }

		[JsonProperty("new_value")]
		public YouTrackUser NewValue { get; private set; }

		protected override void StackImpl(YouTrackIssueEventBase otherEvent)
		{
			YouTrackIssueAssigneeChangedEvent similarEvent = (YouTrackIssueAssigneeChangedEvent) otherEvent;
			Updater = similarEvent.Updater;
			NewValue = similarEvent.NewValue;
		}

		protected override bool IsActualImpl()
		{
			string newUserLogin = NewValue.Login;
			string oldUserLogin = OldValue.Login;
			string updaterUserLogin = Updater.Login;

			bool isNeedSendMessageNewUser = newUserLogin != updaterUserLogin;
			bool isNeedSendMessageOldUser = oldUserLogin != updaterUserLogin;

			return (isNeedSendMessageNewUser || isNeedSendMessageOldUser) && OldValue.Login != NewValue.Login;
		}
	}
}