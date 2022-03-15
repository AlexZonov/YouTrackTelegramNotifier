using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using com.config;
using com.database;
using com.main.logger;
using com.services.telegram.messaging;
using com.services.telegram.youtrack;
using com.services.youtrack;
using com.utilities;
using DryIoc;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram
{
	//https://tlgrm.ru/docs/bots/api
	internal class TelegramService
	{
		public TelegramBotClient BotClient { get; private set; }
		public TelegramUsersController UsersController { get; private set; }

		public int LastUnreadMessageId { get; private set; } = -1;

		private TelegramConfig _telegramConfig;
		private LoggingConfig _loggingConfig;

		private YouTrackNotificationsController _youtrackNotificationsController;

		public TelegramService(IContainer container)
		{
			container.Use(this);

			container.Register<TelegramUsersController>();
			container.Register<YouTrackNotificationsController>();
			container.Register<YouTrackIssuesEventsBuffer>();
			container.Register<IWebProxy, TelegramProxy>();

			_telegramConfig = container.Resolve<TelegramConfig>();
			_loggingConfig = container.Resolve<LoggingConfig>();

			TelegramProxyConfig proxyConfig = _telegramConfig.Proxy;
			if (proxyConfig.Enabled)
			{
				BotClient = new TelegramBotClient(_telegramConfig.BotToken, container.Resolve<IWebProxy>());
			}
			else
			{
				BotClient = new TelegramBotClient(_telegramConfig.BotToken);
			}

			container.Use(BotClient);

			BotClient.OnMessage += OnBotClientMessage;
			BotClient.OnReceiveError += OnBotClientError;

			Update[] unreadData = null;
			while(unreadData == null)
			{
				try
				{
					unreadData = BotClient.GetUpdatesAsync().Result;
				}
				catch(Exception e)
				{
					Logger.LogError($"Failed telegram GetUpdatesAsync operation! May be incorrect proxy! Details: {e.Message}");
					Thread.Sleep(5000);
				}
			}

			if(unreadData.Length > 0)
			{
				//LastUnreadMessageId = unreadData[unreadData.Length - 1].Message.MessageId;
				LastUnreadMessageId = unreadData.LastOrDefault(update => update.Message != null)?.Message.MessageId ?? -1;
			}

			User mineUser = BotClient.GetMeAsync().Result;
			Logger.Log($"Bot initialized! Name: {mineUser.Username}, ID: {mineUser.Id}");
			BotClient.StartReceiving();

			UsersController = container.Resolve<TelegramUsersController>();
			_youtrackNotificationsController = container.Resolve<YouTrackNotificationsController>();

			container.Use(UsersController);
		}

		private void OnBotClientMessage(object sender, MessageEventArgs e)
		{
			e.Message.Text = HtmlUtilities.Escape(e.Message.Text);
			UsersController.HandleMessage(e.Message);
		}

		private void OnBotClientError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
		{
			Logger.LogError($"[Bot] Message: {receiveErrorEventArgs.ApiRequestException.Message}, help: {receiveErrorEventArgs.ApiRequestException.HelpLink}");
		}

		public Task<Message> SendTextMessageAsync(int chatId, string message, IReplyMarkup replyMarkup = null)
		{
			if (_telegramConfig.IsDebugSendMessageEnabled)
			{
				chatId = _telegramConfig.DebugSendMessageChatId;
			}

			if (replyMarkup == null)
			{
				replyMarkup = new ReplyKeyboardRemove();
			}

			if (_loggingConfig.SendTelegramMessage)
			{
				Logger.Log($"Send message to {chatId}: {message}");
			}

			return BotClient.SendTextMessageAsync(
				chatId, 
				message, 
				_telegramConfig.ParseMode, 
				false, 
				false, 
				0, 
				replyMarkup);
		}

		public void SendYouTrackNotification(int chatId, string message)
		{
			UsersController.SendYouTrackNotification(chatId, message);
		}
	}
}