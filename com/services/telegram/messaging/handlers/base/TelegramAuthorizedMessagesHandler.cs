using com.database;

namespace com.services.telegram.messaging
{
	internal class TelegramAuthorizedMessagesHandler : TelegramMessagesHandlerBase
	{
		protected ChatUser ChatUser { get; private set; }

		public TelegramAuthorizedMessagesHandler(ChatUser chatUser) : base(chatUser.Id)
		{
			ChatUser = chatUser;
		}
	}
}