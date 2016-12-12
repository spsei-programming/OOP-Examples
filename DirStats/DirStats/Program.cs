using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DirStats
{
	class Program
	{
		static void Main(string[] args)
		{
			// Create stats for a directory
			// Stats as follow:
			// 1. Count of files starting with letter A-Z
			// 2. Count of files by extension
			// 3. Sum of all exe files

			// Not working because of ACL
			//var directories = Directory.GetDirectories("c:\\Program Files", "*", SearchOption.AllDirectories);

			// This is the way to go
			//var topDirectories = Directory.GetDirectories("c:\\Program Files");

			Stopwatch stopwatch = Stopwatch.StartNew();

			List<string> subdirs = new List<string>(2000);

			getSubdirs(@"C:\Program files", subdirs);

			var filesList = getFilesList(subdirs);

			Console.WriteLine($"Folders: {subdirs.Count}");
			Console.WriteLine($"Files: {filesList.Count}");

			stopwatch.Stop();
			Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");

			stopwatch.Restart();
			var files = getFiles(filesList);
			stopwatch.Stop();
			Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");

			var azDict = getAzDict();

			analyzeAZFiles(files, azDict);
		}

		private static void analyzeAZFiles(List<FileInfo> files, Dictionary<char, List<FileInfo>> azDict)
		{
			foreach (FileInfo fileInfo in files)
			{
#warning Not working in all cases, watchout
				azDict[fileInfo.Name.ToUpper()[0]].Add(fileInfo);
			}
		}


		private static string[] getSubdirs(string directory, List<string> subdirs)
		{
			try
			{
				var directories = Directory.GetDirectories(directory);
				if (directories.Length == 0) return new string[] { };

				foreach (var dir in directories)
				{
					subdirs.Add(dir);
					getSubdirs(dir, subdirs);
				}

				return directories;
			}
			catch (Exception)
			{
				return new string[] { };
			}
		}

		private static List<string> getFilesList(List<string> dirs)
		{
			List<string> files = new List<string>(10000);
			foreach (var dir in dirs)
			{
				try
				{
					files.AddRange(Directory.GetFiles(dir));
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Could not read files from dir {dir} with message {ex.Message}");
				}
			}

			return files;
		}

		private static List<FileInfo> getFiles(List<string> filePaths)
		{
			List<FileInfo> files = new List<FileInfo>();
			foreach (var filePath in filePaths)
			{
				files.Add(new FileInfo(filePath));
			}

			return files;
		}

		private static Dictionary<char, List<FileInfo>> getAzDict()
		{
			Dictionary<char, List<FileInfo>> dictionary = new Dictionary<char, List<FileInfo>>();

			for (int i = 'A'; i < 'Z'; i++)
			{
				dictionary.Add((char)i, new List<FileInfo>(1000));
			}

			return dictionary;
		}
	}
}
