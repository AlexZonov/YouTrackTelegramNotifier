using System.Net;
using System.Net.Http;
using com.config;
using com.services.youtrack.rest;
using Newtonsoft.Json;

namespace com.services.youtrack
{
	internal class YouTrackREST
	{
		private YouTrackConfig _config;
		private YouTrackHttpClient _httpClient;

		public YouTrackREST(YouTrackConfig config)
		{
			_config = config;
			_httpClient = new YouTrackHttpClient();
		}

		public bool TryGetUser(string userLogin, out YouTrackRESTUser user)
		{
			user = null;

			string url = $"{_config.Api.UserCheckUrl}{userLogin}";

			HttpResponseMessage response = _httpClient.GetAsync(url).Result;
			string responseContent = response.Content.ReadAsStringAsync().Result;

			if(response.StatusCode == HttpStatusCode.OK)
			{
				user = JsonConvert.DeserializeObject<YouTrackRESTUser>(responseContent);
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}