using System.Net.Http;
using System.Net.Http.Headers;
using com.config;
using com.main;
using com.main.application;

namespace com.services.youtrack
{
	internal class YouTrackHttpClient : HttpClient
	{
		private YouTrackConfig YouTrackConfig { get { return App.Config.YouTrack; }}

		public YouTrackHttpClient()
		{
			DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", YouTrackConfig.Token);
			DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
	}
}