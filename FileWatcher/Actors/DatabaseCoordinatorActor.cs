using Akka.Actor;
using Easy.Common.Extensions;
using FileWatcher.Models;
using FileWatcher.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace FileWatcher.Actors
{
    internal class DatabaseCoordinatorActor : ReceiveActor
    {
        public DatabaseCoordinatorActor()
        {
            Receive<WatchedFile>(message => handleWatchedFile(message));
        }

        private void handleWatchedFile(WatchedFile incomingFile)
        {
            var result = WatchedFiles.Files.FirstOrDefault(file => file.Key.Equals(incomingFile.FileName));

            WatchedFiles.Files.AddOrUpdate(
                incomingFile.FileName,
                (incomingFile.LineCount, incomingFile.LastWriteTime),
                (key, oldValue) => (incomingFile.LineCount, incomingFile.LastWriteTime));

            if (!incomingFile.ShouldWriteResultToConsole) return;

            if (result.Key.IsNotNullOrEmpty())
                handleExistingFile(incomingFile, result);
            else
                handleAddedFile(incomingFile);
        }

        private void handleExistingFile(WatchedFile newValue,
            KeyValuePair<string, (long LineCount, DateTime LastModified)> oldValue)
        {
            var lineDifference = Math.Abs(oldValue.Value.LineCount - newValue.LineCount);
            var plusOrMinus = oldValue.Value.LineCount > newValue.LineCount ? "-" : "+";

            NonBlockingConsole.WriteLine($"Modified file: {newValue.FileName}" +
                                         $"\n\tLines in file: {newValue.LineCount} ({plusOrMinus}{lineDifference})");
        }

        private void handleAddedFile(WatchedFile file)
        {
            NonBlockingConsole.WriteLine($"New file: {file.FileName}" +
                                         $"\n\tLines in file: {file.LineCount}", Color.Cyan);
        }
    }
}
