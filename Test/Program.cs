//
//#define TEST_CDEVICE
#define TEST_CPAGE
//
using System;
using System.Text;
using WelchAllyn.Nand512Library;
using WelchAllyn.NandPageLibrary;
#if TEST_CPAGE
using WelchAllyn.CNandProperties;
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
#endif
        static void Main(string[] args)
        {
#if TEST_CPAGE
            CNand512 properties = new CNand512();
            CPageData page_data = new CPageData();
            byte[] main_data = new byte[properties.BytesPerPage];
            byte[] spare_data = new byte[properties.BytesPerSpare];

            for (int nn = 0; nn < properties.BytesPerPage; ++nn)
            {
                main_data[nn] = (byte)(nn);
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
