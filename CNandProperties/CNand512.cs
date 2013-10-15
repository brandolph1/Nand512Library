using System;

namespace WelchAllyn.CNandProperties
{
    /// <summary>
    /// This class defines the sizes of the various memory structures.
    /// The memory device is typically an ST-Micro NAND512W3A2C.
    /// This device provides 4096 blocks of byte-sized memory
    /// where each block is 16K-bytes plus 512 bytes spare.
    /// </summary>
    public class CNand512
    {
        /// <summary>
        /// Provides the number of blocks per device
        /// </summary>
        public long BlocksPerDevice
        {
            get { return 4096L; }
        }
        /// <summary>
        /// Provides the number of pages per block
        /// </summary>
        public long PagesPerBlock
        {
            get { return 32L; }
        }
        /// <summary>
        /// Provides the number of bytes in the main area of each page
        /// </summary>
        public long BytesPerPage
        {
            get { return 512L; }
        }
        /// <summary>
        /// Provides the number of bytes in the spare area of each page
        /// </summary>
        public long BytesPerSpare
        {
            get { return 16L; }
        }
        /// <summary>
        /// Provides the number of bytes in a block (including spare areas)
        /// </summary>
        public long BytesPerBlock
        {
            get { return PagesPerBlock * (BytesPerPage + BytesPerSpare); }
        }
        /// <summary>
        /// Provides the byte location index (zero-based) of the bad block marker
        /// </summary>
        public long FactoryBadBlockMarkerLoc
        {
            get { return BytesPerPage + 5L; }   // point to the 6th byte in the spare area
        }
    }
}
