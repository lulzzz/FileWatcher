using Akka.Actor;

namespace FileWatcher.Messages
{
    internal struct InitializeFileReader
    {
        public InitializeFileReader(IActorRef dbCoordinator)
        {
            DbCoordinator = dbCoordinator;
        }

        public IActorRef DbCoordinator;
    }
}
