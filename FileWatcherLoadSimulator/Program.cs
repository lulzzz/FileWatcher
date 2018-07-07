using Akka.Actor;
using FileWatcherLoadSimulator.Actors;
using FileWatcherLoadSimulator.Messages;
using System;
using System.IO;
using Console = Colorful.Console;

namespace FileWatcherLoadSimulator
{
    internal class Program
    {
        private static ActorSystem _actorSystem;
        private static readonly string _pressEsc = "\nPress ESC to exit FileWatcher.";

        private static void Main(string[] args)
        {
            _actorSystem = ActorSystem.Create("FileWatcherActorSystem");

            SafetyCheck(args);

            var fileLocation = args[0];
            var filePattern = args[1];

            Console.WriteLine("How many users do you want to spin up (Default: 10)");
            if (!int.TryParse(Console.ReadLine(), out var numberOfUsers)) numberOfUsers = 10;

            Console.WriteLine("Up to how many Mb do you want the files to be (Default: 1024)?");
            if (!int.TryParse(Console.ReadLine(), out var sizeOfFiles)) sizeOfFiles = 1024;

            Console.WriteLine("Up to how many files do you want the users to create (Default: 10000)?");
            if (!int.TryParse(Console.ReadLine(), out var numberOfFiles)) numberOfFiles = 10000;

            Console.WriteLine(
                $"FileWatcherLoadSimulator is running up-to {numberOfUsers} Users to modify up-to {numberOfFiles}/{sizeOfFiles} Mb files in {fileLocation} using the following pattern: {filePattern}" +
                $"{_pressEsc}");

            var userSupervisor = _actorSystem.ActorOf<UserSupervisorActor>();
            userSupervisor.Tell(new WorkOrder(fileLocation, filePattern, numberOfUsers, sizeOfFiles, numberOfFiles));

            while (Console.ReadKey().Key != ConsoleKey.Escape) Console.ReadKey();

            QuitProgram(false);
        }

        private static void SafetyCheck(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(
                    "FileWatcherLoadSimulator requires two arguments to run:" +
                    "\n\t1) The path to the folder you wish to watch" +
                    "\n\t2) A pattern indicating which file type to watch for." +
                    "\n" +
                    "\nExample: FileWatcherLoadSimulator.exe \"c:\\file folder\" *.txt");
                QuitProgram(true);
            }

            var fileLocation = args[0];

            if (!Directory.Exists(fileLocation))
            {
                Console.WriteLine(
                    $"Folder location provided ({fileLocation}) either does not exist or you don't have permissions to access it.");
                QuitProgram(true);
            }
        }

        private static void QuitProgram(bool withWarning)
        {
            if (withWarning)
            {
                Console.WriteLine(_pressEsc);
                while (Console.ReadKey(true).Key != ConsoleKey.Escape) Console.WriteLine(_pressEsc);
            }

            Console.Write("Exiting FileWatcherLoadSimulator.");
            Environment.Exit(0);
        }
    }
}
