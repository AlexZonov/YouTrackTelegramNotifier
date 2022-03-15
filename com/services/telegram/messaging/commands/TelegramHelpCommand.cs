using System;
using System.Collections.Generic;
using System.Text;
using com.database;

namespace com.services.telegram.commands
{
	internal class TelegramHelpCommand : TelegramCommand
	{
		private string _cachedHelpText;

		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			SendMessage(user.Id, GetHelpText());
		}

		protected override string GetCommandName()
		{
			return "help";
		}

		protected override string GetDescription()
		{
			return "get help for all commands.";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}

		private string GetHelpText()
		{
			if (!String.IsNullOrEmpty(_cachedHelpText))
			{
				return _cachedHelpText;
			}

			StringBuilder builder = new StringBuilder();

			Dictionary<string, TelegramCommand> allCommands = TelegramCommands.Commands;
			foreach (var commandPair in allCommands)
			{
				string commandName = commandPair.Key;
				TelegramCommand command = commandPair.Value;

				if (command.IsPrivate())
				{
					continue;
				}

				builder.AppendLine(command.Description);
				//builder.AppendLine("");
			}

			return builder.ToString();
		}
	}
}