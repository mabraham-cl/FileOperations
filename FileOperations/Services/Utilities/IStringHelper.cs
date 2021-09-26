using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Services.Utilities
{
    /// <summary>
    /// public interface to string helper
    /// </summary>
    public interface IStringHelper
    {
        /// <summary>
        /// Gets the default encoded string.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <param name="index">Index of the first byte</param>
        /// <param name="count">count</param>
        /// <returns>string</returns>
        string GetDefaultEncodedString(byte[] bytes, int index, int count);

        /// <summary>
        /// Gets the unicode encoded string.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <param name="index">Index of the first byte</param>
        /// <param name="count">count</param>
        /// <returns>string</returns>
        string GetUniCodeEncodedString(byte[] bytes, int index, int count);
    }
}
