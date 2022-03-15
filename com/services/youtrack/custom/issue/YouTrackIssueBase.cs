using System.Runtime.Serialization;
using com.utilities;
using Newtonsoft.Json;

namespace com.services.youtrack.custom.issue
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackIssueBase
	{
		[JsonProperty("id")]
		public string Id { get; private set; }

		[JsonProperty("url")]
		public string Url { get; private set; }

		[JsonProperty("summary")]
		public string Summary { get; private set; }

		[JsonProperty("resolved")]
		public bool IsResolved { get; private set; }

		public string Link { get; private set; }
		public string LinkWithBrackets { get; private set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Url = Url.Replace("http://", "https://");
			Link = $"{Id}".ToLink(Url);
			LinkWithBrackets = $"[{Id}]".ToLink(Url);
		}
	}
}