using FileWatcher.Models;
using System.Collections.Generic;

namespace FileWatcher.Utilities
{
    public class WatchedFileNameComparer : IEqualityComparer<WatchedFile>
    {
        public static readonly WatchedFileNameComparer Default = new WatchedFileNameComparer();

        public bool Equals(WatchedFile x, WatchedFile y)
        {
            return x.LowerCaseFileName == y.LowerCaseFileName;
        }

        public int GetHashCode(WatchedFile obj)
        {
            return obj.LowerCaseFileName.GetHashCode();
        }
    }
}
