using System;

namespace com.main.logger
{
	internal interface ILogger
	{
		void Log(string message, object context = null);
		void LogError(string message, object context = null);
		void LogException(Exception exception, object context = null);
	}
}