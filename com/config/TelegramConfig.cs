using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;

namespace com.config
{
	internal class TelegramConfig
	{
		[JsonProperty("proxy")]
		public TelegramProxyConfig Proxy { get; private set; } = new TelegramProxyConfig();

		[JsonProperty("bot_token")]
		public string BotToken { get; private set; }

		[JsonProperty("secret_key")]
		public string SecretKey { get; private set; }

		[JsonProperty("debug_send_message_chat_id")]
		public int DebugSendMessageChatId { get; private set; }

		[JsonProperty("is_debug_send_message_enabled")]
		public bool IsDebugSendMessageEnabled { get; private set; }

		[JsonProperty("parse_mode")]
		public ParseMode ParseMode { get; private set; } = ParseMode.Html;
	}
}