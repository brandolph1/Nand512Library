// CNandProperties_cppcli.h

#pragma once

using namespace System;

namespace WelchAllyn
{
	namespace CNandProperties
	{
		/// <summary>
		/// This class defines the sizes of the various memory structures.
		/// The memory device is typically an ST-Micro NAND512W3A2C.
		/// This device provides 4096 blocks of byte-sized memory
		/// where each block is 16K-bytes plus 512 bytes spare.
		/// </summary>
		public ref class CNand512
		{
		public:
			/// <summary>
	        /// Provides the number of blocks per device
		    /// </summary>
			property long BlocksPerDevice
			{
				long get() { return 4096L; }
			}
			/// <summary>
			/// Provides the number of pages per block
			/// </summary>
			property long PagesPerBlock
			{
				long get() { return 32L; }
			}
			/// <summary>
			/// Provides the number of bytes in the main area of each page
			/// </summary>
			property long BytesPerPage
			{
				long get() { return 512L; }
			}
			/// <summary>
			/// Provides the number of bytes in the spare area of each page
			/// </summary>
			property long BytesPerSpare
			{
				long get() { return 16L; }
			}
			/// <summary>
			/// Provides the number of bytes in a block (including spare areas)
			/// </summary>
			property long BytesPerBlock
			{
				long get() { return PagesPerBlock * (BytesPerPage + BytesPerSpare); }
			}
			/// <summary>
			/// Provides the byte location index (zero-based) of the bad block marker
			///
			property long FactoryBadBlockMarkerLoc
			{
				long get() { return BytesPerPage + 5L; } // point to the 6th byte in the spare area
			}
		};
	}
}
