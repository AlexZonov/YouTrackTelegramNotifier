using Newtonsoft.Json;

namespace com.services.youtrack.custom.issue
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueField
	{
		[JsonProperty("name")]
		public string Name { get; private set; }

		[JsonProperty("value")]
		public string Value { get; private set; }
	}
}