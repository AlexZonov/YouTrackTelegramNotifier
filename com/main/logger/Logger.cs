using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using com.utilities;

namespace com.main.logger
{
	internal class Logger
	{
		private static TraceSource _infoSource;
		private static TraceSource _errorSource;

		public static void Init()
		{
			_infoSource = new TraceSource("info_source", SourceLevels.Information);
			_errorSource = new TraceSource("error_source", SourceLevels.Error);

			string logFilePath = @"logs.log";
			string errorLogFilePath = @"error_logs.log";

			TryDeleteLogFile(@"nohup.out", 0f);

			Encoding outEncoding = Encoding.UTF8;

			StreamWriter fileLogStream = new StreamWriter(logFilePath, false, outEncoding) {AutoFlush = true};
			StreamWriter fileErrorLogStream = new StreamWriter(errorLogFilePath, false, outEncoding){AutoFlush = true};

			TraceListener fileLogListener = new TextWriterTraceListener(fileLogStream);
			TraceListener fileErrorLogListener = new TextWriterTraceListener(fileErrorLogStream);

			Console.OutputEncoding = outEncoding;
			Console.InputEncoding = outEncoding;
			TraceListener consoleLogListener = new TextWriterTraceListener(Console.Out);
			TraceListener consoleErrorListener = new TextWriterTraceListener(Console.Error);

			_infoSource.Listeners.Add(fileLogListener);
			_errorSource.Listeners.Add(fileErrorLogListener);

			_infoSource.Listeners.Add(consoleLogListener);
			_errorSource.Listeners.Add(consoleErrorListener);

			Log("------------------------------------------------------------------------------------------------------------------------");
		}

		private static void TryDeleteLogFile(string path, float maxMegabytes)
		{
			FileInfo logFileInfo = new FileInfo(path);
			if (logFileInfo.Exists)
			{
				float kilobytes = logFileInfo.Length / 1000f;
				float megabytes = kilobytes / 1000f;
				if (megabytes > maxMegabytes)
				{
					logFileInfo.Delete();
				}
			}
		}

		public static void Log(string message, object context = null)
		{
			StringBuilder builder = new StringBuilder();
			AppendBasePart(builder, context);
			builder.Append(message);

			_infoSource.TraceEvent(TraceEventType.Information, 0, builder.ToString());
		}

		public static void LogError(string message, object context = null)
		{
			StringBuilder builder = new StringBuilder();
			AppendBasePart(builder, context);
			builder.Append(message);

			_errorSource.TraceEvent(TraceEventType.Error, 0, builder.ToString());
		}

		public static void LogException(Exception exception, object context = null)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine(exception.ToString());

			LogError(builder.ToString());
		}

		private static void AppendBasePart(StringBuilder builder, object context)
		{
			builder.Append('[');
			builder.Append(DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss.fff"));
			builder.Append(']');
			if (context != null)
			{
				builder.Append('[');
				builder.Append(context.GetType().GetNiceName());
				builder.Append(']');
			}
			builder.Append(' ');
		}
	}
}