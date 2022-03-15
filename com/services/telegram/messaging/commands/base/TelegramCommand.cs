using System;
using System.Text;
using com.database;
using com.main;
using com.main.application;
using com.services.telegram.messaging;

namespace com.services.telegram.commands
{
	internal abstract class TelegramCommand
	{
		protected TelegramService Telegram { get { return App.Telegram; } }

		public string Name { get; private set; }
		public string SpecialName { get; private set; }
		public string Description { get; private set; }
		public TelegramCommandParameter[] Parameters { get; private set; }

		public TelegramCommand()
		{
			Name = GetCommandName();
			SpecialName = "/" + Name;
			Parameters = CreateParameters();
			Description = GenerateDescription();
		}

		public virtual bool IsPrivate()
		{
			return false;
		}

		public virtual bool IsSilently()
		{
			return false;
		}

		public void Execute(ChatUser user, string[] inputArguments)
		{
			string checkResult;
			if (!IsArgumentsValid(inputArguments, out checkResult))
			{
				SendMessage(user.Id, string.Format(TelegramMessages.InvalidArguments, checkResult));
				return;
			}

			ExecuteImpl(user, inputArguments);
		}

		protected virtual void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			SendMessage(user.Id, $"Command: {SpecialName} not implemented!");
		}

		protected void SendMessage(int userId, string message)
		{
			if (!IsSilently())
			{
				Telegram.SendTextMessageAsync(userId, message);
			}
		}

		private bool IsArgumentsValid(string[] inputArguments, out string result)
		{
			int parametersLength = Parameters == null ? 0 : Parameters.Length;
			int invalidCount = 0;

			int[] indexes = new int[parametersLength];

			for (int i = 0; i < parametersLength; i++)
			{
				if (i > inputArguments.Length - 1 || String.IsNullOrEmpty(inputArguments[i]))
				{
					indexes[invalidCount++] = i + 1;
				}
			}

			if (invalidCount > 0)
			{
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < invalidCount; i++)
				{
					int invalidIndex = indexes[i];

					builder.Append('[');
					builder.Append(i);
					builder.Append(']');

					if (invalidIndex < invalidCount)
					{
						builder.Append(", ");
					}
				}

				result = builder.ToString();
			}
			else
			{
				result = "";
			}

			return invalidCount == 0;
		}

		private string GenerateDescription()
		{
			StringBuilder builder = new StringBuilder();
			TelegramCommandParameter[] parameters = Parameters;
			builder.Append(SpecialName);

			if (parameters != null)
			{
				for (int j = 0; j < parameters.Length; j++)
				{
					builder.Append(" [");
					builder.Append(j);
					builder.Append("]");
				}
			}

			builder.Append(" - ");
			builder.Append(GetDescription());

			if (parameters != null && parameters.Length > 0)
			{
				builder.AppendLine("");
				builder.Append("Example: ");
				builder.Append(SpecialName);
				for (int j = 0; j < parameters.Length; j++)
				{
					TelegramCommandParameter parameter = parameters[j];
					builder.Append(' ');
					builder.Append(parameter.Example);
				}
				builder.Append(".");
			}

			return builder.ToString();
		}

		protected abstract string GetCommandName();
		protected abstract string GetDescription();
		protected abstract TelegramCommandParameter[] CreateParameters();
	}
}