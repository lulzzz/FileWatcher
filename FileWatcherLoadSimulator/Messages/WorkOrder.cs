namespace FileWatcherLoadSimulator.Messages
{
    internal class WorkOrder
    {
        public WorkOrder(string fileLocation, string filePattern, int numberOfUsers, int sizeOfFiles, int numberOfFiles)
        {
            FileLocation = fileLocation;
            FilePattern = filePattern;
            NumberOfUsers = numberOfUsers;
            SizeOfFiles = sizeOfFiles;
            NumberOfFiles = numberOfFiles;
        }

        public string FileLocation { get; }
        public string FilePattern { get; }
        public int NumberOfUsers { get; }
        public int SizeOfFiles { get; }
        public int NumberOfFiles { get; }
    }
}
