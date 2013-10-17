//
#define TEST_CDEVICE
//#define TEST_CPAGE
//
using System;

#if TEST_CPAGE
using WelchAllyn.CNandProperties;
#endif
using WelchAllyn.NandPageLibrary;
#if TEST_DEVICE
using WelchAllyn.Nand512Library;
#endif

namespace WelchAllyn.Nand512Library.Test
{
    class Program
    {
#if TEST_CDEVICE
        static CDevice nand;
#endif
#if TEST_CPAGE
        static CPage page;
        static CBlock block;
#endif
        static bool Compare(byte[] left, byte[] right)
        {
            bool bRv = false;

            if (left.Length == right.Length)
            {
                bRv = true;

                for (int ii = 0; ii < left.Length; ++ii)
                {
                    if (left[ii] != right[ii])
                    {
                        bRv = false;
                        break;
                    }
                }
            }

            return bRv;
        }
        
        static void Clear(ref byte[] dest)
        {
            for (int ii = 0; ii < dest.Length; ++ii)
            {
                dest[ii] = 0;
            }
        }

        static void Main(string[] args)
        {
#if TEST_CPAGE
            CNand512 properties = new CNand512();
            CPageData page_data = new CPageData();
            byte[] main_data = new byte[properties.BytesPerPage];
            byte[] spare_data = new byte[properties.BytesPerSpare];

            for (int nn = 0; nn < properties.BytesPerPage; ++nn)
            {
                main_data[nn] = (byte)(nn+1);
            }

            for (int nn = 0; nn < properties.BytesPerSpare; ++nn)
            {
                spare_data[nn] = (byte)(nn + 16);
            }

            int pages_per_device = (int)(properties.BlocksPerDevice * properties.PagesPerBlock);

            for (int kk = 0; kk < pages_per_device; ++kk)
            {
                page = new CPage();
                page.Main = main_data;
                page.Spare = spare_data;
            }

            int blocks_per_device = (int)properties.BlocksPerDevice;
            int pages_per_block = (int)properties.PagesPerBlock;
            page_data.Main = main_data;
            page_data.Spare = spare_data;

            for (int kk = 0; kk < blocks_per_device; ++kk)
            {
                block = new CBlock();

                for (int nn = 0; nn < pages_per_block; ++nn)
                {
                    block.FillPage(nn, page_data);
                }

                page_data.Clear();

                for (int mm = 0; mm < pages_per_block; ++mm)
                {
                    block.GetPage(mm, ref page_data);

                    if (!Compare(main_data, page_data.Main))
                    {
                        Console.WriteLine("Comparison failed in block allocation {0}, page #{1}", kk, mm);
                    }

                    if (!Compare(spare_data, page_data.Spare))
                    {
                        Console.WriteLine("Comparison failed in block allocation {0}, spare #{1}", kk, mm);
                    }
                }

                block.Erase();
                block = null;
            }
#endif
#if TEST_CDEVICE
            nand = new CDevice();

            long num_blocks = nand.NumberOfBlocks;
            byte[] block = new byte[nand.BlockLength];
            bool bRv = false;

            for (long jj = 0; jj < nand.NumberOfBlocks; ++jj)
            {
                bRv = nand.GetBlock(0, ref block);

                if (bRv)
                {
                    for (long ii = 0; ii < nand.BlockLength; ++ii)
                    {
                        if (block[ii] != 0xFF)
                        {
                            Console.WriteLine("Unexpected values at byte #{0} of block #{1}", ii, jj);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR: Failed to get block #{0}", jj);
                }

                Array.Clear(block, 0, (int) nand.BlockLength);
            }
#endif
            Console.WriteLine("Done.");
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
