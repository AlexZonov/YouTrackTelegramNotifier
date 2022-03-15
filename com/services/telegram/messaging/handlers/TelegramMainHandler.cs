using System.Text;
using com.database;
using com.main.logger;
using com.services.telegram.commands;
using Telegram.Bot.Types;

namespace com.services.telegram.messaging.handlers
{
	internal class TelegramMainHandler : TelegramAuthorizedMessagesHandler
	{
		public override void Dispose()
		{
			base.Dispose();
		}

		public TelegramMainHandler(ChatUser chatUser) : base(chatUser)
		{
			//empty
		}

		protected override void HandleUnreadMessageImpl(Message message)
		{
			Logger.Log($"Skipped unread message with id: {message.MessageId}");
		}

		protected override void HandleSomeMessageImpl(Message message)
		{
			//empty
		}

		protected override void HandleSimpleMessageImpl(Message message)
		{
			string messageData = GetMessageStringData(message);
			Logger.Log(messageData);
		}

		protected override void HandleCommandMessageImpl(Message message, string commandName, string[] commandArgs)
		{
			TelegramCommand command = TelegramCommands.GetCommand(commandName);
			if (command != null)
			{
				Logger.Log($"User: {ChatUser.Username}, Command: {command.SpecialName}, Args count: {(commandArgs == null ? 0 : commandArgs.Length)}");
				command.Execute(ChatUser, commandArgs);
			}
			else
			{
				SendTextMessage($"Unknown command: {commandName}");
			}
		}

		private string GetMessageStringData(Message message)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("[");
			builder.Append(message.Date);
			builder.Append("] ID: ");
			builder.Append(message.From.Id);
			builder.Append(", Username: ");
			builder.Append(message.From.Username);
			builder.Append(", Text: ");
			builder.Append(message.Text);

			return builder.ToString();
		}
	}
}