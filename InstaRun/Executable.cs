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
        public string Path { get; set; }

        public Executable()
        {

        }

        public Executable(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
