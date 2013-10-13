using System;
using System.Text;
using WelchAllyn.Nand512Library;

namespace WelchAllyn.Nand512Library.Test
{
    class Program
    {
        static CDevice nand;

        static void Main(string[] args)
        {
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

            Console.WriteLine("Done.");
        }
    }
}
