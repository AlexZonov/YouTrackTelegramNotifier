using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using com.main;
using com.main.application;
using com.services.telegram.dialogs;
using com.services.telegram.messaging;
using com.services.telegram.youtrack;
using Newtonsoft.Json;

namespace com.database
{
	[JsonObject(MemberSerialization.OptIn)]
	public class LinkedUser
	{
		[JsonProperty("login")]
		public string YouTrackLogin { get; private set; }

		[JsonProperty("chat_id")]
		public int TelegramChatId { get; private set; }

		[JsonProperty("is_tester")]
		public bool IsTester { get; private set; }

		[JsonProperty("notification_settings")]
		public Dictionary<int, bool> NotificationSettings { get; private set; } = new Dictionary<int, bool>();

		public LinkedUser(string youTrackLogin, int telegramChatId)
		{
			InternalUpdate(youTrackLogin, telegramChatId);
			TrySetDefaultNotificationSettings();
		}

		public void Update(string youTrackLogin, int telegramChatId)
		{
			InternalUpdate(youTrackLogin, telegramChatId);
			Save();
		}

		public void ReplaceLogin(string youTrackLogin)
		{
			InternalUpdate(youTrackLogin, TelegramChatId);
			Save();
		}

		public void SetTester(bool value)
		{
			IsTester = value;
			Save();
		}

		public void SetNotificationEnable(YouTrackNotificationType notificationType, bool enable)
		{
			NotificationSettings[(int)notificationType] = enable;
			Save();
		}

		public bool IsNotificationEnabled(YouTrackNotificationType notificationType)
		{
			bool result;
			if (!NotificationSettings.TryGetValue((int)notificationType, out result))
			{
				result = false;
			}

			return result;
		}

		private void Save()
		{
			Database.Save(App.Database);
		}

		private void InternalUpdate(string youTrackLogin, int telegramChatId)
		{
			YouTrackLogin = youTrackLogin;
			TelegramChatId = telegramChatId;
		}

		[OnDeserialized]
		private void OnDeserialize(StreamingContext context)
		{
			TrySetDefaultNotificationSettings();
		}

		private void TrySetDefaultNotificationSettings()
		{
			int[] typesValues = (int[]) Enum.GetValues(typeof(YouTrackNotificationType));
			for (int i = 0; i < typesValues.Length; i++)
			{
				int typeValue = typesValues[i];
				YouTrackNotificationType typeEnumValue = (YouTrackNotificationType) typeValue;

				if (!NotificationSettings.ContainsKey(typeValue))
				{
					NotificationSettings[typeValue] = true;
				}
			}
		}
	}
}