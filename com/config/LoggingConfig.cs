using Newtonsoft.Json;

namespace com.config
{
	internal class LoggingConfig
	{
		[JsonProperty("send_message")]
		public bool SendTelegramMessage { get; private set; } = true;

		[JsonProperty("receive_message")]
		public bool ReceiveTelegramMessage { get; private set; } = true;

		[JsonProperty("buffering")]
		public bool BufferingYoutrackNotifications { get; private set; } = true;
	}
}