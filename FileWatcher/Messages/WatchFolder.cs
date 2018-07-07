using Akka.Actor;

namespace FileWatcher.Messages
{
    internal struct WatchFolder
    {
        public WatchFolder(string fileLocation, string matchingPattern, IActorRef dbCoordinator)
        {
            FileLocation = fileLocation;
            MatchingPattern = matchingPattern;
            DbCoordinator = dbCoordinator;
        }

        public string FileLocation { get; }
        public string MatchingPattern { get; }
        public IActorRef DbCoordinator { get; }
    }
}
