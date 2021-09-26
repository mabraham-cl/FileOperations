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
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "mobydick.txt");
           
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

            textFileOperations.FileName = Path.Combine(Directory.GetCurrentDirectory(), "NoFolder", "mobydick.txt");

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

        private string content = @"**The Project Gutenberg Etext of Moby Dick, by Herman Melville**
            #3 in our series by Herman Melville

            This Project Gutenberg version of Moby Dick is based on a combination
            of the etext from the ERIS project at Virginia Tech and another from
            Project Gutenberg's archives, as compared to a public-domain hard copy.

            Copyright laws are changing all over the world, be sure to check
            the copyright laws for your country before posting these files!!

            Please take a look at the important information in this header.
            We encourage you to keep this file on your own disk, keeping an
            electronic path open for the next readers.  Do not remove this.


            **Welcome To The World of Free Plain Vanilla Electronic Texts**

            **Etexts Readable By Both Humans and By Computers, Since 1971**

            *These Etexts Prepared By Hundreds of Volunteers and Donations*

            Information on contacting Project Gutenberg to get Etexts, and
            further information is included below.  We need your donations.


            Title:  Moby Dick; or The Whale";
            }
}