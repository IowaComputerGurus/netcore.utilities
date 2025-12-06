using System;
using System.IO;
using System.Linq;
using Xunit;

namespace ICG.NetCore.Utilities.Tests
{
    /// <summary>
    /// Unit tests for <see cref="DirectoryProvider"/>.
    /// </summary>
    public class DirectoryProviderTests : IDisposable
    {
        private readonly IDirectoryProvider _directoryProvider;
        private readonly string _testRoot;

        public DirectoryProviderTests()
        {
            _directoryProvider = new DirectoryProvider();
            _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testRoot);
        }

        private string GetTestDir(string name = null) => Path.Combine(_testRoot, name ?? Guid.NewGuid().ToString("N"));
        private string GetTestFile(string dir = null, string name = null) => Path.Combine(dir ?? _testRoot, name ?? Guid.NewGuid().ToString("N") + ".txt");

        public void Dispose()
        {
            if (Directory.Exists(_testRoot))
            {
                try { Directory.Delete(_testRoot, true); } catch { /* ignore cleanup errors */ }
            }
        }

        [Fact]
        public void CreateDirectory_ShouldCreateAndReturnDirectoryInfo()
        {
            var dir = GetTestDir("createdir");
            var info = _directoryProvider.CreateDirectory(dir);
            Assert.True(Directory.Exists(dir));
            Assert.Equal(dir, info.FullName);
        }

        [Fact]
        public void Delete_ShouldRemoveDirectory()
        {
            var dir = GetTestDir("deletedir");
            Directory.CreateDirectory(dir);
            _directoryProvider.Delete(dir);
            Assert.False(Directory.Exists(dir));
        }

        [Fact]
        public void Delete_Recursive_ShouldRemoveDirectoryWithContents()
        {
            var dir = GetTestDir("recursivedelete");
            Directory.CreateDirectory(dir);
            File.WriteAllText(GetTestFile(dir, "file.txt"), "data");
            _directoryProvider.Delete(dir, true);
            Assert.False(Directory.Exists(dir));
        }

        [Fact]
        public void EnumerateDirectories_ShouldReturnSubdirectories()
        {
            var dir1 = GetTestDir("sub1");
            var dir2 = GetTestDir("sub2");
            Directory.CreateDirectory(dir1);
            Directory.CreateDirectory(dir2);
            var dirs = _directoryProvider.EnumerateDirectories(_testRoot).ToList();
            Assert.Contains(dir1, dirs);
            Assert.Contains(dir2, dirs);
        }

        [Fact]
        public void EnumerateFiles_ShouldReturnFiles()
        {
            var file1 = GetTestFile(name: "file1.txt");
            var file2 = GetTestFile(name: "file2.txt");
            File.WriteAllText(file1, "A");
            File.WriteAllText(file2, "B");
            var files = _directoryProvider.EnumerateFiles(_testRoot).ToList();
            Assert.Contains(file1, files);
            Assert.Contains(file2, files);
        }

        [Fact]
        public void EnumerateFiles_WithSearchPattern_ShouldReturnMatchingFiles()
        {
            var file1 = GetTestFile(name: "match1.log");
            var file2 = GetTestFile(name: "match2.log");
            var file3 = GetTestFile(name: "nomatch.txt");
            File.WriteAllText(file1, "A");
            File.WriteAllText(file2, "B");
            File.WriteAllText(file3, "C");
            var files = _directoryProvider.EnumerateFiles(_testRoot, "*.log").ToList();
            Assert.Contains(file1, files);
            Assert.Contains(file2, files);
            Assert.DoesNotContain(file3, files);
        }

        [Fact]
        public void EnumerateFileSystemEntries_ShouldReturnFilesAndDirectories()
        {
            var dir = GetTestDir("fsdir");
            var file = GetTestFile(name: "fsfile.txt");
            Directory.CreateDirectory(dir);
            File.WriteAllText(file, "X");
            var entries = _directoryProvider.EnumerateFileSystemEntries(_testRoot).ToList();
            Assert.Contains(dir, entries);
            Assert.Contains(file, entries);
        }

        [Fact]
        public void Exists_ShouldReturnTrueForExistingDirectory()
        {
            var dir = GetTestDir("existsdir");
            Directory.CreateDirectory(dir);
            Assert.True(_directoryProvider.Exists(dir));
        }

