using Newtonsoft.Json;

namespace com.config
{
	internal class YouTrackBufferingConfig
	{
		[JsonProperty("first_duration")]
		public float FirstDuration { get; private set; } = 30;

		[JsonProperty("following_duration")]
		public float FollowingDuration { get; private set; } = 5;
	}
}