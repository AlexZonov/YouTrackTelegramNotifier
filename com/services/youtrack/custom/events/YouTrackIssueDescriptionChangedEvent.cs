using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueDescriptionChangedEvent : YouTrackIssueStringValueChangedEvent
	{
		//empty
	}
}