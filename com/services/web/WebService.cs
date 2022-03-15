using System;
using System.IO;
using System.Net;
using System.Text;
using com.config;
using com.main.logger;

namespace com.services.telegram
{
	internal class WebService
	{
		public event Action<string> OnReceivedTextData = (string textData) => { };

		private WebServerConfig _config;
		private MultiThreadHttpServer _server;

		public WebService(WebServerConfig config)
		{
			_config = config;
			_server = CreateServer();
		}

		private MultiThreadHttpServer CreateServer()
		{
			MultiThreadHttpServer server = new MultiThreadHttpServer(_config.MaxThreads);
			string listeningUrl = server.Start(_config.Address, _config.Port, _config.UrlPath);
			server.ProcessRequest += OnServerProcessRequest;
			Logger.Log($"Listening started on url:\"{listeningUrl}\" ...");
			return server;
		}

		private void OnServerProcessRequest(HttpListenerContext context)
		{
			HttpListenerRequest request = context.Request;

			int statusCode;
			bool isSupportedRequest = IsSupportedRequest(request, out statusCode);
			context.Response.StatusCode = statusCode;

			if (!isSupportedRequest)
			{
				Logger.Log($"[Reject request] Reason: {(HttpStatusCode) statusCode}");
				context.Response.Close();
				return;
			}

			int contentSize = (int)context.Request.ContentLength64;

			StreamReader streamReader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
			string content = streamReader.ReadToEnd();

			content = content.Trim(new char[] {' ', '\t', '\r', '\n'});
			Logger.Log($"[Receive] Length: {contentSize},\r\n\tjson: {content},\r\n\tescaped json: {content.Replace("\"", "\\\"")}");

			context.Response.Close();

			OnReceivedTextData(content);
		}

		private bool IsSupportedRequest(HttpListenerRequest request, out int statusCode)
		{
			bool isPostRequest = request.HttpMethod == "POST";
			if (!isPostRequest)
			{
				statusCode = (int)HttpStatusCode.MethodNotAllowed;
				return false;
			}

			bool isContentTypeJson = request.ContentType == "application/json";
			if (!isContentTypeJson)
			{
				statusCode = (int)HttpStatusCode.UnsupportedMediaType;
				return false;
			}

			string accessToken = request.Headers.Get(_config.AccessTokenHeaderKey);
			if (accessToken != _config.AccessToken)
			{
				statusCode = (int)HttpStatusCode.Unauthorized;
				return false;
			}

			long contentSize = request.ContentLength64;
			if (contentSize > _config.MaxContentSize)
			{
				statusCode = (int)HttpStatusCode.RequestEntityTooLarge;
				return false;
			}

			statusCode = (int)HttpStatusCode.OK;
			return true;
		}
	}
}