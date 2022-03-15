using System;
using com.services.telegram.commands;
using com.services.telegram.youtrack;

namespace com.services.telegram.messaging
{
	internal static class TelegramMessages
	{
		public static string InvalidArguments = "Some arguments are incorrect: {0}.\r\n" +
		                                        $"Use {TelegramCommands.Help.SpecialName} command for to be sure!";

		public static string YouNotLinked = $"You not linked! Use {TelegramCommands.Link.SpecialName} command for link.";
		public static string NotImplementedCommand = $"Not implemented command!";
	}
}