using System.IO;

namespace FileWatcher.Models
{
    public struct FileToProcess
    {
        public FileToProcess(FileInfo fileInfo, bool shouldWriteResultToConsole)
        {
            FileInfo = fileInfo;
            ShouldWriteResultToConsole = shouldWriteResultToConsole;
        }

        public FileInfo FileInfo { get; }
        public bool ShouldWriteResultToConsole { get; }
    }
}
