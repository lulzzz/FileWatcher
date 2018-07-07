using Akka.Actor;
using Akka.Routing;
using FileWatcher.Actors;
using FileWatcher.Messages;
using FileWatcher.Utilities;
using System;
using System.Collections.Concurrent;
using System.IO;
using Console = Colorful.Console;

namespace FileWatcher
{
    internal class Program
    {
        private static ActorSystem _actorSystem;
        private static readonly string _pressEsc = "\nPress ESC to exit FileWatcher.";

        private static void Main(string[] args)
        {
            SafetyCheck(args);

            var fileLocation = args[0];
            var filePattern = args[1];

            WatchedFiles.Files = new ConcurrentDictionary<string, (long LineCount, DateTime LastModified)>();

            _actorSystem = ActorSystem.Create("FileWatcherActorSystem");
            var dbCoordinatorPool =
                _actorSystem.ActorOf(
                    Props.Create(() => new DatabaseCoordinatorActor())
                        .WithRouter(new RoundRobinPool(5, new DefaultResizer(1, 10))),
                    fileLocation.ToActorName() + "dbPool");

            NonBlockingConsole.WriteLine(
                $"FileWatcher is watching {fileLocation} for changes, using the following pattern: {filePattern}" +
                $"{_pressEsc}");
            
            var fileWatcherActor = _actorSystem.ActorOf<FolderWatcherActor>();
            var watchFiles = new WatchFolder(fileLocation, filePattern, dbCoordinatorPool);
            fileWatcherActor.Tell(watchFiles);

            while (Console.ReadKey().Key != ConsoleKey.Escape) Console.ReadKey();

            QuitProgram(false);
        }

        private static void SafetyCheck(string[] args)
        {
            if (args.Length != 2)
            {
                NonBlockingConsole.WriteLine(
                    "FileWatcher requires two arguments to run:" +
                    "\n\t1) The path to the folder you wish to watch" +
                    "\n\t2) A pattern indicating which file type to watch for." +
                    "\n" +
                    "\nExample: FileWatcher.exe \"c:\\file folder\" *.txt");
                QuitProgram(true);
            }

            var fileLocation = args[0];

            if (!Directory.Exists(fileLocation))
            {
                NonBlockingConsole.WriteLine(
                    $"Folder location provided ({fileLocation}) either does not exist or you don't have permissions to access it.");
                QuitProgram(true);
            }
        }

        private static void QuitProgram(bool withWarning)
        {
            if (withWarning)
            {
                NonBlockingConsole.WriteLine(_pressEsc);
                while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                    NonBlockingConsole.WriteLine(_pressEsc);
            }

            NonBlockingConsole.WriteLine("Exiting FileWatcher.");
            Environment.Exit(0);
        }
    }
}
