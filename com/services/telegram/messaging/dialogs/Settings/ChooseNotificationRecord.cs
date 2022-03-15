using com.services.telegram.youtrack;

namespace com.services.telegram.dialogs
{
	public class ChooseNotificationRecord
	{
		public string Id { get; private set; }
		public YouTrackNotificationType Type { get; private set; }
		public string Description { get; private set; }
		public string Presentation { get; private set; }

		public ChooseNotificationRecord(string id, YouTrackNotificationType type, string description)
		{
			Id = id;
			Type = type;
			Description = description;
			Presentation = $"{Id}. {Description}";
		}
	}
}