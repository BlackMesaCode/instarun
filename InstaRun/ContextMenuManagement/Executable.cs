using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InstaRun.ContextMenuManagement
{
    public class Executable : Item
    {
        public string Name { get; set; }
        public string MagicWords { get; set; }

        [XmlIgnore]
        public List<string> MagicWordsSplitted { get { return MagicWords.ToLower().Trim().Replace(" ", string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); } }

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

        public Executable(string name, string magicwords, string path, bool isGlobalPath = false, string arguments = "")
        {
            Name = name;
            MagicWords = magicwords;
            Path = path;
            IsInGlobalPath = isGlobalPath;
            Arguments = arguments;
        }

    }
}
