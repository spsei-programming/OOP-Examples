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
        private static List<int> AZThroughSwitch(List<FileInfo> files)
        {
            List<int> pocty = new List<int>(26);
            int a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, h = 0, i = 0, j = 0, k = 0, l = 0, m = 0, n = 0, o = 0, p = 0, q = 0, r = 0, s = 0, t = 0, u = 0, v = 0, w = 0, x = 0, y = 0, z = 0;
​
            foreach (FileInfo nameFileInfo in files)
            {
                var selektor = nameFileInfo.Name[0];
                Convert.ToChar(selektor);
                selektor = Char.ToUpper(selektor);

                switch (selektor)
                {
                    case 'A':
                        {
                            a++;
                        }
                        break;
​
                    case 'B':
                        {
                            b++;
                        }
                        break;
​
                    case 'C':
                        {
                            c++;
                        }
                        break;
​
                    case 'D':
                        {
                            d++;
                        }
                        break;
​
                    case 'E':
                        {
                            e++;
                        }
                        break;
​
                    case 'F':
                        {
                            f++;
                        }
                        break;
​
                    case 'G':
                        {
                            g++;
                        }
                        break;
​
                    case 'H':
                        {
                            h++;
                        }
                        break;
​
                    case 'I':
                        {
                            i++;
                        }
                        break;
​
                    case 'J':
                        {
                            j++;
                        }
                        break;
​
                    case 'K':
                        {
                            k++;
                        }
                        break;
​
                    case 'L':
                        {
                            l++;
                        }
                        break;
​
                    case 'M':
                        {
                            m++;
                        }
                        break;
                    case 'N':
                        {
                            n++;
                        }
                        break;
​
                    case 'O':
                        {
                            o++;
                        }
                        break;
​
                    case 'P':
                        {
                            p++;
                        }
                        break;
​
                    case 'Q':
                        {
                            q++;
                        }
                        break;
​
                    case 'R':
                        {
                            r++;
                        }
                        break;
​
                    case 'S':
                        {
                            s++;
                        }
                        break;
​
                    case 'T':
                        {
                            t++;
                        }
                        break;
​
                    case 'U':
                        {
                            u++;
                        }
                        break;
​
                    case 'V':
                        {
                            v++;
                        }
                        break;
​
                    case 'W':
                        {
                            w++;
                        }
                        break;
​
                    case 'X':
                        {
                            x++;
                        }
                        break;
​
                    case 'Y':
                        {
                            y++;
                        }
                        break;
​
                    case 'Z':
                        {
                            z++;
                        }
                        break;
​
                }
            }
​
​
            pocty.Add(a);
            pocty.Add(b);
            pocty.Add(c);
            pocty.Add(d);
            pocty.Add(e);
            pocty.Add(f);
            pocty.Add(g);
            pocty.Add(h);
            pocty.Add(i);
            pocty.Add(j);
            pocty.Add(k);
            pocty.Add(l);
            pocty.Add(m);
            pocty.Add(n);
            pocty.Add(o);
            pocty.Add(p);
            pocty.Add(q);
            pocty.Add(r);
            pocty.Add(s);
            pocty.Add(t);
            pocty.Add(u);
            pocty.Add(v);
            pocty.Add(w);
            pocty.Add(x);
            pocty.Add(y);
            pocty.Add(z);
​
​
            return pocty;
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
