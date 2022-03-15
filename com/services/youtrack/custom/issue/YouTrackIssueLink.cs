using Newtonsoft.Json;

namespace com.services.youtrack.custom.issue
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueLink
	{
		[JsonProperty("link_type")]
		public string LinkType { get; private set; }

		[JsonProperty("issue_link")]
		public YouTrackIssueBase Issue { get; private set; }
	}
}