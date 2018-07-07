using System;
using System.IO;

namespace FileWatcher.Models
{
    public class WatchedFile : IEquatable<WatchedFile>
    {
        public WatchedFile(FileInfo fileInfo, long lineCount, bool shouldWriteResultToConsole)
        {
            FileName = fileInfo.Name;
            LineCount = lineCount;
            LastWriteTime = fileInfo.LastWriteTime;
            ShouldWriteResultToConsole = shouldWriteResultToConsole;
        }

        public string LowerCaseFileName
        {
            get
            {
                var lowercase = FileName.ToLower();
                return lowercase;
            }
        }

        public string FileName { get; set; }

        public long LineCount { get; set; }

        public DateTime LastWriteTime { get; set; }

        public bool ShouldWriteResultToConsole { get; set; }

        public bool Equals(WatchedFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(LowerCaseFileName, other.LowerCaseFileName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((WatchedFile) obj);
        }

        public override int GetHashCode()
        {
            return FileName.GetHashCode();
        }
    }
}
