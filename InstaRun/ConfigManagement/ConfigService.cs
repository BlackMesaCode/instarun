using InstaRun.ContextMenuManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace InstaRun.ConfigManagement
{
    public class ConfigService
    {
        public delegate void OnConfigChangedHandler(Config newConfig);
        public event OnConfigChangedHandler OnConfigChanged;

        private static string _exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        private static string _exeDir = Path.GetDirectoryName(_exePath);

        private static string _configFileName = "Config.xml";
        private static string _configPath = Path.Combine(_exeDir, _configFileName);

        private static string _sampleConfigFileName = "Config.Sample.xml";
        private static string _sampleConfigPath = Path.Combine(_exeDir, _sampleConfigFileName);

        private FileWatcherService _fileWatcherService;

        private Config _config;


        public ConfigService()
        {
            // Check if Config.xml exists
            if (!File.Exists(_configPath))
            {
                MessageBox.Show($"Couldn't find: {_configPath}\n\nProgram will be closed.");
                App.Current.Shutdown();
            }

            // Create SampleConfig.xml is not already existing
            CreateSampleConfigXml();

            // Read config.xml
            UpdateConfigFromXml();

            // Start watching for changes on the config.xml
            _fileWatcherService = new FileWatcherService(_configPath);
            _fileWatcherService.OnChangedAndNotLocked += _fileWatcherService_OnChangedAndNotLocked;
        }

        private void _fileWatcherService_OnChangedAndNotLocked()
        {
            UpdateConfigFromXml();
        }

        private Config CreateSampleConfig()
        {
            var settings = new Settings();
            var items = new List<Item>()
            {
                new Executable("Self", "self", @"InstaRun.exe"),
                new Executable("VSCode", "code, vscode", @"code", true, "."),
                new Executable("PowerShell", "powershell, shell", @"%SystemRoot%\system32\WindowsPowerShell\v1.0\powershell.exe"),
                new Executable("UserProfile", "userprofile", @"%USERPROFILE%"),
                new Separator(),
                new Container("Browsers", new List<Item>() {
                    new Executable("Internet Explorer (Google)", "ie,iexplore,internetexplorer", @"iexplore", true, "www.google.de"),
                    new Executable("Chrome", "chrome", @"chrome", true),
                    new Container("Test Browsers", new List<Item>() {
                        new Executable("Firefox", "firefox, ff", @"firefox", true),
                    }),
                }),
            };
            var config = new Config(settings, items);
            return config;
        }


        public void UpdateConfigFromXml()
        {
            _config = Deserialize<Config>(_configPath);
            OnConfigChanged?.Invoke(_config);
        }


        private void CreateSampleConfigXml()
        {
            if (!File.Exists(_sampleConfigPath))
            {
                var objectToSerialize = CreateSampleConfig();
                Serialize<Config>(objectToSerialize, _sampleConfigPath);
            }
        }


        private T Deserialize<T>(string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                T result = default(T);

                using (var reader = new StreamReader(path))
                {
                    result = (T)xs.Deserialize(reader);
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void Serialize<T>(T objectToSerialize, string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            var writerSettings = new XmlWriterSettings
            {
                Encoding = Encoding.Unicode,
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace,
            };

            using (var writer = XmlWriter.Create(path, writerSettings))
            {
                xs.Serialize(writer, objectToSerialize);
            }
        }


    }
}
