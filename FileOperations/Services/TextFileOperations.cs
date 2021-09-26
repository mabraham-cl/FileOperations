﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using FileOperations.Services.Utilities;

namespace FileOperations.Services
{
    /// <summary>
    /// Class that handles methods of a text file.
    /// </summary>
    public class TextFileOperations : BaseFileOperations
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileSystem">Abstracted file system object. Used IFileSystem for easier unittesting to mock.</param>
        /// <param name="stringHelper">StringHelper object</param>
        public TextFileOperations(IFileSystem fileSystem, IStringHelper stringHelper)
        {
            _iFileSystem = fileSystem;
            _stringHelper = stringHelper;
        }

        /// <summary>
        /// This will fetch the top n frequent words used in the file.
        /// </summary>
        public override void FetchFrequentWords()
        {
            // If validation fail then return.
            if (!IsValid())
                return;

            _allWords = new Dictionary<string, int>();

            // Buffered reading to save memory used at one point. It is not reading the entire file and storing in memory.
            using var fileStream = _iFileSystem.FileStream.Create(FileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096);
            byte[] buffer = new byte[0x1000];
            int numRead;            

            while ((numRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string text = _stringHelper.GetDefaultEncodedString(buffer, 0, numRead);

                string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    if (_allWords.ContainsKey(word))
                        _allWords[word] = _allWords[word] + 1;
                    else
                        _allWords.Add(word, 1);
                }
            }

            // Retrieve the top n words and its count
            if (_allWords !=null)
                FrequentWords = _allWords.OrderByDescending(x=>x.Value).Take(_count).ToDictionary(pair => pair.Key, pair => pair.Value);

        }

        /// <summary>
        /// Overriden method for validating the size of the file i.e.3000000 bytes
        /// </summary>
        protected override bool Validate()
        {
            // Any additional validation can be done here.

            // Common validations are there in base class
            if (!base.Validate())
                return false;
            

            // Any additional validations for just this class can be added here. e.g: below that checks the file size e.g: limit is 3000000 bytes

            if (new FileInfo(FileName)?.Length > 3000000)
            {
                ErrorMessage = "File is too large to process.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stores the no:ofwords returned.
        /// This can be configuration in configation
        /// </summary>
        private int _count = 20;

        /// <summary>
        /// Store all words and its count
        /// </summary>
        private Dictionary<string, int> _allWords = new Dictionary<string, int>();


        /// <summary>
        /// Stores IFileSystem object
        /// </summary>
        private IFileSystem _iFileSystem;

        /// <summary>
        /// Stores IStringHelper object
        /// </summary>
        private IStringHelper _stringHelper;
    }
}
