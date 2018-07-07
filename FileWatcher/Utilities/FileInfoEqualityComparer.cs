using System.Collections.Generic;
using System.IO;

namespace FileWatcher.Utilities
{
    public class FileInfoEqualityComparer : IEqualityComparer<FileInfo>
    {
        public static FileInfoEqualityComparer Default = new FileInfoEqualityComparer();

        public bool Equals(FileInfo x, FileInfo y)
        {
            return x.FullName.Equals(y.FullName);
        }

        public int GetHashCode(FileInfo obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}
