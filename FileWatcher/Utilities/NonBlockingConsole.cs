using Colorful;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;

namespace FileWatcher.Utilities
{
    public static class NonBlockingConsole
    {
        private static readonly BlockingCollection<(string Value, Color Color)> _queue =
            new BlockingCollection<(string Value, Color Color)>();

        static NonBlockingConsole()
        {
            var thread = new Thread(() =>
                    {
                        while (true)
                        {
                            var newLine = _queue.Take();
                            Console.WriteLine(newLine.Value, newLine.Color);
                        }
                    })
                {IsBackground = true};
            thread.Start();
        }

        public static void WriteLine(string value)
        {
            WriteLine(value, Color.White);
        }

        public static void WriteLine(string value, Color color)
        {
            _queue.Add((value, color));
        }
    }
}
