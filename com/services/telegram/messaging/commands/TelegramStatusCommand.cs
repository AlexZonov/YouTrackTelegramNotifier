using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.database;
using com.main;
using com.main.application;
using com.services.telegram.messaging;
using com.services.telegram.youtrack;
using com.utilities;

namespace com.services.telegram.commands
{
	internal class TelegramStatusCommand : TelegramCommand
	{
		protected Database Database { get { return App.Database; }}

		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			int chatId = user.Id;
			LinkedUser linkedUser;
			if (Database.TryGetLinkedUserByChatId(chatId, out linkedUser))
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("You are linked! Login: \"");
				builder.Append(linkedUser.YouTrackLogin);
				builder.Append("\"");
				builder.Append(", Chat ID: \"");
				builder.Append(linkedUser.TelegramChatId);
				builder.Append("\"");
				builder.AppendLine(".");
				builder.AppendLine("");
				builder.Append("Tester mode: ".ToBold());
				builder.AppendLine(linkedUser.IsTester ? "enabled." : "disabled.");

				List<int> enabled = linkedUser.NotificationSettings.Where(pair => pair.Value).Select(pair => pair.Key).ToList();
				List<int> disabled = linkedUser.NotificationSettings.Keys.Except(enabled).ToList();

				if (enabled.Count > 0)
				{
					builder.AppendLine("");
					builder.AppendLine("Enabled notifications:".ToBold());

					for (int j = 0; j < enabled.Count; j++)
					{
						YouTrackNotificationType notificationType = (YouTrackNotificationType) enabled[j];
						builder.Append(j + 1);
						builder.Append(". ");
						builder.AppendLine(StringUtilities.GetVariablePresentation(notificationType.ToString(), true));
					}
				}

				if (disabled.Count > 0)
				{
					builder.AppendLine("");
					builder.AppendLine("Disabled notifications:".ToBold());

					for (int j = 0; j < disabled.Count; j++)
					{
						YouTrackNotificationType notificationType = (YouTrackNotificationType) disabled[j];
						builder.Append(j + 1);
						builder.Append(". ");
						builder.AppendLine(StringUtilities.GetVariablePresentation(notificationType.ToString(), true));
					}
				}

				SendMessage(user.Id, builder.ToString());
			}
			else
			{
				SendMessage(user.Id, TelegramMessages.YouNotLinked);
			}
		}

		protected override string GetCommandName()
		{
			return "status";
		}

		protected override string GetDescription()
		{
			return "get current integration information.";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}
	}
}