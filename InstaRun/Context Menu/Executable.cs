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
                if (IsInGlobalPath)
                    return path;
                return System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(path));
            }
            set { path = value; }
        }

        public bool IsInGlobalPath { get; set; }
        public string Arguments { get; set; }

        public Executable()
        {

        }

        public Executable(string name, string path, bool isGlobalPath = false, string arguments = "")
        {
            Name = name;
            Path = path;
            IsInGlobalPath = isGlobalPath;
            Arguments = arguments;
        }

    }
}
