using Newtonsoft.Json;

namespace com.config
{
	internal class YouTrackApiConfig
	{
		[JsonProperty("user_check_url")]
		public string UserCheckUrl { get; private set; }
	}
}