        [Fact]
        public void Exists_ShouldReturnFalseForNonExistingDirectory()
        {
            var dir = GetTestDir("notexistsdir");
            Assert.False(_directoryProvider.Exists(dir));
        }

        [Fact]
        public void GetCreationTime_ShouldReturnValidTime()
        {
            var dir = GetTestDir("creationtimedir");
            Directory.CreateDirectory(dir);
            var time = _directoryProvider.GetCreationTime(dir);
            Assert.True(time <= DateTime.Now);
        }

        [Fact]
        public void GetCurrentDirectory_ShouldReturnCurrentDirectory()
        {
            var expected = Directory.GetCurrentDirectory();
            var actual = _directoryProvider.GetCurrentDirectory();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetDirectories_ShouldReturnSubdirectories()
        {
            var dir1 = GetTestDir("getdir1");
            var dir2 = GetTestDir("getdir2");
            Directory.CreateDirectory(dir1);
            Directory.CreateDirectory(dir2);
            var dirs = _directoryProvider.GetDirectories(_testRoot);
            Assert.Contains(dir1, dirs);
            Assert.Contains(dir2, dirs);
        }

        [Fact]
        public void GetFiles_ShouldReturnFiles()
        {
            var file1 = GetTestFile(name: "getfile1.txt");
            var file2 = GetTestFile(name: "getfile2.txt");
            File.WriteAllText(file1, "A");
            File.WriteAllText(file2, "B");
            var files = _directoryProvider.GetFiles(_testRoot);
            Assert.Contains(file1, files);
            Assert.Contains(file2, files);
        }

        [Fact]
        public void GetDirectoryRoot_ShouldReturnRoot()
        {
            var root = Path.GetPathRoot(_testRoot);
            var actual = _directoryProvider.GetDirectoryRoot(_testRoot);
            Assert.Equal(root, actual);
        }

        [Fact]
        public void Move_ShouldMoveDirectory()
        {
            var src = GetTestDir("movedirsrc");
            var dest = GetTestDir("movedirdest");
            Directory.CreateDirectory(src);
            _directoryProvider.Move(src, dest);
            Assert.False(Directory.Exists(src));
            Assert.True(Directory.Exists(dest));
        }

        [Fact]
        public void SetAndGetLastAccessTime_ShouldWork()
        {
            var dir = GetTestDir("accesstimedir");
            Directory.CreateDirectory(dir);
            var now = DateTime.Now;
            _directoryProvider.SetLastAccessTime(dir, now);
            var actual = _directoryProvider.GetLastAccessTime(dir);
            Assert.Equal(now.Year, actual.Year);
        }

        [Fact]
        public void SetAndGetLastWriteTime_ShouldWork()
        {
            var dir = GetTestDir("writetimedir");
            Directory.CreateDirectory(dir);
            var now = DateTime.Now;
            _directoryProvider.SetLastWriteTime(dir, now);
            var actual = _directoryProvider.GetLastWriteTime(dir);
            Assert.Equal(now.Year, actual.Year);
        }

        [Fact]
        public void SetAndGetCreationTime_ShouldWork()
        {
            var dir = GetTestDir("setcreationtimedir");
            Directory.CreateDirectory(dir);
            var now = DateTime.Now;
            _directoryProvider.SetCreationTime(dir, now);
            var actual = _directoryProvider.GetCreationTime(dir);
            Assert.Equal(now.Year, actual.Year);
        }

        [Fact]
        public void GetLogicalDrives_ShouldReturnDrives()
        {
            var expectedDrives = DriveInfo.GetDrives();
            var drives = _directoryProvider.GetLogicalDrives();
            Assert.NotEmpty(drives);
            Assert.Equal(expectedDrives.Length, drives.Length);
            foreach(var expectedDrive in expectedDrives)
            {
                Assert.Contains(expectedDrive.Name, drives);
            }
        }

        [Fact]
        public void GetParent_ShouldReturnParentDirectoryInfo()
        {
            var dir = GetTestDir("parentdir");
            Directory.CreateDirectory(dir);
            var parent = _directoryProvider.GetParent(dir);
            Assert.Equal(_testRoot, parent.FullName);
        }
    }
}