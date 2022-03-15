using DryIoc;

namespace com.services.youtrack
{
	internal class YouTrackService
	{
		public YouTrackREST REST { get; private set; }
		public YouTrackNotificationsReceiver NotificationsReceiver  { get; private set; }

		public YouTrackService(IContainer container)
		{
			container.Register<YouTrackREST>();
			container.Register<YouTrackNotificationsReceiver>();

			REST = container.Resolve<YouTrackREST>();
			NotificationsReceiver = container.Resolve<YouTrackNotificationsReceiver>();
		}
	}
}