using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileManager.Providers
{
    public class FileManager
    {
        public event EventHandler<EventArgs> OnOperationStarted;

        public event EventHandler<EventArgs> OnOperationFinished;

        public string CopyFile(string from, string to, bool overwrite)
        {
           

            File.Copy(from, to, overwrite);

            return to;
        }

        public void RemoveFile(string from)
        {
            throw new NotImplementedException();
        }

        public void CopyFiles(List<string> filesToCopy, string targetDir, bool overwrite)
        {
            filesToCopy.ForEach(f=>
            {
                Console.WriteLine($"Copying file {f}");
                CopyFile(f
                    , Path.Combine(targetDir, f.Substring(f.LastIndexOf('\\') + 1))
                    , overwrite);
            });
        }

        public void CopyFiles(string from, string mask, string to, bool overwrite)
        {
            Stopwatch watch = new Stopwatch();

#warning *.prn;*.txt might not be working
            watch.Start();

            var masks = mask.Split(';');

            foreach (var m in masks)
            {
                List<string> files = Directory.GetFiles(from, m).ToList();

                CopyFiles(files, to, overwrite);
            }

            watch.Stop();
        }

        public void RemoveFiles(string from, string mask)
        {
            throw new NotImplementedException();
        }
    }
}