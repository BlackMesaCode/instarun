using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstaRun.ConfigManagement
{
    public class FileWatcherService
    {
        public event Action OnChangedAndNotLocked;
        private string _fileToWatch;

        public FileWatcherService(string fileToWatch)
        {
            _fileToWatch = fileToWatch;

            CreateFileSystemWatcher(Path.GetDirectoryName(_fileToWatch), Path.GetFileName(_fileToWatch));
        }

        private void CreateFileSystemWatcher(string folder, string filter)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = folder;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = filter;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }


        private bool IsFileUnlocked()
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(_fileToWatch, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (inputStream.Length > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        private async void CheckIfFileUnlocked()
        {
            if (IsFileUnlocked())
            {
                OnChangedAndNotLocked?.Invoke();
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                CheckIfFileUnlocked();
            }
        }

        private Task _checkIfFileUnlockedTask;

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Start background polling task, if such a task isnt already running
            if (_checkIfFileUnlockedTask == null || _checkIfFileUnlockedTask.IsCompleted)
                _checkIfFileUnlockedTask = Task.Run(() => CheckIfFileUnlocked());
        }

    }
}
