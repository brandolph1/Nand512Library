// Nand512Library_cppcli.h

#pragma once

using namespace System;
using namespace WelchAllyn::CNandProperties;
using namespace WelchAllyn::NandPageLibrary;

namespace WelchAllyn
{
	namespace Nand512Library
	{
		public ref class CBlock : public CNand512
		{
		private:
			array<CPage^>^ _pages;
		public:
			/// <summary>
			/// Block constructor
			/// </summary>
			CBlock()
			{
				long ppb = static_cast<long>(PagesPerBlock);
				_pages = gcnew array<CPage^>(ppb);

				for (int ii = 0; ii < static_cast<int>(ppb); ++ii)
				{
					_pages[ii] = gcnew CPage();
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <param name="src_data"></param>
			/// <param name="src_spare"></param>
			/// <returns></returns>
			bool FillPage(long index, array<Byte>^ src_data, array<Byte>^ src_spare)
			{
				bool bRv = false;

				if (index < PagesPerBlock)
				{
					if (src_data->Length == BytesPerPage)
					{
						if (src_spare->Length == BytesPerSpare)
						{
							CPage^ p = _pages[index];

							p->Main = src_data;
							p->Spare = src_spare;
							bRv = true;
						}
					}
				}

				return bRv;
			}
			///
			///
			///
			bool FillPage(long index, CPageData^ src)
			{
				return FillPage(index, src->Main, src->Spare);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <param name="dest_page"></param>
			/// <param name="dest_spare"></param>
			/// <returns></returns>
			bool GetPage(long index, array<Byte>^ dest_page, array<Byte>^ dest_spare)
			{
				bool bRv = false;

				if (index < PagesPerBlock)
				{
					if (dest_page->Length == BytesPerPage)
					{
						if (dest_spare->Length == BytesPerSpare)
						{
							CPage^ p = _pages[index];
	                        
							p->Main->CopyTo(dest_page, 0);
							p->Spare->CopyTo(dest_spare, 0);
							bRv = true;
						}
					}
				}

				return bRv;
			}
			/// <summary>
			///
			/// </summary>
			bool GetPage(long index, CPageData^% dest)
			{
				bool bRv = false;

				if (index < PagesPerBlock)
				{
					if (dest != nullptr)
					{
						CPage^ p = _pages[index];
				
						dest->Main = p->Main;
						dest->Spare = p->Spare;
						bRv = true;
					}
				}

				return bRv;
			}
			/// <summary>
			/// Erase this block-full of pages
			/// </summary>
			void Erase(void)
			{
				for each (CPage^ p in _pages)
				{
					p->Erase();
				}
			}
			/// <summary>
			/// Fill is a member of the CBlock class.
			/// <para>Accepts a 'block-sized' array of data-bytes and parses it into the pages of this storage block.</para>
			/// </summary>
			/// <param name="src">A reference to the source array of data-bytes.</param>
			void Fill(array<Byte>^ src)
			{
				Fill(src, 0);
			}
			/// <summary>
			/// Fill is a member of the CBlock class.
			/// <para>Accepts a 'block-sized' array of data-bytes and parses it into the pages of this storage block.</para>
			/// </summary>
			/// <param name="src">A reference to the source array of data-bytes.</param>
			/// <param name="start">The index to the first byte in the array.</param>
			void Fill(array<Byte>^ src, long start)
			{
				Fill(src, start, src->Length);
			}
			/// <summary>
			/// Fill is a member of the CBlock class.
			/// <para>Accepts a 'block-sized' array of data-bytes and parses it into the pages of this storage block.</para>
			/// </summary>
			/// <param name="src">A reference to the source array of data-bytes.</param>
			/// <param name="start">The index to the first byte in the array.</param>
			/// <param name="length">The number of bytes available in the array.</param>
			void Fill(array<Byte>^ src, long start, long length)
			{
				int src_index = static_cast<int>(start);
				int xxLen = 0;

				for each (CPage^ p in _pages)
				{
					if (src_index < length)
					{
						if (src_index < (length - BytesPerPage))
						{
							xxLen = BytesPerPage;
						}
						else
						{
							xxLen = length - src_index;
						}

						Array::Copy(src, src_index, p->Main, 0, xxLen);
						src_index += BytesPerPage;

						if (src_index < length)
						{
							if (src_index < (length - BytesPerSpare))
							{
								xxLen = BytesPerSpare;
							}
							else
							{
								xxLen = length - src_index;
							}

							Array::Copy(src, src_index, p->Spare, 0, xxLen);
							src_index += BytesPerSpare;
						}
					}
				}
			}
		};
		///
		///
		///
		public ref class CDevice : public CNand512
		{
		private:
			array<CBlock^>^ _blocks;
		public:
			CDevice()
			{
				long bpd = BlocksPerDevice;
				_blocks = gcnew array<CBlock^>(bpd);

				for (int bb = 0; bb < bpd; ++bb)
				{
					_blocks[bb] = gcnew CBlock();
				}
			}
			///
			///
			///
			property long NumberOfBlocks
			{
				long get() { return BlocksPerDevice; }
			}
			///
			///
			///
			property long BlockLength
			{
				long get() { return BytesPerBlock; }
			}
			/// <summary>
			/// FillBlock is a member of the CDevice class.
			/// <para>Accepts a 'block-sized' array of data-bytes to store in the specified block.</para>
			/// </summary>
			bool FillBlock(long index, array<Byte>^ src)
			{
				bool bRv = false;

				if (index < NumberOfBlocks)
				{
					if (src->Length == BlockLength)
					{
						CBlock^ b = _blocks[index];

						b->Fill(src);
						bRv = true;
					}
				}

				return bRv;
			}
			/// <summary>
			/// FillBlock is a member of the CDevice class.
			/// <para>Accepts a 'block-sized' array of data-bytes to store in the specified block.</para>
			/// </summary>
			bool FillBlock(long index, array<Byte>^ src, long start)
			{
				bool bRv = false;

				if (index < NumberOfBlocks)
				{
					CBlock^ b = _blocks[index];

					b->Fill(src, start);
					bRv = true;
				}

				return bRv;
			}
			/// <summary>
			/// FillBlock is a member of the CDevice class.
			/// <para>Accepts a 'block-sized' array of data-bytes to store in the specified block.</para>
			/// </summary>
			bool FillBlock(long index, array<Byte>^ src, long start, long length)
			{
				bool bRv = false;

				if (index < NumberOfBlocks)
				{
					CBlock^ b = _blocks[index];

					b->Fill(src, start, length);
					bRv = true;
				}

				return bRv;
			}
			///
			///
			///
			bool GetBlock(long index, array<Byte>^% dest)	// 'dest' is a tracked reference (like a C# 'ref' parameter)
			{
				bool bRv = false;

				if (index < BlocksPerDevice)
				{
					if (dest->Length == BytesPerBlock)
					{
						array<Byte>^ page_copy = gcnew array<Byte>(BytesPerPage);
						array<Byte>^ spare_copy = gcnew array<Byte>(BytesPerSpare);
						CBlock^ b = _blocks[index];
						long byte_index = 0;

						for (long pg = 0; pg < PagesPerBlock; ++pg)
						{
							if (!b->GetPage(pg, page_copy, spare_copy)) throw gcnew ApplicationException("Failed to get page in block");

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
			///
			///
			///
			bool EraseBlock(long index)
			{
				bool bRv = false;

				if (index < NumberOfBlocks)
				{
					CBlock^ b = _blocks[index];

					b->Erase();
					bRv = true;
				}
				
				return bRv;
			}
			///
			///
			///
			bool SetBadBlock(long index)
			{
				bool bRv = false;

				if (index < BlocksPerDevice)
				{
					if (EraseBlock(index))
					{
						array<Byte>^ page_copy = gcnew array<Byte>(BytesPerPage);
						array<Byte>^ spare_copy = gcnew array<Byte>(BytesPerSpare);
						CBlock^ b = (CBlock^)_blocks[index];

						if (!b->GetPage(0, page_copy, spare_copy)) throw gcnew ApplicationException("Failed to get page in block");

						spare_copy[FactoryBadBlockMarkerLoc - BytesPerPage] = 0x00; // Mark this block as bad

						bRv = b->FillPage(0, page_copy, spare_copy);
					}
				}

				return bRv;
			}
		};
	}
}
