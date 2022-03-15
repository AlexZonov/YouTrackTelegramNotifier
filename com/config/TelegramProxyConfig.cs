using Newtonsoft.Json;

namespace com.config
{
	internal class TelegramProxyConfig
	{
		[JsonProperty("address")]
		public string Address { get; private set; }

		[JsonProperty("port")]
		public int Port { get; private set; }

		[JsonProperty("login")]
		public string Login { get; private set; }

		[JsonProperty("password")]
		public string Password { get; private set; }

		[JsonProperty("enabled")]
		public bool Enabled { get; private set; }
	}
}