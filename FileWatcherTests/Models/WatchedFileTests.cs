using FileWatcher.Models;
using Shouldly;
using System.IO;
using Xunit;

namespace FileWatcherTests.Models
{
    public class WatchedFileTests
    {
        [Fact]
        public void WatchedFileShouldLowerCaseFileNames()
        {
            var watchedFile = new WatchedFile(new FileInfo("TOTALLY UPPERCASE"), 0, false);
            watchedFile.LowerCaseFileName.ShouldBe("totally uppercase");
        }

        [Fact]
        public void WatchedfileShouldMatchOnFileNameAlone()
        {
            var watchedFile1 = new WatchedFile(new FileInfo("test1"), 1, false);
            var watchedFile2 = new WatchedFile(new FileInfo("test1"), 2, false);

            watchedFile1.Equals(watchedFile2).ShouldBeTrue();

            var watchedFile3 = new WatchedFile(new FileInfo("test2"), 2, false);
            watchedFile2.Equals(watchedFile3).ShouldBeFalse();
        }
    }
}
