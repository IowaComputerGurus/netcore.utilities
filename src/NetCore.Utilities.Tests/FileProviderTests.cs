using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace ICG.NetCore.Utilities.Tests
{
    /// <summary>
    /// Unit tests for <see cref="FileProvider"/>.
    /// </summary>
    public class FileProviderTests : IDisposable
    {
        private readonly IFileProvider _fileProvider;
        private readonly string _testDirectory;

        public FileProviderTests()
        {
            _fileProvider = new FileProvider();
            _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDirectory);
        }

        private string GetTestFilePath(string fileName = null)
        {
            return Path.Combine(_testDirectory, fileName ?? Guid.NewGuid().ToString("N") + ".txt");
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, true);
                }
                catch { /* ignore cleanup errors */ }
            }
        }

        [Fact]
        public void WriteAllText_And_ReadAllText_ShouldRoundTrip()
        {
            var path = GetTestFilePath();
            var content = "Hello, FileProvider!";
            _fileProvider.WriteAllText(path, content);
            var read = _fileProvider.ReadAllText(path);
            Assert.Equal(content, read);
        }

        [Fact]
        public void WriteAllText_WithEncoding_ShouldRoundTrip()
        {
            var path = GetTestFilePath();
            var content = "Encoding test";
            var encoding = Encoding.Unicode;
            _fileProvider.WriteAllText(path, content, encoding);
            var read = _fileProvider.ReadAllText(path, encoding);
            Assert.Equal(content, read);
        }

        [Fact]
        public void WriteAllLines_And_ReadAllLines_ShouldRoundTrip()
        {
            var path = GetTestFilePath();
            var lines = new[] { "Line1", "Line2", "Line3" };
            _fileProvider.WriteAllLines(path, lines);
            var read = _fileProvider.ReadAllLines(path);
            Assert.Equal(lines, read);
        }

        [Fact]
        public void WriteAllLines_WithEncoding_ShouldRoundTrip()
        {
            var path = GetTestFilePath();
            var lines = new[] { "A", "B", "C" };
            var encoding = Encoding.UTF8;
            _fileProvider.WriteAllLines(path, lines, encoding);
            var read = _fileProvider.ReadAllLines(path, encoding);
            Assert.Equal(lines, read);
        }

        [Fact]
        public void AppendAllText_ShouldAppendContent()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "First");
            _fileProvider.AppendAllText(path, "Second");
            var read = _fileProvider.ReadAllText(path);
            Assert.Equal("FirstSecond", read);
        }

        [Fact]
        public void AppendAllText_WithEncoding_ShouldAppendContent()
        {
            var path = GetTestFilePath();
            var encoding = Encoding.UTF8;
            _fileProvider.WriteAllText(path, "A", encoding);
            _fileProvider.AppendAllText(path, "B", encoding);
            var read = _fileProvider.ReadAllText(path, encoding);
            Assert.Equal("AB", read);
        }

        [Fact]
        public void AppendAllLines_ShouldAppendLines()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllLines(path, new[] { "1" });
            _fileProvider.AppendAllLines(path, new[] { "2", "3" });
            var read = _fileProvider.ReadAllLines(path);
            Assert.Equal(new[] { "1", "2", "3" }, read);
        }

        [Fact]
        public void AppendAllLines_WithEncoding_ShouldAppendLines()
        {
            var path = GetTestFilePath();
            var encoding = Encoding.UTF8;
            _fileProvider.WriteAllLines(path, new[] { "A" }, encoding);
            _fileProvider.AppendAllLines(path, new[] { "B", "C" }, encoding);
            var read = _fileProvider.ReadAllLines(path, encoding);
            Assert.Equal(new[] { "A", "B", "C" }, read);
        }

        [Fact]
        public void Exists_ShouldReturnTrueForExistingFile()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "Exists");
            Assert.True(_fileProvider.Exists(path));
        }

        [Fact]
        public void Exists_ShouldReturnFalseForNonExistingFile()
        {
            var path = GetTestFilePath();
            Assert.False(_fileProvider.Exists(path));
        }

        [Fact]
        public void Delete_ShouldRemoveFile()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "DeleteMe");
            _fileProvider.Delete(path);
            Assert.False(_fileProvider.Exists(path));
        }

        [Fact]
        public void Copy_ShouldCopyFile()
        {
            var src = GetTestFilePath("src.txt");
            var dest = GetTestFilePath("dest.txt");
            _fileProvider.WriteAllText(src, "CopyMe");
            _fileProvider.Copy(src, dest);
            Assert.True(_fileProvider.Exists(dest));
            Assert.Equal("CopyMe", _fileProvider.ReadAllText(dest));
        }

        [Fact]
        public void Copy_WithOverwrite_ShouldOverwriteFile()
        {
            var src = GetTestFilePath("src.txt");
            var dest = GetTestFilePath("dest.txt");
            _fileProvider.WriteAllText(src, "Source");
            _fileProvider.WriteAllText(dest, "Destination");
            _fileProvider.Copy(src, dest, true);
            Assert.Equal("Source", _fileProvider.ReadAllText(dest));
        }

        [Fact]
        public void Move_ShouldMoveFile()
        {
            var src = GetTestFilePath("moveSrc.txt");
            var dest = GetTestFilePath("moveDest.txt");
            _fileProvider.WriteAllText(src, "MoveMe");
            _fileProvider.Move(src, dest);
            Assert.False(_fileProvider.Exists(src));
            Assert.True(_fileProvider.Exists(dest));
            Assert.Equal("MoveMe", _fileProvider.ReadAllText(dest));
        }

        [Fact]
        public void ReadAllBytes_And_WriteAllBytes_ShouldRoundTrip()
        {
            var path = GetTestFilePath();
            var bytes = new byte[] { 1, 2, 3, 4, 5 };
            _fileProvider.WriteAllBytes(path, bytes);
            var read = _fileProvider.ReadAllBytes(path);
            Assert.Equal(bytes, read);
        }

        [Fact]
        public void OpenRead_ShouldReturnReadableStream()
        {
            var path = GetTestFilePath();
            var content = "OpenRead";
            _fileProvider.WriteAllText(path, content);
            using var stream = _fileProvider.OpenRead(path);
            using var reader = new StreamReader(stream);
            Assert.Equal(content, reader.ReadToEnd());
        }

        [Fact]
        public void OpenWrite_ShouldReturnWritableStream()
        {
            var path = GetTestFilePath();
            using (var stream = _fileProvider.OpenWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("OpenWrite");
            }
            var read = _fileProvider.ReadAllText(path);
            Assert.Equal("OpenWrite", read);
        }

        [Fact]
        public void OpenText_ShouldReturnStreamReader()
        {
            var path = GetTestFilePath();
            var content = "OpenText";
            _fileProvider.WriteAllText(path, content);
            using var reader = _fileProvider.OpenText(path);
            Assert.Equal(content, reader.ReadToEnd());
        }

        [Fact]
        public void CreateText_ShouldCreateFileAndWrite()
        {
            var path = GetTestFilePath();
            using (var writer = _fileProvider.CreateText(path))
            {
                writer.Write("CreateText");
            }
            var read = _fileProvider.ReadAllText(path);
            Assert.Equal("CreateText", read);
        }

        [Fact]
        public void Replace_ShouldReplaceFileContents()
        {
            var src = GetTestFilePath("src.txt");
            var dest = GetTestFilePath("dest.txt");
            var backup = GetTestFilePath("backup.txt");
            _fileProvider.WriteAllText(src, "Source");
            _fileProvider.WriteAllText(dest, "Destination");
            _fileProvider.Replace(src, dest, backup);
            Assert.Equal("Source", _fileProvider.ReadAllText(dest));
            Assert.True(_fileProvider.Exists(backup));
        }

        [Fact]
        public void Replace_WithIgnoreMetadataErrors_ShouldReplaceFileContents()
        {
            var src = GetTestFilePath("src2.txt");
            var dest = GetTestFilePath("dest2.txt");
            var backup = GetTestFilePath("backup2.txt");
            _fileProvider.WriteAllText(src, "Source2");
            _fileProvider.WriteAllText(dest, "Destination2");
            _fileProvider.Replace(src, dest, backup, true);
            Assert.Equal("Source2", _fileProvider.ReadAllText(dest));
            Assert.True(_fileProvider.Exists(backup));
        }

        [Fact]
        public void GetAndSetAttributes_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "Attributes");
            _fileProvider.SetAttributes(path, FileAttributes.ReadOnly);
            var attrs = _fileProvider.GetAttributes(path);
            Assert.True(attrs.HasFlag(FileAttributes.ReadOnly));
        }

        [Fact]
        public void GetAndSetCreationTime_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "CreationTime");
            var now = DateTime.Now;
            _fileProvider.SetCreationTime(path, now);
            var actual = _fileProvider.GetCreationTime(path);
            Assert.Equal(now.Year, actual.Year);
        }

        [Fact]
        public void GetAndSetCreationTimeUtc_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "CreationTimeUtc");
            var utcNow = DateTime.UtcNow;
            _fileProvider.SetCreationTimeUtc(path, utcNow);
            var actual = _fileProvider.GetCreationTimeUtc(path);
            Assert.Equal(utcNow.Year, actual.Year);
        }

        [Fact]
        public void GetAndSetLastAccessTime_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "LastAccessTime");
            var now = DateTime.Now;
            _fileProvider.SetLastAccessTime(path, now);
            var actual = _fileProvider.GetLastAccessTime(path);
            Assert.Equal(now.Year, actual.Year);
        }

        [Fact]
        public void GetAndSetLastAccessTimeUtc_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "LastAccessTimeUtc");
            var utcNow = DateTime.UtcNow;
            _fileProvider.SetLastAccessTimeUtc(path, utcNow);
            var actual = _fileProvider.GetLastAccessTimeUtc(path);
            Assert.Equal(utcNow.Year, actual.Year);
        }

        [Fact]
        public void GetAndSetLastWriteTime_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "LastWriteTime");
            var now = DateTime.Now;
            _fileProvider.SetLastWriteTime(path, now);
            var actual = _fileProvider.GetLastWriteTime(path);
            Assert.Equal(now.Year, actual.Year);
        }

        [Fact]
        public void GetAndSetLastWriteTimeUtc_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "LastWriteTimeUtc");
            var utcNow = DateTime.UtcNow;
            _fileProvider.SetLastWriteTimeUtc(path, utcNow);
            var actual = _fileProvider.GetLastWriteTimeUtc(path);
            Assert.Equal(utcNow.Year, actual.Year);
        }

        [Fact]
        public void ReadLines_ShouldReturnLines()
        {
            var path = GetTestFilePath();
            var lines = new[] { "A", "B", "C" };
            _fileProvider.WriteAllLines(path, lines);
            var read = _fileProvider.ReadLines(path).ToArray();
            Assert.Equal(lines, read);
        }

        [Fact]
        public void ReadLines_WithEncoding_ShouldReturnLines()
        {
            var path = GetTestFilePath();
            var lines = new[] { "X", "Y", "Z" };
            var encoding = Encoding.UTF8;
            _fileProvider.WriteAllLines(path, lines, encoding);
            var read = _fileProvider.ReadLines(path, encoding).ToArray();
            Assert.Equal(lines, read);
        }

        [Fact]
        public void Create_And_Open_ShouldWork()
        {
            var path = GetTestFilePath();
            using (var fs = _fileProvider.Create(path))
            {
                Assert.True(fs.CanWrite);
            }
            using (var fs = _fileProvider.Open(path, FileMode.Open))
            {
                Assert.True(fs.CanRead);
            }
        }

        [Fact]
        public void Create_WithBufferSize_And_FileOptions_ShouldWork()
        {
            var path = GetTestFilePath();
            using (var fs = _fileProvider.Create(path, 4096, FileOptions.None))
            {
                Assert.True(fs.CanWrite);
            }
        }

        [Fact]
        public void Open_WithAccess_And_Share_ShouldWork()
        {
            var path = GetTestFilePath();
            _fileProvider.WriteAllText(path, "OpenWithAccess");
            using (var fs = _fileProvider.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Assert.True(fs.CanRead);
            }
        }
    }
}