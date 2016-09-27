using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaRun.GlobalExceptionHandling
{
    public class TextFileLogger : IExceptionLogger
    {
        private string _loggingFolder;

        public TextFileLogger(string loggingFolder)
        {
            _loggingFolder = loggingFolder;
        }

        public void Log(Exception ex)
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrWhiteSpace(ex.InnerException?.ToString()))
                errorMessage += $"{ex.InnerException}";

            errorMessage += $"\n\n{ex.Message}";

            errorMessage += "\n\nSee error log file for further information.";

            var fileName = "Error[" + DateTime.Now.ToString("dd.MM.yyyy-HH_mm_ss") + "].txt";

            if (!Directory.Exists(_loggingFolder))
                Directory.CreateDirectory(_loggingFolder);

            TextWriter writer = new StreamWriter(Path.Combine(_loggingFolder, fileName));
            writer.WriteLine("-------------- Exception --------------\n\n");
            writer.WriteLine(ex.Message);
            writer.WriteLine("\n\n-------------- Inner Exception --------------\n\n");
            writer.WriteLine(ex.InnerException);
            writer.WriteLine("\n\n-------------- Stack Trace --------------\n\n");
            writer.WriteLine(ex.StackTrace);
            writer.Close();
        }
    }
}
