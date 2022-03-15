using System.IO;
using com.utilities;
using DryIoc;
using Newtonsoft.Json;

namespace com.config
{
	[JsonObject(MemberSerialization.OptIn)]
	internal class Config : IConfig
	{
		public const string FILE_NAME = "config.json";

		[JsonProperty("version")]
		private int _version = 1;

		[JsonProperty("youtrack")]
		public YouTrackConfig YouTrack { get; private set; } = new YouTrackConfig();

		[JsonProperty("telegram")]
		public TelegramConfig Telegram { get; private set; } = new TelegramConfig();

		[JsonProperty("web_server")]
		public WebServerConfig WebServer { get; private set; } = new WebServerConfig();

		[JsonProperty("logging")]
		public LoggingConfig Logging { get; private set; } = new LoggingConfig();

		public static Config Load(IContainer container)
		{
			Config instance = File.Exists(FILE_NAME) ? JsonFilesUtilities.Load<Config>(FILE_NAME) : new Config();
			container.Use(instance.YouTrack);
			container.Use(instance.Telegram);
			container.Use(instance.Telegram.Proxy);
			container.Use(instance.WebServer);
			container.Use(instance.Logging);

			Save(instance);
			return instance;
		}

		private static void Save(Config config)
		{
			JsonFilesUtilities.Save(FILE_NAME, config);
		}
	}
}