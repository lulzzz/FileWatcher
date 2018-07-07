using Akka.Actor;
using FileWatcherLoadSimulator.Messages;
using System;
using System.IO;
using System.Linq;
using System.Text;
using XKCDPasswordGen;

namespace FileWatcherLoadSimulator.Actors
{
    internal class EndUserActor : UntypedActor
    {
        private bool _currentlyWorking;
        private string[] _files;
        private Random _random;
        private WorkOrder _workOrder;

        protected override void OnReceive(object message)
        {
            if (message is WorkOrder workOrder)
            {
                _workOrder = workOrder;
                _random = new Random();
                getFilesInFolder();
                getToWork();
            }
        }

        private void getToWork()
        {
            while (true) chooseJob();
        }

        private void chooseJob()
        {
            if (!_currentlyWorking)
            {
                var decision = _random.Next(1,4);
                // Do one of three things...
                switch (decision)
                {
                    case 1: // ... create new file
                        createNewFile();
                        break;

                    case 2: // ... modify existing file
                        modifyExistingFile();
                        break;

                    case 3: // ... delete existing file
                        deleteExistingFile();
                        break;
                }
            }
        }

        private void deleteExistingFile()
        {
            if (_files.Length == 0)
            {
                createNewFile();
            }
            else
            {
                _currentlyWorking = true;
                var filePath = _files.ElementAt(_random.Next(0, _files.Length));
                if (File.Exists(filePath)) File.Delete(filePath);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                getFilesInFolder();

                _currentlyWorking = false;
            }
        }

        private void modifyExistingFile()
        {
            if (_files.Length == 0)
            {
                createNewFile();
            }
            else
            {
                _currentlyWorking = true;
                var filePath = _files.ElementAt(_random.Next(0, _files.Length));

                modifyDataInFile(filePath);
                _currentlyWorking = false;
            }
        }

        private void createNewFile()
        {
            _currentlyWorking = true;
            var numberOfFiles = _files.Length;

            if (numberOfFiles >= _workOrder.NumberOfFiles)
            {
                _currentlyWorking = false;
                chooseJob();
            }

            var fileName = _workOrder.FilePattern.Replace("*", XkcdPasswordGen.Generate(4));
            var filePath = Path.Combine(_workOrder.FileLocation, fileName);

            using (var fs = File.Create(filePath))
            {
                var info = new UTF8Encoding(true).GetBytes(XkcdPasswordGen.Generate(3));
                fs.Write(info, 0, info.Length);
            }

            getFilesInFolder();

            modifyDataInFile(filePath);
            _currentlyWorking = false;
        }

        private void getFilesInFolder()
        {
            _files = Directory.GetFiles(_workOrder.FileLocation);
        }

        private void modifyDataInFile(string filePath)
        {
            var sizeInMb = _random.Next(1, _workOrder.SizeOfFiles);
            const int blockSize = 1024 * 8;
            const int blocksPerMb = 1024 * 1024 / blockSize;
            var data = new byte[blockSize];

            try
            {
                using (var writer = new StreamWriter(filePath, false))
                {
                    for (var i = 0; i < sizeInMb * blocksPerMb; i++)
                    {
                        _random.NextBytes(data);
                        var encoding = _random.Next(1, 2) / 2 == 1 ? Encoding.UTF8 : Encoding.ASCII;
                        var stringToWrite = encoding.GetString(data);
                        writer.WriteLine(stringToWrite);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
