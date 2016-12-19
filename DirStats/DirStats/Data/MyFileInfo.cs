using System;

namespace DirStats.Data
{
    public class MyFileInfo
    {
        public string Name { get; set; }   
        public long Length { get; set; }
        public string Extension { get; set; }
        public DateTime Created { get; set; }

        public override string ToString()
        {
            return $"File: {Name}, Length: {Length}";
        }
    }

    public class User
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return $"User is {Name}";
        }
    }
}