using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaRun
{
    public class Executable : Item
    {
        public string Name { get; set; }

        private string path;
        public string Path
        {
            get {
                if (IsGlobalPath)
                    return path;
                return System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(path));
            }
            set { path = value; }
        }

        public bool IsGlobalPath { get; set; }
        public string Arguments { get; set; }

        public Executable()
        {

        }

        public Executable(string name, string path, bool isGlobalPath = false, string arguments = "")
        {
            Name = name;
            Path = path;
            IsGlobalPath = isGlobalPath;
            Arguments = arguments;
        }

    }
}
