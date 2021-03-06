﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DirStats.Data;
using DirStats.Enums;

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
            // 3. Total size of all exe files
            // 4. Print all exe files
            // 5. Print all files with size bigger than 3 000 000 bytes - use linq
            // 6. Print all files from folders starting with C
            // 7. Total size size of all hidden files
            // 8. First file with size greater than 15 000 000 bytes
            // 9. Print first 3 and last 3 first level folders sorted by the 
            //    total size of all files it contains
            // 10. Print files paged by page size defined by user in a specific folder


            // Not working because of ACL
            //var directories = Directory.GetDirectories("c:\\Program Files", "*", SearchOption.AllDirectories);

            // This is the way to go
            //var topDirectories = Directory.GetDirectories("c:\\Program Files");

            Stopwatch stopwatch = Stopwatch.StartNew();

            List<string> subdirs = new List<string>(2000);

            string rootFolder = @"C:\Program files";
            getSubdirs(rootFolder, subdirs);

            var filesList = getFilesList(subdirs);

            Console.WriteLine($"Folders: {subdirs.Count}");
            Console.WriteLine($"Files: {filesList.Count}");

            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");

            stopwatch.Restart();
            var files = getFiles(filesList);
            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");


            //var azDict = getAzDict();
            //analyzeAZFiles(files, azDict);

            //sumOfAllExeFiles(files);

            //printAllExeFiles(files);

            //var myFiles = convertToMyFilesLinq(files);

            //printAllFilesInDirStartWithFromFilePath(files, "Co");
            //printTopBiggestFiles(files, 20);



            var dirs = Directory.GetDirectories(rootFolder);
                //.Select(d => new DirectoryInfo(d)).ToList(); // not needed

            Console.WriteLine("Print first 3 and last 3 first level folders");
            stopwatch.Restart();
            printFirstLast3Folders(dirs, files);
            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");


        }

        private static void analyzeAZFiles(List<FileInfo> files, Dictionary<char, List<FileInfo>> azDict)
        {
            foreach (FileInfo fileInfo in files)
            {
                char key = fileInfo.Name.ToUpper()[0];

                if (azDict.ContainsKey(key))
                    azDict[key].Add(fileInfo);
                else
                {
                    azDict.Add(key, new List<FileInfo>(1000));
                    azDict[key].Add(fileInfo);
                }
            }
        }

        private static Dictionary<string, List<FileInfo>> analyzeByExtension(List<FileInfo> files)
        {
            Dictionary<string, List<FileInfo>> countsByExt = new Dictionary<string, List<FileInfo>>();

            foreach (var file in files)
            {
                if (!countsByExt.ContainsKey(file.Extension))
                    countsByExt.Add(file.Extension, new List<FileInfo>(1000));

                countsByExt[file.Extension].Add(file);
            }

            return countsByExt;
        }

        private static long sumOfAllExeFilesPlain(List<FileInfo> files)
        {
            long sum = 0;
            foreach (var file in files)
            {
                if (file.Extension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase))
                    sum += file.Length;
            }
            return sum;
        }

        private static long sumOfAllExeFiles(List<FileInfo> files)
        {
            return sumOfAllFilesLinq(files, FileExtensions.ExeFiles);
        }

        private static long sumOfAllFilesLinq(List<FileInfo> files, string extension)
        {
            //with intermediate results
            //var exeFiles = files.Where(file => file.Extension.Equals(extension)).ToList();
            //var sum = exeFiles.Sum(file=>file.Length);

            
            //chained methods
            return files
                .Where(x=>x.Extension.Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                .Sum(x=>x.Length);
        }

        private static void printAllExeFiles(List<FileInfo> files)
        {
            printAllFiles(files, FileExtensions.ExeFiles);
        }

        private static void printAllFiles(List<FileInfo> files, string extension)
        {
            files
                .Where(f=>f.Extension.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .ForEach(f =>
                {
                    Console.WriteLine($"File: {f.Name}, Length: {f.Length}");
                });    
        }

        private static void printAllExeMyFiles(List<MyFileInfo> files)
        {
            printAllMyFiles(files, FileExtensions.ExeFiles);
        }

        private static void printAllMyFiles(List<MyFileInfo> files, string extension)
        {
            files
                .Where(f => f.Extension.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .ForEach(Console.WriteLine);
        }

        private static void printAllFilesInDirStartWith(List<FileInfo> files, string startsWith)
        {
            files.Where(x=>x.Directory.Name.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
                .ToList()
                .ForEach(Console.WriteLine);
        }

        private static void printAllFilesInDirStartWithFromFilePath(List<FileInfo> files, string startsWith)
        {
            files.Where(x =>
            {
                var firstSub = x.FullName.Substring(0, x.FullName.LastIndexOf('\\'));
                var dir = firstSub.Substring(firstSub.LastIndexOf('\\') + 1);
                return dir.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase);
            })
            .ToList()
            .ForEach(Console.WriteLine);

        }
        private static void printTopBiggestFiles(List<FileInfo> files, int topX)
        {
            files
            .OrderByDescending(x=>x.Length) // sort all files by its size - descending order (for ascending order use OrderBy)
            //.Skip(20) - this would skip first 20 records in the collection
            .Take(topX) // take only first topX records, if this Skip(20) is used takes records 21 + topX, if possible of course
            .ToList()
            .ForEach(x=>Console.WriteLine($"{x.Length.ToString("### ### ### ###")} - {x.Name}")); // display data in custom number format, numbers split to groups of 3
        }




        private static long totalSizeOfHiddenFiles(List<FileInfo> files)
        {
            return files.Where(x => x.Attributes == FileAttributes.Hidden).Sum(x => x.Length);
        }

        private static FileInfo firstFileWithSizeGreaterThan(List<FileInfo> files, long size)
        {
            return files.FirstOrDefault(x => x.Length > size);
        }


        private static List<MyFileInfo> convertToMyFilesPlain(List<FileInfo> files)
        {
            List<MyFileInfo> myFiles = new List<MyFileInfo>(files.Count);

            foreach (var file in files)
            {
                MyFileInfo my = new MyFileInfo();
                my.Name = file.Name;
                my.Length = file.Length;
                my.Created = file.CreationTime;
                my.Extension = file.Extension;

                myFiles.Add(my);
            }

            return myFiles;
        }

        private static List<MyFileInfo> convertToMyFilesLinq(List<FileInfo> files)
        {
            //not necessary, use Select
            //List<MyFileInfo> myFiles = new List<MyFileInfo>(files.Count);

            //files.ForEach(file =>
            //{
            //    MyFileInfo my = new MyFileInfo();
            //    my.Name = file.Name;
            //    my.Length = file.Length;
            //    my.Created = file.CreationTime;
            //    my.Extension = file.Extension;

            //    myFiles.Add(my);
            //});

            //return myFiles;

            return files.Select(file =>
            {
                MyFileInfo my = new MyFileInfo();
                my.Name = file.Name;
                my.Length = file.Length;
                my.Created = file.CreationTime;
                my.Extension = file.Extension;

                return my;
            }).ToList();
        }

        private static void printAllFilesBigger3MBs(List<MyFileInfo> files)
        {
            files.Where(x => x.Length > 3000000).ToList().ForEach(Console.WriteLine);
        }
       

        // 9. Print first 3 and last 3 first level folders sorted by the 
        //    total size of all files it contains
        private static List<FileInfo> getFilesInFolder(string dir, List<FileInfo> files)
        {
            return files.Where(x => x.FullName.StartsWith(dir, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
        // 9. Print first 3 and last 3 first level folders sorted by the 
        //    total size of all files it contains
        private static void printFirstLast3Folders(string[] dirs, List<FileInfo> files)
        {
            //var firstLevelFolders = getFirstLevelFolders(rootFolder, dirs);

            var folders = dirs.Select(d =>
            {
                var filesInFolder = getFilesInFolder(d, files);
                var sum = filesInFolder.Sum(f=>f.Length);

                return new {Name = d, TotalSize = sum};
            }).ToList();
            

            folders.OrderByDescending(f=>f.TotalSize)
                .Take(3)
                .ToList()
                .ForEach((f) =>Console.WriteLine($"Folder: {f.Name} \tSize: {f.TotalSize}"));

            folders.OrderByDescending(f => f.TotalSize)
                .Skip(folders.Count-3)
                //.Take(3)
                .ToList()
                .ForEach((f) => Console.WriteLine($"Folder: {f.Name} \tSize: {f.TotalSize}"));

        }









        /// <summary>
        /// Ondrej's version of how to replace Dictionary(char, int)
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static List<int> AZThroughIfs(List<FileInfo> files)
        {
            int a = 0;
            int b = 0;
            int c = 0;

            // ints for other letters

            int z = 0;

            foreach (var file in files)
            {
                if (file.Name.StartsWith("A") || file.Name.StartsWith("a"))
                    a++;
                if (file.Name.StartsWith("B") || file.Name.StartsWith("b"))
                    b++;
                if (file.Name.StartsWith("C") || file.Name.StartsWith("c"))
                    c++;

                // ifs for other letters

                if (file.Name.StartsWith("C") || file.Name.StartsWith("c"))
                    z++;
            }

            List<int> counts = new List<int>();

            counts.Add(a);
            counts.Add(b);
            counts.Add(c);
            counts.Add(z);

            return counts;
        }

        /// <summary>
        /// Albert's version of how to replace Dictionary(char, int)
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static int[] AZThroughArray(List<FileInfo> files)
        {
            int[] chararr = new int[26];

            foreach (var file in files)
            {
                var firstChar = file.Name.ToUpper()[0];

                if (firstChar >= 'A' && firstChar <= 'Z')
                {
                    chararr[firstChar - 'A']++;
                }
            }

            return chararr;
        }

        /// <summary>
        /// Filip Jakab's solution of how to replace Dictionary(char, int)
        /// </summary>
        /// <param name="filesInfos"></param>
        /// <returns></returns>
        private static List<int> AZThroughList(List<FileInfo> filesInfos)
        {
            List<int> fileCount = new List<int>(26);

            for (int i = 0; i < 25; i++)
            {
                fileCount.Add(0);
            }

            for (int i = 0; i < 25; i++)
            {
                foreach (FileInfo file in filesInfos)
                {
                    if (file.Name[0] == (char)(i + 'A'))
                    {
                        fileCount[i]++;
                    }
                }
            }
            return fileCount;
        }

        /// <summary>
        /// Jakub's structure to keep the track of counts
        /// </summary>
        struct FileCount
        {
            public char Letter;
            public int count;
        }

        /// <summary>
        /// Jakub's version of how to replace Dictionary(char, int)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static List<FileCount> AZThroughStructure(List<FileInfo> info)
        {
            List<FileCount> fileCounts = new List<FileCount>(26);

            FileCount f = new FileCount();
            f.Letter = 'A';
            f.count = 0;
            fileCounts.Add(f);
            for (int i = 0; i < 25; i++)
            {
                FileCount ff = new FileCount();
                ff.Letter = ++f.Letter;
                ff.count = 0;
                fileCounts.Add(ff);
            }
            for (int i = 0; i < info.Count; i++)
            {
                for (int j = 0; j < fileCounts.Count; j++)
                {
                    if (info[i].Name.ToUpper().ToCharArray()[0] == fileCounts[j].Letter)
                    {
                        FileCount temp = new FileCount();
                        temp.Letter = fileCounts[j].Letter;
                        temp.count = fileCounts[j].count + 1;
                        fileCounts[j] = temp;
                    }
                }
            }
            return fileCounts;
        }

        /// <summary>
        /// Maria's version of how to replace Dictionary(char, int)
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
//        private static List<int> AZThroughSwitch(List<FileInfo> files)
//        {
//            List<int> pocty = new List<int>(26);
//            int a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0, u = 0, v = 0, w = 0, x = 0, y = 0, z = 0;
//​
//            foreach (FileInfo nameFileInfo in files)
//            {
//                var selektor = nameFileInfo.Name[0];
//                Convert.ToChar(selektor);
//                selektor = Char.ToUpper(selektor);

//                switch (selektor)
//                {
//                    case 'A':
//                        {
//                            a++;
//                        }
//                        break;
//​
//                    case 'B':
//                        {
//                            b++;
//                        }
//                        break;
//​
//                    case 'C':
//                        {
//                            c++;
//                        }
//                        break;
//​
//                    case 'D':
//                        {
//                            d++;
//                        }
//                        break;
//​
//                    case 'E':
//                        {
//                            e++;
//                        }
//                        break;
//​
//                    case 'F':
//                        {
//                            f++;
//                        }
//                        break;
//​
//                    case 'G':
//                        {
//                            g++;
//                        }
//                        break;
//​
//                    case 'H':
//                        {
//                            h++;
//                        }
//                        break;
//​
//                    case 'I':
//                        {
//                            i++;
//                        }
//                        break;
//​
//                    case 'J':
//                        {
//                            j++;
//                        }
//                        break;
//​
//                    case 'K':
//                        {
//                            k++;
//                        }
//                        break;
//​
//                    case 'L':
//                        {
//                            l++;
//                        }
//                        break;
//​
//                    case 'M':
//                        {
//                            m++;
//                        }
//                        break;
//                    case 'N':
//                        {
//                            n++;
//                        }
//                        break;
//​
//                    case 'O':
//                        {
//                            o++;
//                        }
//                        break;
//​
//                    case 'P':
//                        {
//                            p++;
//                        }
//                        break;
//​
//                    case 'Q':
//                        {
//                            q++;
//                        }
//                        break;
//​
//                    case 'R':
//                        {
//                            r++;
//                        }
//                        break;
//​
//                    case 'S':
//                        {
//                            s++;
//                        }
//                        break;
//​
//                    case 'T':
//                        {
//                            t++;
//                        }
//                        break;
//​
//                    case 'U':
//                        {
//                            u++;
//                        }
//                        break;
//​
//                    case 'V':
//                        {
//                            v++;
//                        }
//                        break;
//​
//                    case 'W':
//                        {
//                            w++;
//                        }
//                        break;
//​
//                    case 'X':
//                        {
//                            x++;
//                        }
//                        break;
//​
//                    case 'Y':
//                        {
//                            y++;
//                        }
//                        break;
//​
//                    case 'Z':
//                        {
//                            z++;
//                        }
//                        break;
//​
//                }
//            }
//​
//​
//            pocty.Add(a);
//            pocty.Add(b);
//            pocty.Add(c);
//            pocty.Add(d);
//            pocty.Add(e);
//            pocty.Add(f);
//            pocty.Add(g);
//            pocty.Add(h);
//            pocty.Add(i);
//            pocty.Add(j);
//            pocty.Add(k);
//            pocty.Add(l);
//            pocty.Add(m);
//            pocty.Add(n);
//            pocty.Add(o);
//            pocty.Add(p);
//            pocty.Add(q);
//            pocty.Add(r);
//            pocty.Add(s);
//            pocty.Add(t);
//            pocty.Add(u);
//            pocty.Add(v);
//            pocty.Add(w);
//            pocty.Add(x);
//            pocty.Add(y);
//            pocty.Add(z);
//​
//​
//            return pocty;
//        }


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
