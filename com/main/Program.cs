using System;
using System.Threading;
using com.main.application;
using com.main.logger;
using DryIoc;

namespace com.main
{
	internal static class Program
	{
		private static IContainer _container;
		
		public static void Main(string[] args)
		{
			Logger.Init();
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

			_container = new Container(rules => rules.WithDefaultReuse(Reuse.Singleton));
			_container.Register<IApplication, App>();

			using(var scope = _container.OpenScope())
			{
				IApplication application = scope.Resolve<IApplication>();
				application.Run();
			}

			while(true)
			{
				Thread.Sleep(1000000);
			}
		}
		
		private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			Exception exception = e.ExceptionObject as Exception;
			Logger.LogException(exception);
		}
	}
}