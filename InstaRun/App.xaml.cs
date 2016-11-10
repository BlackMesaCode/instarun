﻿using InstaRun.ConfigManagement;
using InstaRun.ContextMenuManagement;
using InstaRun.GlobalExceptionHandling;
using InstaRun.TriggerManagement;
using Ninject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InstaRun
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static readonly string ExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string ExeDir = Path.GetDirectoryName(ExePath);
        public static readonly string LogDirName = "ErrorLogs";
        public static readonly string LogDirPath = Path.Combine(ExeDir, LogDirName);

        private InstaRunService _instaRunService;
        private GlobalExceptionHandler _globalExceptionHandler;

        public App()
        {
            _globalExceptionHandler = new GlobalExceptionHandler(this.Dispatcher);
            _globalExceptionHandler.ExceptionLoggers.Add(new TextFileLogger(LogDirPath));
            _globalExceptionHandler.ExceptionLoggers.Add(new MessageBoxLogger());


            var kernel = new StandardKernel();

            kernel.Bind<ConfigService>().To<ConfigService>().InSingletonScope();
            kernel.Bind<ContextMenuService>().To<ContextMenuService>().InSingletonScope();
            kernel.Bind<TaskbarService>().To<TaskbarService>().InSingletonScope();
            kernel.Bind<HotkeyService>().To<HotkeyService>().InSingletonScope();
            kernel.Bind<KeyboardHook>().To<KeyboardHook>().InSingletonScope();

            _instaRunService = kernel.Get<InstaRunService>();
        }

 



    }
}
