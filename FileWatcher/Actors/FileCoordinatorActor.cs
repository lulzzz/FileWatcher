using Akka.Actor;
using FileWatcher.Messages;
using FileWatcher.Models;
using FileWatcher.Utilities;

namespace FileWatcher.Actors
{
    internal class FileCoordinatorActor : ReceiveActor
    {
        private readonly IActorRef _db;

        public FileCoordinatorActor(IActorRef databaseCoordinator)
        {
            _db = databaseCoordinator;

            Receive<FileToProcess>(file => handleFile(file));
        }

        private void handleFile(FileToProcess file)
        {
            var fileName = file.FileInfo.Name;

            var child = GetChildForFile(fileName);

            child.Tell(file);
        }

        private IActorRef GetChildForFile(string fileName)
        {
            var childName = fileName.ToActorName();
            var child = Context.Child(childName);

            if (!child.IsNobody()) return child;

            child = Context.ActorOf<FileReaderActor>(childName);
            child.Tell(new InitializeFileReader(_db));
            return child;
        }
    }
}
