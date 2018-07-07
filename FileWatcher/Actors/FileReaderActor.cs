using Akka.Actor;
using Easy.Common.Extensions;
using FileWatcher.Models;
using System;
using System.IO;
using FileWatcher.Messages;

namespace FileWatcher.Actors
{
    internal class FileReaderActor : UntypedActor
    {
        private IActorRef _db;

        public FileReaderActor()
        {
            var timeout = TimeSpan.FromSeconds(30);
            SetReceiveTimeout(timeout);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case InitializeFileReader initFileReader:
                    _db = initFileReader.DbCoordinator;
                    break;

                case FileToProcess fileToProcess:
                    ReadFile(fileToProcess);
                    break;

                case ReceiveTimeout _:
                    Context.Stop(Context.Self);
                    break;
            }
        }

        private void ReadFile(FileToProcess fileToProcess)
        {
            var fileName = fileToProcess.FileInfo.FullName;
            long lineCount;

            if (File.Exists(fileName)) // Sometimes files get deleted before they're processed
            {
                using (var inStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    lineCount = inStream.CountLines();
                }

                _db.Tell(new WatchedFile(fileToProcess.FileInfo, lineCount, fileToProcess.ShouldWriteResultToConsole),
                    Self);
            }

            Self.Tell(PoisonPill.Instance);
        }
    }
}
