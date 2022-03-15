using System;
using System.Collections.Generic;

namespace com.services.telegram.commands
{
	internal static class TelegramCommands
	{
		public static readonly TelegramStartCommand Start;
		public static readonly TelegramHelpCommand Help;
		public static readonly TelegramLinkCommand Link;
		public static readonly TelegramUnlinkCommand Unlink;
		public static readonly TelegramStatusCommand Status;
		public static readonly TelegramSettingsCommand Settings;
		public static readonly TelegramCancelCommand Cancel;
		public static readonly TelegramTesterModeCommand TesterMode;

		public static readonly Dictionary<string, TelegramCommand> Commands;

		static TelegramCommands()
		{
			Commands = new Dictionary<string, TelegramCommand>();

			Start = RegisterCommand<TelegramStartCommand>();
			Help = RegisterCommand<TelegramHelpCommand>();
			Link = RegisterCommand<TelegramLinkCommand>();
			Unlink = RegisterCommand<TelegramUnlinkCommand>();
			Status = RegisterCommand<TelegramStatusCommand>();
			Settings = RegisterCommand<TelegramSettingsCommand>();
			Cancel = RegisterCommand<TelegramCancelCommand>();
			TesterMode = RegisterCommand<TelegramTesterModeCommand>();
		}

		private static T RegisterCommand<T>() where T : TelegramCommand
		{
			T result = (T) Activator.CreateInstance(typeof(T));
			Commands.Add(result.Name, result);
			return result;
		}

		public static TelegramCommand GetCommand(string name)
		{
			TelegramCommand result;
			if (!Commands.TryGetValue(name, out result))
			{
				result = null;
			}

			return result;
		}
	}
}