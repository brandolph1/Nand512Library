using System;
using System.Text;
using WelchAllyn.CNandProperties;
using WelchAllyn.NandPageLibrary;

namespace WelchAllyn.Nand512Library
{
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

                        p.Main = src_data;
                        p.Spare = src_spare;
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
                        
                        p.Main.CopyTo(dest_page, 0);
                        p.Spare.CopyTo(dest_spare, 0);
                        bRv = true;
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
