using com.main;
using com.main.application;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace com.database
{
	public class ChatUser
	{
		[JsonProperty("id")]
		public int Id { get; private set; }

		[JsonProperty("username")]
		public string Username { get; private set; }

		[JsonProperty("authorized")]
		public bool IsAuthorized { get; private set; }

		[JsonConstructor]
		private ChatUser() {}

		public ChatUser(User telegramUser)
		{
			Id = telegramUser.Id;
			Username = telegramUser.Username;
		}

		public void Authorize()
		{
			IsAuthorized = true;
			Save();
		}

		private void Save()
		{
			Database.Save(App.Database);
		}
	}
}