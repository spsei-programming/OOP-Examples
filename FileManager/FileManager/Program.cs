using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            // File manager is an application to copy/remove files from/to a directory
            // While doing operations on files user is always notified of what is going on

            // Copy requires a source directory, file mask and the destination directory
            // Removal requires just a source directory and file mask, but a prompt is 
            //   displayed to ask user if he/she is sure to remove all files

            //new DirectoryInfo(sourceDir).GetFiles().ToList().ForEach(f=>f.CopyTo(targetDir));

            Providers.FileManager manager = new Providers.FileManager();
            
            manager.CopyFiles(@"c:\Windows\System32\", "*.png;*.com", @"c:\temp\00000\", true);

            // NOT IN A MILLION YEARS!!!
            //manager.CopyFiles(new List<string>() { @"C:\windows\system32\inetcpl.cpl" }, @"c:\temp\00000\", true);

            manager.CopyFile(@"C:\windows\system32\inetcpl.cpl", @"c:\temp\00000\inetcpl.cpl", true);

            Console.ReadKey();
        }
    }
}
