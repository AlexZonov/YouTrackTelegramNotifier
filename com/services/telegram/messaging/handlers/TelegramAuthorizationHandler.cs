using System;
using com.config;
using com.database;
using com.main;
using com.main.application;
using Telegram.Bot.Types;

namespace com.services.telegram.messaging.handlers
{
	internal class TelegramAuthorizationHandler : TelegramMessagesHandlerBase
	{
		public event Action<ChatUser> OnAuthorizationSuccess = (ChatUser chatUser) => { };

		private Database Database { get { return App.Database; }}
		private Config Config { get { return App.Config; }}

		private bool IsFirstMessage { get; set; } = true;

		public TelegramAuthorizationHandler(int userId) : base(userId) { }

		public override void Dispose()
		{
			base.Dispose();
			OnAuthorizationSuccess = null;
		}

		protected override void HandleSomeMessageImpl(Message message)
		{
			User user = message.From;
			ChatUser chatUser = Database.TryAddChatUser(user);

			if (chatUser.IsAuthorized)
			{
				OnAuthorizationSuccess(chatUser);
				return;
			}

			if (IsFirstMessage)
			{
				IsFirstMessage = false;
				SendTextMessage("You are not logged in, enter the secret key!");
				return;
			}

			string inputSecretKey = message.Text;
			bool isSecretKeyValid = inputSecretKey == Config.Telegram.SecretKey;
			if (isSecretKeyValid)
			{
				chatUser.Authorize(); //Database.Save(App.Database); внутри выглядит не очень, а тут уже имеется БД!
				SendTextMessage($"You have successfully logged!");
				OnAuthorizationSuccess(chatUser);
			}
			else
			{
				SendTextMessage($"Secret key: \"{inputSecretKey}\" is not valid! Try again.");
			}
		}
	}
}