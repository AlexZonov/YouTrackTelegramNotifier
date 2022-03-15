using com.config;
using MihaZupan;

namespace com.services.telegram
{
	internal class TelegramProxy : HttpToSocks5Proxy
	{
		public TelegramProxy(TelegramProxyConfig config) : base(config.Address, config.Port, config.Login, config.Password) 
		{
			// empty
		}
	}
}