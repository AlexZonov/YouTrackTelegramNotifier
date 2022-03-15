using com.database;

namespace com.services.telegram.commands
{
	internal class TelegramStartCommand : TelegramCommand
	{
		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			SendMessage(user.Id, $"Hello {user.Username}!");
			TelegramCommands.Help.Execute(user, inputArguments);
		}

		protected override string GetCommandName()
		{
			return "start";
		}

		protected override string GetDescription()
		{
			return "starting work with bot, show help.";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}

		public override bool IsPrivate()
		{
			return true;
		}
	}
}