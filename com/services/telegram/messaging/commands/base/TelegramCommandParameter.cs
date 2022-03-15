namespace com.services.telegram.commands
{
	internal class TelegramCommandParameter
	{
		public string Description { get; private set; }
		public string Example { get; private set; }

		public TelegramCommandParameter(string description, string example)
		{
			Description = description;
			Example = example;
		}
	}
}