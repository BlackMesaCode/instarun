﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InstaRun.ContextMenuManagement
{
    [XmlInclude(typeof(Executable))]
    [XmlInclude(typeof(Separator))]
    public class Container : Item
    {
        public string Name { get; set; }
        public List<Item> Items { get; set; }

        public Container()
        {

        }

        public Container(string name, List<Item> items)
        {
            Name = name;
            Items = items;
        }
    }
}
