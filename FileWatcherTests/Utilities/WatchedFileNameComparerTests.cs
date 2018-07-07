using FileWatcher.Models;
using FileWatcher.Utilities;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace FileWatcherTests.Utilities
{
    public class WatchedFileNameComparerTests
    {
        [Fact]
        public void ComparererShouldWorkInContainsStatements()
        {
            var watchedList1 = new List<WatchedFile>
            {
                new WatchedFile(new FileInfo("test1"), 1, false),
                new WatchedFile(new FileInfo("test2"), 2, false)
            };

            var watchedFile = new WatchedFile(new FileInfo("test2"), 1, false);

            watchedList1.Contains(watchedFile, WatchedFileNameComparer.Default).ShouldBeTrue();
        }

        [Fact]
        public void ComparerShouldCompareBasedOnFileName()
        {
            var watchedList1 = new List<WatchedFile> {new WatchedFile(new FileInfo("test1"), 1, false)};
            var watchedList2 = new List<WatchedFile> {new WatchedFile(new FileInfo("test1"), 2, false)};

            watchedList1.SequenceEqual(watchedList2, WatchedFileNameComparer.Default).ShouldBeTrue();

            var watchedList3 = new List<WatchedFile> {new WatchedFile(new FileInfo("test2"), 2, false)};

            watchedList2.SequenceEqual(watchedList3, WatchedFileNameComparer.Default).ShouldBeFalse();
        }

        [Fact]
        public void ComparerShouldWorkInExceptStatements()
        {
            var watchedList1 = new List<WatchedFile>
            {
                new WatchedFile(new FileInfo("test1"), 1, false),
                new WatchedFile(new FileInfo("test2"), 2, false)
            };

            var watchedList2 = new List<WatchedFile>
            {
                new WatchedFile(new FileInfo("test1"), 1, false),
                new WatchedFile(new FileInfo("test2"), 2, false),
                new WatchedFile(new FileInfo("test3"), 3, false)
            };

            var resultList = watchedList2.Except(watchedList1, WatchedFileNameComparer.Default);
            resultList.Count().ShouldBe(1);
            resultList.First().LowerCaseFileName.ShouldBe("test3");
        }
    }
}
