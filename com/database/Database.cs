using System.Collections.Generic;
using System.Linq;
using com.utilities;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace com.database
{
	internal class Database
	{
		public const string FILE_NAME = "database.json";

		[JsonProperty("version")]
		private int _version = 1;

		[JsonProperty("members")]
		private Dictionary<int, ChatUser> _chatUsers = new Dictionary<int, ChatUser>();

		[JsonProperty("linked_users")]
		private Dictionary<int, LinkedUser> _linkedUsers = new Dictionary<int, LinkedUser>();

		public static Database Load()
		{
			if (File.Exists(FILE_NAME))
			{
				return JsonFilesUtilities.Load<Database>(FILE_NAME);
			}
			else
			{
				Database defaultDatabase = new Database();
				Save(defaultDatabase);
				return defaultDatabase;
			}
		}

		public static void Save(Database database)
		{
			JsonFilesUtilities.Save(FILE_NAME, database);
		}

		public bool TryGetLinkedUserByChatId(int telegramChatId, out LinkedUser linkedUser)
		{
			return _linkedUsers.TryGetValue(telegramChatId, out linkedUser);
		}

		public bool TryGetLinkedUsersByYoutrackLogin(string youTrackLogin, out List<LinkedUser> linkedUsers)
		{
			bool result = false;
			linkedUsers = new List<LinkedUser>();
			foreach (KeyValuePair<int, LinkedUser> linkedUserPair in _linkedUsers)
			{
				LinkedUser linkedUser = linkedUserPair.Value;
				if (linkedUser.YouTrackLogin == youTrackLogin)
				{
					linkedUsers.Add(linkedUser);
					result = true;
				}
			}
			return result;
		}

		public List<LinkedUser> GetAllLinkedUsers()
		{
			return _linkedUsers.Values.ToList();
		}

		public bool TryGetChatUserById(int id, out ChatUser chatUser)
		{
			return _chatUsers.TryGetValue(id, out chatUser);
		}

		public bool IsExistLinkedUserByChatId(int telegramChatId)
		{
			return _linkedUsers.ContainsKey(telegramChatId);
		}

		public Dictionary<int, ChatUser> GetAllChatUsers()
		{
			return _chatUsers;
		}

		public ChatUser TryAddChatUser(User user)
		{
			_chatUsers.TryGetValue(user.Id, out ChatUser chatUser);
			if (chatUser == null)
			{
				chatUser = new ChatUser(user);
				_chatUsers.Add(user.Id, chatUser);
				Save(this);
			}
			return chatUser;
		}

		public void AddLinkedUser(string youtrackLogin, int telegramChatId)
		{
			LinkedUser linkedUser = new LinkedUser(youtrackLogin, telegramChatId);
			_linkedUsers.Add(telegramChatId, linkedUser);
			Save(this);
		}

		public void RemoveLinkedUser(int telegramChatId)
		{
			_linkedUsers.Remove(telegramChatId);
			Save(this);
		}
	}
}