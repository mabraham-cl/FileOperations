using NUnit.Framework;
using Moq;
using FileOperations.Services.Utilities;
using System.IO;
using FileOperations.Services;
using System;
using Microsoft.Win32.SafeHandles;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Abstractions;

namespace FileOperations_UnitTest
{   
    public class TextFileOperations_UnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TextFileOperations_UnitTest_FetchFrequentWords()
        {          
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "content.txt");
           
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();
            Mock<IFileStreamFactory> fileStream = new Mock<IFileStreamFactory>();
            Mock<Stream> stream = new Mock<Stream>();

            byte[] buffer = new byte[0x1000];

            stream.Setup(x => x.Read(buffer, 0, buffer.Length)).Returns(4096).Callback(() => {
                stream.Setup(x => x.Read(buffer, 0, buffer.Length)).Returns(0);
            });

            fileStream.Setup(x=>x.Create(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096)).Returns(stream.Object);

            fileSystemMock.Setup(x => x.FileStream).Returns(fileStream.Object);

            Mock<IStringHelper> mockStringHelper = new Mock<IStringHelper>();

            mockStringHelper.Setup(x=>x.GetDefaultEncodedString(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(content);

            TextFileOperations textFileOperations = new TextFileOperations(fileSystemMock.Object, mockStringHelper.Object);

            textFileOperations.FileName = filePath;

            textFileOperations.FetchFrequentWords();

            Assert.AreEqual(null, textFileOperations.ErrorMessage);

            Assert.That(textFileOperations.FrequentWords.Count > 0);
        }

        [Test]
        public void TextFileOperations_UnitTest_FetchFrequentWords_ValidateFile_Failed()
        {
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();

            Mock<IStringHelper> mockStringHelper = new Mock<IStringHelper>();

            TextFileOperations textFileOperations = new TextFileOperations(fileSystemMock.Object, mockStringHelper.Object);

            textFileOperations.FileName = null;
           
            textFileOperations.FetchFrequentWords();
           
            Assert.AreEqual("Please upload file.", textFileOperations.ErrorMessage);
        }

        [Test]
        public void TextFileOperations_UnitTest_FetchFrequentWords_ValidateFile_Failed_FileDoesNotExists()
        {
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();

            Mock<IStringHelper> mockStringHelper = new Mock<IStringHelper>();

            TextFileOperations textFileOperations = new TextFileOperations(fileSystemMock.Object, mockStringHelper.Object);

            textFileOperations.FileName = Path.Combine(Directory.GetCurrentDirectory(), "NoFolder", "content.txt");

            textFileOperations.FetchFrequentWords();

            Assert.AreEqual("File does not exists.", textFileOperations.ErrorMessage);
        }

        [Test]
        public void TextFileOperations_UnitTest_FetchFrequentWords_ValidateFile_Failed_Size()
        {
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();

            Mock<IStringHelper> mockStringHelper = new Mock<IStringHelper>();

            TextFileOperations textFileOperations = new TextFileOperations(fileSystemMock.Object, mockStringHelper.Object);

            textFileOperations.FileName = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "content_large.txt");

            textFileOperations.FetchFrequentWords();

            Assert.AreEqual("File is too large to process.", textFileOperations.ErrorMessage);
        }

        private string content = @"0123456789ABCDEF
/* ********************************************** */
	Table with TABs (09)
    Table with TABs (09)
	1       2       3
	3.14	6.28	9.42";
            }
}