using Newtonsoft.Json;

namespace com.services.youtrack.custom.events
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueLinksRemovedEvent : YouTrackIssueLinksEvent
	{
		//empty
	}
}