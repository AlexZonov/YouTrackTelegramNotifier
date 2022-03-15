using Newtonsoft.Json;

namespace com.config
{
	internal class YouTrackConfig
	{
		[JsonProperty("login")]
		public string Login { get; private set; }

		[JsonProperty("password")]
		public string Password { get; private set; }

		[JsonProperty("token")]
		public string Token { get; private set; }

		[JsonProperty("api")]
		public YouTrackApiConfig Api { get; private set; } = new YouTrackApiConfig();

		[JsonProperty("buffering")]
		public YouTrackBufferingConfig Buffering { get; private set; } = new YouTrackBufferingConfig();
	}
}