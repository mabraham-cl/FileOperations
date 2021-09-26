using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using FileOperations.Services;
using FileOperations.Services.Utilities;
using Moq;
using NUnit.Framework;

namespace FileOperations_UnitTest
{
    class BinaryFileOperations_UnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void BinaryFileOperations_UnitTest_FetchFrequentWords()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "content.dat");

            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();
            Mock<IFileStreamFactory> fileStream = new Mock<IFileStreamFactory>();
            Mock<Stream> stream = new Mock<Stream>();

            byte[] buffer = new byte[0x1000];

            stream.Setup(x => x.Read(buffer, 0, buffer.Length)).Returns(4096).Callback(() => {
                stream.Setup(x => x.Read(buffer, 0, buffer.Length)).Returns(0);
            });

            fileStream.Setup(x => x.Create(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096)).Returns(stream.Object);

            fileSystemMock.Setup(x => x.FileStream).Returns(fileStream.Object);

            Mock<IStringHelper> mockStringHelper = new Mock<IStringHelper>();

            mockStringHelper.Setup(x => x.GetUniCodeEncodedString(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns(content);

            BinaryFileOperations binFileOperations = new BinaryFileOperations(fileSystemMock.Object, mockStringHelper.Object);

            binFileOperations.FileName = filePath;

            binFileOperations.FetchFrequentWords();

            Assert.AreEqual(null, binFileOperations.ErrorMessage);

            Assert.That(binFileOperations.FrequentWords.Count > 0);
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
        public void BinaryFileOperations_UnitTest_FetchFrequentWords_ValidateFile_Failed_FileDoesNotExists()
        {
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();

            Mock<IStringHelper> mockStringHelper = new Mock<IStringHelper>();

            BinaryFileOperations binFileOperations = new BinaryFileOperations(fileSystemMock.Object, mockStringHelper.Object);

            binFileOperations.FileName = Path.Combine(Directory.GetCurrentDirectory(), "NoFolder", "content.dat");

            binFileOperations.FetchFrequentWords();

            Assert.AreEqual("File does not exists.", binFileOperations.ErrorMessage);
        }

        [Test]
        public void BinaryFileOperations_UnitTest_FetchFrequentWords_ValidateFile_Failed_File_Size()
        {
            Mock<IFileSystem> fileSystemMock = new Mock<IFileSystem>();

            Mock<IStringHelper> mockStringHelper = new Mock<IStringHelper>();

            BinaryFileOperations binFileOperations = new BinaryFileOperations(fileSystemMock.Object, mockStringHelper.Object);

            binFileOperations.FileName = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "content_large.dat");

            binFileOperations.FetchFrequentWords();

            Assert.AreEqual("File is too large to process.", binFileOperations.ErrorMessage);
        }


        private string content = @"0000000 30 31 32 33 34 35 36 37 38 39 41 42 43 44 45 46
0000010 0a 2f 2a 20 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a
0000020 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a 2a
*
0000040 2a 2a 20 2a 2f 0a 09 54 61 62 6c 65 20 77 69 74
0000050 68 20 54 41 42 73 20 28 30 39 29 0a 09 31 09 09
0000060 32 09 09 33 0a 09 33 2e 31 34 09 36 2e 32 38 09
0000070 39 2e 34 32 0a                                 
0000075";
    }
}
