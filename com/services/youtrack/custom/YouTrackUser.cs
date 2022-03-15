using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace com.services.youtrack.custom
{
	[JsonObject(MemberSerialization.OptIn)]
	public class YouTrackUser
	{
		private const string UNASSIGNED_LITERAL = "Unassigned";
		
		[JsonProperty("login")]
		public string Login { get; private set; }

		[JsonProperty("full_name")]
		public string FullName { get; private set; }

		[JsonProperty("email")]
		public string Email { get; private set; }

		public bool IsUnassigned { get; private set; }

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			IsUnassigned = Login == UNASSIGNED_LITERAL && FullName == UNASSIGNED_LITERAL;
		}
	}
}