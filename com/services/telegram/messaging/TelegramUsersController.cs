using System.Collections.Generic;
using com.database;
using com.main.application;
using com.services.telegram.dialogs;
using Telegram.Bot.Types;

namespace com.services.telegram.messaging
{
	internal class TelegramUsersController : ITelegramMessagesHandler
	{
		private Dictionary<int, TelegramUserController> _controllers;

		public TelegramUsersController()
		{
			_controllers = new Dictionary<int, TelegramUserController>();

			CreateKnownUsersControllers();
		}

		public void HandleMessage(Message message)
		{
			GetController(message.From.Id).HandleMessage(message);
		}

		public void StartDialog(int userId, TelegramDialog dialog)
		{
			GetController(userId).StartDialog(dialog);
		}

		public void SendYouTrackNotification(int userId, string message)
		{
			GetController(userId).SendYouTrackNotification(message);
		}

		private TelegramUserController GetController(int userId)
		{
			TelegramUserController result; 
			if (!_controllers.TryGetValue(userId, out result))
			{
				result = CreateUserController(userId);
			}

			return result;
		}

		private TelegramUserController CreateUserController(int userId)
		{
			TelegramUserController controller = new TelegramUserController(userId);
			_controllers.Add(userId, controller);
			return controller;
		}

		private void CreateKnownUsersControllers()
		{
			foreach (var chatUserPair in App.Database.GetAllChatUsers())
			{
				ChatUser chatUser = chatUserPair.Value;
				CreateUserController(chatUser.Id);
			}
		}
	}
}