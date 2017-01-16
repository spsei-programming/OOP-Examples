using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FileManager.Providers
{
    public class FileManager
    {
        public event EventHandler<InformationEventArgs> OnOperationStarted;

        public event EventHandler<InformationEventArgs> OnOperationFinished;


        public string CopyFile(string from, string to, bool overwrite)
        {
            raiseOperationStarted($"Starting copying file {from}");
            
            //Console.WriteLine($"Copying file {from}");

            File.Copy(from, to, overwrite);

            raiseOperationFinished($"Finished copying file {from}");
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

        private void raiseOperationStarted(string message)
        {
            InformationEventArgs args = new InformationEventArgs() { Message = message };
            if (OnOperationStarted != null)
            {
                OnOperationStarted.Invoke(this, args);  
            }
        }

        private void raiseOperationFinished(string message)
        {
            InformationEventArgs args = new InformationEventArgs() {Message = message};
            if (OnOperationFinished != null)
            {
                OnOperationFinished.Invoke(this, args);
            }
        }
    }

    public class InformationEventArgs : EventArgs
    {
        public string Message { get; set; }  
    }
}