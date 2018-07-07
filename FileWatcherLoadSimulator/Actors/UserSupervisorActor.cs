using Akka.Actor;
using FileWatcherLoadSimulator.Messages;
using System;
using System.Threading;

namespace FileWatcherLoadSimulator.Actors
{
    internal class UserSupervisorActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            if (message is WorkOrder workOrder) startWorking(workOrder);
        }

        private void startWorking(WorkOrder workOrder)
        {
            var numberOfUsers = 0;
            while (numberOfUsers < workOrder.NumberOfUsers)
            {
                startNewUser(workOrder);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private void startNewUser(WorkOrder workOrder)
        {
            var newUser = getUser();
            newUser.Tell(workOrder);
        }

        private IActorRef getUser()
        {
            var userName = Guid.NewGuid().ToString();

            // Check if a child with that name exists
            var user = Context.Child(userName);

            // if it doesn't exist, create it
            if (user.IsNobody()) user = Context.ActorOf<EndUserActor>(userName);

            return user;
        }
    }
}
