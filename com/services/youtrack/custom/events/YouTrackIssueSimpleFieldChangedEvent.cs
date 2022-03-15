using System;
using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueSimpleFieldChangedEvent : YouTrackIssueEventBase
	{
		[JsonProperty("updater")]
		public YouTrackUser Updater { get; private set; }

		[JsonProperty("field")]
		public string FieldName { get; private set; }

		[JsonProperty("old_value")]
		public string OldValue { get; private set; }

		[JsonProperty("new_value")]
		public string NewValue { get; private set; }

		public override bool IsSimilar(YouTrackIssueEventBase otherEvent)
		{
			YouTrackIssueSimpleFieldChangedEvent similarEvent = otherEvent as YouTrackIssueSimpleFieldChangedEvent;
			return similarEvent != null && base.IsSimilar(otherEvent) && FieldName == similarEvent.FieldName;
		}

		protected override void StackImpl(YouTrackIssueEventBase otherEvent)
		{
			YouTrackIssueSimpleFieldChangedEvent similarEvent = (YouTrackIssueSimpleFieldChangedEvent) otherEvent;
			Updater = similarEvent.Updater;
			NewValue = similarEvent.NewValue;
		}

		protected override bool IsActualImpl()
		{
			return Updater.Login != Issue.Assignee.Login && OldValue != NewValue && !String.IsNullOrEmpty(NewValue);
		}
	}
}