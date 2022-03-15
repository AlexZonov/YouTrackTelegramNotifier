using Telegram.Bot.Types;

namespace com.services.telegram.messaging
{
	internal interface ITelegramMessagesHandler
	{
		void HandleMessage(Message message);
	}
}