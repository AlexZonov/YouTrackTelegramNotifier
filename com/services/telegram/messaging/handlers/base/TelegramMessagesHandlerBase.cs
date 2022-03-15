using System;
using com.main;
using com.main.application;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.services.telegram.messaging
{
	internal class TelegramMessagesHandlerBase : ITelegramMessagesHandler, IDisposable
	{
		protected int UserId { get; set; }
		protected TelegramService Telegram { get { return App.Telegram; } }
		private int LastUnreadMessageId  { get { return Telegram.LastUnreadMessageId; } }

		public TelegramMessagesHandlerBase(int userId)
		{
			UserId = userId;
		}

		public virtual void Dispose()
		{
			//empty
		}

		public void HandleMessage(Message message)
		{
			string messageText = message.Text;

			if(LastUnreadMessageId != -1 && message.MessageId <= LastUnreadMessageId)
			{
				HandleUnreadMessageImpl(message);
				return;
			}

			bool isMessageEmpty = String.IsNullOrEmpty(messageText);
			if (isMessageEmpty)
			{
				return;
			}

			HandleSomeMessageImpl(message);
			bool isCommand = messageText[0] == '/';
			if (isCommand)
			{
				string[] commandParts = messageText.Split(' ');
				string commandName = commandParts[0].Remove(0, 1);
				string[] commandArgs = new string[commandParts.Length - 1];
				for (int i = 0; i < commandParts.Length - 1; i++)
				{
					commandArgs[i] = commandParts[i + 1];
				}
				HandleCommandMessageImpl(message, commandName, commandArgs);
			}
			else
			{
				HandleSimpleMessageImpl(message);
			}
		}

		protected virtual void HandleUnreadMessageImpl(Message message)
		{
			//empty
		}

		protected virtual void HandleSomeMessageImpl(Message message)
		{
			//empty
		}

		protected virtual void HandleSimpleMessageImpl(Message message)
		{
			//empty
		}

		protected virtual void HandleCommandMessageImpl(Message message, string commandName, string[] commandArgs)
		{
			//empty
		}

		protected void SendTextMessage(string message, IReplyMarkup replyMarkup = null)
		{
			App.Telegram.SendTextMessageAsync(UserId, message, replyMarkup);
		}
	}
}