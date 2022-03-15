using com.database;
using com.utilities;

namespace com.services.telegram.commands
{
	internal class TelegramCancelCommand : TelegramCommand
	{
		public string PresentationName { get; private set; }

		public TelegramCancelCommand() : base()
		{
			PresentationName = Name.FirstCharToUpper();
		}

		protected override string GetCommandName()
		{
			return "cancel";
		}

		protected override string GetDescription()
		{
			return "cancel current dialog.";
		}

		protected override TelegramCommandParameter[] CreateParameters()
		{
			return null;
		}

		protected override void ExecuteImpl(ChatUser user, string[] inputArguments)
		{
			//empty
		}

		public override bool IsPrivate()
		{
			return true;
		}
	}
}