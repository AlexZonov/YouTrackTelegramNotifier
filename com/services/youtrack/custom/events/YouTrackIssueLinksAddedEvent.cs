using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueLinksAddedEvent : YouTrackIssueLinksEvent
	{
		//empty
	}
}