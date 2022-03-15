namespace com.config
{
	internal interface IConfig
	{
		YouTrackConfig YouTrack { get; }
		TelegramConfig Telegram { get; }
		WebServerConfig WebServer { get; }
		LoggingConfig Logging { get; }
	}
}