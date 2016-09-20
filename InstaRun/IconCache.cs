using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.IconLib;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaRun
{
    public static class IconCache
    {

        public static void BuildCache(List<Item> items)
        {
            if (!Directory.Exists(App.PathToIconCache))
                Directory.CreateDirectory(App.PathToIconCache);

            foreach (var item in items)
            {
                if (item.GetType() == typeof(Executable))
                {
                    var executable = item as Executable;

                    if (!executable.IsInGlobalPath) // No icons for global path calls possible - we would have to search all the directories in the PATH variable
                    {
                        if (File.Exists(executable.Path))
                        {
                            var outputPath = Path.Combine(App.PathToIconCache, executable.Name + ".ico");

                            //Icon.ExtractAssociatedIcon(executable.Path).ToBitmap().Save(outputPath);

                            Icon icon = Icon.ExtractAssociatedIcon(executable.Path);
                            MultiIcon mIcon = new MultiIcon();
                            SingleIcon sIcon = mIcon.Add("whatever");
                            sIcon.CreateFrom(icon.ToBitmap(), IconOutputFormat.Vista);
                            sIcon.Save(outputPath);

                            //using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                            //{
                            //    Icon.ExtractAssociatedIcon(executable.Path).Save(fileStream);
                            //}
                        }
                        else if (Directory.Exists(executable.Path))
                        {
                            var outputPath = Path.Combine(App.PathToIconCache, executable.Name + ".ico");

                            Icon icon = IconReceiver.ReceiveIcon(executable.Path, false);
                            MultiIcon mIcon = new MultiIcon();
                            SingleIcon sIcon = mIcon.Add("whatever");
                            sIcon.CreateFrom(icon.ToBitmap(), IconOutputFormat.Vista);
                            sIcon.Save(outputPath);

                            //using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                            //{
                            //    IconReceiver.ReceiveIcon(executable.Path, false).Save(fileStream);
                            //}
                        }
                    }
                }
                else if (item.GetType() == typeof(Container))
                {
                    var container = item as Container;
                    BuildCache(container.Items);
                }
            }
        }

    }
}
