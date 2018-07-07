using Akka.Actor;
using Akka.Routing;
using Easy.Common.Extensions;
using FileWatcher.Messages;
using FileWatcher.Models;
using FileWatcher.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileWatcher.Actors
{
    internal class FolderWatcherActor : UntypedActor
    {
        private IActorRef _fileCoordinator;
        private List<FileInfo> _fileList;

        public FolderWatcherActor()
        {
            _fileList = new List<FileInfo>();
        }

        protected override void OnReceive(object message)
        {
            if (message is WatchFolder watchMessage) StartWatching(watchMessage);
        }

        private void StartWatching(WatchFolder watchMessage)
        {
            _fileCoordinator = Context.ActorOf(Props.Create(() => new FileCoordinatorActor(watchMessage.DbCoordinator))
                    .WithRouter(new RoundRobinPool(5, new DefaultResizer(1, 10))),
                watchMessage.FileLocation.ToActorName() + "filePool");

            var isFirstPass = true;

            var stopwatch = new Stopwatch();
            var lastScanTime = DateTime.Now.ToUniversalTime();
            while (true)
            {
                var scanStarted = DateTime.Now.ToUniversalTime();
                stopwatch.Restart();
                var dirInfo = new DirectoryInfo(watchMessage.FileLocation);
                var currentFiles = dirInfo
                    .EnumerateFiles(watchMessage.MatchingPattern)
                    .AsParallel()
                    .Select(file =>
                    {
                        if (_fileList.Count == 0
                            || fileModifiedSinceLastScan(file.LastWriteTime.ToUniversalTime(), lastScanTime)
                            // Sometimes a file will be modified > 10 seconds ago, and the system will miss it because of lag
                            // This  happens the most when there are lots of big files
                            || fileModifiedSinceLastRecordedWriteTime(file)) 
                            _fileCoordinator.Tell(new FileToProcess(file, !isFirstPass));
                        return file;
                    })
                    .ToList();

                handleDeletedFiles(currentFiles);

                lastScanTime = scanStarted;
                stopwatch.Stop();
                var timeToSleep = TimeSpan.FromSeconds(10) - stopwatch.Elapsed;
                if (timeToSleep > TimeSpan.Zero) Thread.Sleep(timeToSleep);

                isFirstPass = false;
            }
        }

        private bool fileModifiedSinceLastRecordedWriteTime(FileInfo file)
        {
            (long FileCount, DateTime LastModifed) existingValue = (-1, DateTime.MinValue);
            var fileExists = WatchedFiles.Files.TryGetValue(file.Name, out existingValue);
            if (!fileExists) return false;

            return file.LastWriteTime > existingValue.LastModifed;
        }

        private void handleDeletedFiles(List<FileInfo> fileList)
        {
            var deletedFiles = _fileList.Except(fileList, FileInfoEqualityComparer.Default);
            _fileList = fileList;

            deletedFiles.ForEach(file =>
            {
                var fileValue = WatchedFiles.Files.GetValueOrDefault(file.Name);
                WatchedFiles.Files.TryRemove(file.Name, out fileValue);
                NonBlockingConsole.WriteLine($"Deleted file: {file}", Color.Orange);
            });
        }

        private bool fileModifiedSinceLastScan(DateTime lastWriteTime, DateTime lastScanTime)
        {
            return lastWriteTime > lastScanTime;
        }
    }
}
