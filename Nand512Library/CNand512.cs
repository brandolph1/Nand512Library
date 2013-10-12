using System;
using System.Text;

namespace WelchAllyn.Nand512Library
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
        protected long BlocksPerDevice
        {
            get { return 4096L; }
        }
        /// <summary>
        /// Provides the number of pages per block
        /// </summary>
        protected long PagesPerBlock
        {
            get { return 32L; }
        }
        /// <summary>
        /// Provides the number of bytes in the main area of each page
        /// </summary>
        protected long BytesPerPage
        {
            get { return 512L; }
        }
        /// <summary>
        /// Provides the number of bytes in the spare area of each page
        /// </summary>
        protected long BytesPerSpare
        {
            get { return 16L; }
        }
        /// <summary>
        /// Provides the number of bytes in a block (including spare areas)
        /// </summary>
        protected long BytesPerBlock
        {
            get { return PagesPerBlock * (BytesPerPage + BytesPerSpare); }
        }
        /// <summary>
        /// Provides the byte location index (zero-based) of the bad block marker
        /// </summary>
        protected long FactoryBadBlockMarkerLoc
        {
            get { return BytesPerPage + 5L; }   // point to the 6th byte in the spare area
        }
    }
    /// <summary>
    /// This class provides the basic memory areas of a page within a NAND flash device.
    /// </summary>
    public class CPageData : CNand512
    {
        /// <summary>
        /// Arrays for storage
        /// </summary>
        private byte[] _page;
        private byte[] _spare;
        /// <summary>
        /// 
        /// </summary>
        public CPageData()
        {
            _page = new byte[BytesPerPage];
            _spare = new byte[BytesPerSpare];
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Main
        {
            get { return _page; }
            set { value.CopyTo(_page, 0); }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Spare
        {
            get { return _spare; }
            set { value.CopyTo(_spare, 0); }
        }
    }
    /// <summary>
    /// This class provides the operations on a page of memory.
    /// </summary>
    public class CPage : CNand512
    {
        /// <summary>
        /// Page storage
        /// </summary>
        private CPageData _page_data;
        /// <summary>
        /// Allocate storage for a page of memory and erase the bytes (set to 0xFF)
        /// </summary>
        public CPage()
        {
            //_page_data = new CPageData();
            Erase();
        }
        /// <summary>
        /// Erase the page
        /// </summary>
        internal void ErasePage()
        {
            byte[] pg = _page_data.Main;

            for (int ii = 0; ii < BytesPerPage; ++ii)
            {
                pg[ii] = 0xFF;
            }
        }
        /// <summary>
        /// Erase the spare area
        /// </summary>
        internal void EraseSpare()
        {
            byte[] sp = _page_data.Spare;

            for (int ii = 0; ii < BytesPerSpare; ii++)
            {
                sp[ii] = 0xFF;
            }
        }
        /// <summary>
        /// Erase the page and spare area
        /// </summary>
        internal void Erase()
        {
            ErasePage();
            EraseSpare();
        }
        /// <summary>
        /// Fill the main area with data
        /// </summary>
        /// <param name="src">
        /// A byte array filled with data to store in the page's main area.
        /// The length of the source array must be the same as the main area size.
        /// </param>
        /// <returns>
        /// True if the copy is successful, false otherwise.
        /// </returns>
        internal bool FillMain(byte[] src)
        {
            bool bRv = false;

            if (src.Length == BytesPerPage)
            {
                src.CopyTo(_page_data.Main, 0);
                bRv = true;
            }

            return bRv;
        }
        /// <summary>
        /// Fill the spare area with data
        /// </summary>
        /// <param name="src">
        /// A byte array filled with data to store in the page's spare area.
        /// The length of the source array must be the same as the spare area size.
        /// </param>
        /// <returns>
        /// True if the copy is successful, false otherwise.
        /// </returns>
        internal bool FillSpare(byte[] src)
        {
            bool bRv = false;

            if (src.Length == BytesPerSpare)
            {
                src.CopyTo(_page_data.Spare, 0);
                bRv = true;
            }

            return bRv;
        }
        
        internal bool Fill(byte[] src)
        {
        	bool bRv = false;
        	
        	if (src.Length == (BytesPerPage + {BytesPerSpare))
        	{
        		// more to do...
        		bRv = true;
        	}
        	
        	return bRv;
        }
        /// <summary>
        /// Return the page data
        /// </summary>
        /// <returns>
        /// A byte array reference to the main area of the page
        /// </returns>
        internal byte[] GetMain()
        {
            byte[] dest = _page_data.Main;

            return dest;
        }
        /// <summary>
        /// Return the spare area data
        /// </summary>
        /// <returns>
        /// A byte array reference to the spare area of the page
        /// </returns>
        internal byte[] GetSpare()
        {
            byte[] dest = _page_data.Spare;

            return dest;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CBlock : CNand512
    {
        private Object[] _pages;
        /// <summary>
        /// 
        /// </summary>
        public CBlock()
        {
            long ppb = PagesPerBlock;
            _pages = new Object[ppb];

            for (int pp = 0; pp < ppb; ++pp)
            {
                _pages[pp] = new CPage();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="page"></param>
        /// <param name="spare"></param>
        /// <returns></returns>
        internal bool FillPage(long index, byte[] src_data, byte[] src_spare)
        {
            bool bRv = false;

            if (index < PagesPerBlock)
            {
                if (src_data.Length == BytesPerPage)
                {
                    if (src_spare.Length == BytesPerSpare)
                    {
                        CPage p = (CPage) _pages[index];

                        p.FillMain(src_data);
                        p.FillSpare(src_spare);
                        bRv = true;
                    }
                }
            }

            return bRv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="dest_page"></param>
        /// <param name="dest_spare"></param>
        /// <returns></returns>
        internal bool GetPage(long index, ref byte[] dest_page, ref byte[] dest_spare)
        {
            bool bRv = false;

            if (index < PagesPerBlock)
            {
                if (dest_page.Length == BytesPerPage)
                {
                    if (dest_spare.Length == BytesPerSpare)
                    {
                        CPage p = (CPage) _pages[index];
                        byte[] page_data = p.GetMain();
                        byte[] spare_data = p.GetSpare();

                        page_data.CopyTo(dest_page, 0);
                        spare_data.CopyTo(dest_spare, 0);
                    }
                }
            }

            return bRv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool EraseBlock()
        {
            for (int ii=0; ii<PagesPerBlock; ++ii)
            {
                CPage p = (CPage) _pages[ii];
                p.Erase();
            }

            return true;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CDevice : CNand512
    {
        private Object[] _blocks;
        /// <summary>
        /// 
        /// </summary>
        public CDevice()
        {
            long bpd = BlocksPerDevice;
            _blocks = new Object[bpd];

            for (int bb = 0; bb < bpd; ++bb)
            {
                _blocks[bb] = new CBlock();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public long NumberOfBlocks
        {
            get { return BlocksPerDevice; }
        }
        /// <summary>
        /// 
        /// </summary>
        public long BlockLength
        {
            get { return BytesPerBlock; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">
        /// A zero-based value indicating the block to fill
        /// </param>
        /// <param name="src">
        /// A byte array of length to hold all pages and spare areas</param>
        /// <returns></returns>
        public bool FillBlock(long index, byte[] src)
        {
            bool bRv = false;

            if (index < BlocksPerDevice)
            {
                if (src.Length == BytesPerBlock)
                {
                    byte[] page_copy = new byte[BytesPerPage];
                    byte[] spare_copy = new byte[BytesPerSpare];
                    CBlock b = (CBlock) _blocks[index];
                    long byte_index = 0;

                    for (long pg = 0; pg < PagesPerBlock; ++pg)
                    {
                        for (int ii = 0; ii < BytesPerPage; ++ii)
                        {
                            page_copy[ii] = src[byte_index];
                            ++byte_index;
                        }

                        for (int ii = 0; ii < BytesPerSpare; ++ii)
                        {
                            spare_copy[ii] = src[byte_index];
                            ++byte_index;
                        }

                        b.FillPage(pg, page_copy, spare_copy);
                    }

                    bRv = true;
                }
            }

            return bRv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool GetBlock(long index, ref byte[] dest)
        {
            bool bRv = false;

            if (index < BlocksPerDevice)
            {
                if (dest.Length == BytesPerBlock)
                {
                    byte[] page_copy = new byte[BytesPerPage];
                    byte[] spare_copy = new byte[BytesPerSpare];
                    CBlock b = (CBlock) _blocks[index];
                    long byte_index = 0;

                    for (long pg = 0; pg < PagesPerBlock; ++pg)
                    {
                        if (!b.GetPage(pg, ref page_copy, ref spare_copy)) throw new ApplicationException("Failed to get page in block");

                        for (int ii = 0; ii < BytesPerPage; ++ii)
                        {
                            dest[byte_index] = page_copy[ii];
                            ++byte_index;
                        }

                        for (int ii = 0; ii < BytesPerSpare; ++ii)
                        {
                            dest[byte_index] = spare_copy[ii];
                            ++byte_index;
                        }
                    }

                    bRv = true;
                }
            }

            return bRv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool EraseBlock(long index)
        {
            bool bRv = false;

            if (index < NumberOfBlocks)
            {
                CBlock b = (CBlock) _blocks[index];

                bRv = b.EraseBlock();
            }

            return bRv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool SetBadBlock(long index)
        {
            bool bRv = false;

            if (index < BlocksPerDevice)
            {
                if (EraseBlock(index))
                {
                    byte[] page_copy = new byte[BytesPerPage];
                    byte[] spare_copy = new byte[BytesPerSpare];
                    CBlock b = (CBlock)_blocks[index];

                    if (!b.GetPage(0, ref page_copy, ref spare_copy)) throw new ApplicationException("Failed to get page in block");

                    spare_copy[FactoryBadBlockMarkerLoc - BytesPerPage] = 0x00; // Mark this block as bad

                    bRv = b.FillPage(0, page_copy, spare_copy);
                }
            }

            return bRv;
        }
    }
}
