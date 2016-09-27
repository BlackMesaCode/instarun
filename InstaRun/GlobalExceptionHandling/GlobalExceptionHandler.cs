using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InstaRun.GlobalExceptionHandling
{
    class GlobalExceptionHandler
    {
        public List<IExceptionLogger> ExceptionLoggers = new List<IExceptionLogger>();

        public GlobalExceptionHandler(Dispatcher dispatcher)
        {
            dispatcher.UnhandledException += OnDispatcherUnhandledException; // will only handle exceptions thrown on the main UI thread in a WPF application
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;  // will handle any exceptions thrown on any thread 
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUncaughtException((Exception)e.ExceptionObject);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleUncaughtException(e.Exception);
        }

        private void HandleUncaughtException(Exception exception)
        {
            foreach (var exceptionLogger in ExceptionLoggers)
            {
                exceptionLogger.Log(exception); 
            }
            Application.Current.Shutdown();
        }
    }
}
