using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InstaRun
{
    [XmlInclude(typeof(Settings))]
    [XmlInclude(typeof(Item))]
    [XmlInclude(typeof(Executable))]
    [XmlInclude(typeof(Seperator))]
    [XmlInclude(typeof(Container))]
    public class Config
    {
        public Settings Settings { get; set; }
        public List<Item> Items { get; set; }

        public Config()
        {

        }

        public Config(Settings settings, List<Item> items)
        {
            Settings = settings;
            Items = items;
        }
    }
}
