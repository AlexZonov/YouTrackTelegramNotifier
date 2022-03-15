using com.config;
using com.database;
using com.services.telegram;
using com.services.youtrack;
using DryIoc;

namespace com.main.application
{
	internal class App : IApplication
	{
		public static Config Config { get; private set; }
		public static Database Database { get; private set; }
		public static YouTrackService YouTrack { get; private set; }
		public static TelegramService Telegram { get; private set; }
		private static WebService Web { get; set; }
		
		private static IContainer _container;

		public void Run()
		{
			_container = new Container(rules => rules.WithDefaultReuse(Reuse.Singleton));
			_container.Register<WebService>();
			_container.Register<YouTrackService>();
			_container.Register<TelegramService>();
			_container.Register<IConfig, Config>(Made.Of(() => Config.Load(Arg.Of<IContainer>())));
			_container.Register<Database>(Made.Of(() => Database.Load()));

			Config = _container.Resolve<IConfig>() as Config;
			Database = _container.Resolve<Database>();

			Web = _container.Resolve<WebService>();
			YouTrack = _container.Resolve<YouTrackService>();
			Telegram = _container.Resolve<TelegramService>();
		}
	}
}