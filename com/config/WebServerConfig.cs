using Newtonsoft.Json;

namespace com.config
{
	internal class WebServerConfig
	{
		[JsonProperty("address")]
		public string Address { get; private set; } = "+";

		[JsonProperty("port")]
		public int Port { get; private set; } = 8080;

		[JsonProperty("max_threads")]
		public int MaxThreads { get; private set; } = 4;

		[JsonProperty("max_content_size")]
		public int MaxContentSize { get; private set; } = 65536;

		[JsonProperty("url_path")]
		public string UrlPath { get; private set; }

		[JsonProperty("access_token_header_key")]
		public string AccessTokenHeaderKey { get; private set; } = "Access-Token";

		[JsonProperty("access_token")]
		public string AccessToken { get; private set; }
	}
}