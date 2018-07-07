using System;
using System.Collections.Concurrent;

namespace FileWatcher.Utilities
{
    public static class WatchedFiles
    {
        public static ConcurrentDictionary<string, (long LineCount, DateTime LastModified)> Files;
    }
}
