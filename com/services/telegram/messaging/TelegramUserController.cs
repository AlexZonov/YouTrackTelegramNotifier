using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.database;
using com.main;
using com.main.application;
using com.main.logger;
using com.services.telegram.commands;
using com.services.telegram.dialogs;
using com.services.telegram.messaging.handlers;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram.messaging
{
	internal class TelegramUserController : ITelegramMessagesHandler
	{
		protected TelegramService Telegram { get { return App.Telegram; } }
		protected Database Database { get { return App.Database; } }

		private ITelegramMessagesHandler _handlerProxy;

		private TelegramAuthorizationHandler _authorizationHandler;
		private TelegramMainHandler _mainHandler;
		private TelegramDialogsHandler _dialogsHandler;
		private int _userId;

		private Task _youtrackNotificationProcess;
		private Queue<string> _youTrackNotificationsQueue;

		public TelegramUserController(int userId)
		{
			_userId = userId;
			_youTrackNotificationsQueue = new Queue<string>();

			ChatUser chatUser;
			if (!Database.TryGetChatUserById(userId, out chatUser) || !chatUser.IsAuthorized)
			{
				InitAuthorizationHandler(_userId);
				SetAuthorizationHandlerAsProxy();
			}
			else
			{
				InitOtherHandlers(chatUser);
			}
		}

		public void HandleMessage(Message message)
		{
			if (App.Config.Logging.ReceiveTelegramMessage)
			{
				Logger.Log($"Receive message from {_userId}: {message.Text}");
			}

			if (_handlerProxy != null)
			{
				_handlerProxy.HandleMessage(message);
			}
			else
			{
				Logger.Log($"Invalid logic, proxy is not set! User: {_userId}");
			}
		}

		public void StartDialog(TelegramDialog dialog)
		{
			SetDialogHandlerAsProxy();
			_dialogsHandler.StartDialog(dialog);
		}

		public void SendYouTrackNotification(string message)
		{
			_youTrackNotificationsQueue.Enqueue(message);
		}

		private void InitAuthorizationHandler(int userId)
		{
			_authorizationHandler = new TelegramAuthorizationHandler(userId);
			_authorizationHandler.OnAuthorizationSuccess += OnAuthorizationSuccess;
		}

		private void DisposeAuthorizationHandler()
		{
			if (_authorizationHandler != null)
			{
				_authorizationHandler.OnAuthorizationSuccess -= OnAuthorizationSuccess;
				_authorizationHandler.Dispose();
				_authorizationHandler = null;
			}
		}

		private void OnAuthorizationSuccess(ChatUser chatUser)
		{
			DisposeAuthorizationHandler();
			InitOtherHandlers(chatUser);

			TelegramCommands.Help.Execute(chatUser, null);
		}

		private void InitOtherHandlers(ChatUser chatUser)
		{
			InitMainHandler(chatUser);
			InitDialogHandler(chatUser);

			SetMainHandlerAsProxy();

			_youtrackNotificationProcess = StartYouTrackNotificationProcess();
		}

		private void InitMainHandler(ChatUser chatUser)
		{
			_mainHandler = new TelegramMainHandler(chatUser);
		}

		private void InitDialogHandler(ChatUser chatUser)
		{
			_dialogsHandler = new TelegramDialogsHandler(chatUser);
			_dialogsHandler.OnSomeDialogComplete += OnDialogsHandlerSomeDialogComplete;
		}

		private void OnDialogsHandlerSomeDialogComplete()
		{
			if (!_dialogsHandler.IsDialogsExist())
			{
				SetMainHandlerAsProxy();
			}
		}

		private void SetAuthorizationHandlerAsProxy()
		{
			_handlerProxy = _authorizationHandler;
		}

		private void SetMainHandlerAsProxy()
		{
			_handlerProxy = _mainHandler;
		}

		private void SetDialogHandlerAsProxy()
		{
			_handlerProxy = _dialogsHandler;
		}

		private Task<Message> SendTextMessage(string message, IReplyMarkup replyMarkup = null)
		{
			return Telegram.SendTextMessageAsync(_userId, message, replyMarkup);
		}

		//TODO: may be create special class?
		private async Task StartYouTrackNotificationProcess()
		{
			while (true)
			{
				bool isMainHandlerAvailable = _mainHandler != null && _handlerProxy == _mainHandler;
				bool isNotificationsExist = _youTrackNotificationsQueue.Count > 0;
				if (isMainHandlerAvailable && isNotificationsExist)
				{
					string message = _youTrackNotificationsQueue.Dequeue();
					SendTextMessage(message);
				}

				await Task.Delay(200);
			}
		}
	}
}