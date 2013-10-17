// NandPageLibrary_cppcli.h

#pragma once

using namespace System;
using namespace WelchAllyn::CNandProperties;

namespace WelchAllyn
{
	namespace NandPageLibrary
	{
		/// <summary>
		/// This class provides the basic memory areas of a page within a NAND flash device.
		/// </summary>
		public ref class CPageData : public CNand512
		{
		private:
			/// <summary>
			/// Arrays for storage
			/// </summary>
			array<Byte>^ _page;
			array<Byte>^ _spare;
		public:
			CPageData()
			{
				_page = gcnew array<Byte>(static_cast<int>(BytesPerPage));
				_spare = gcnew array<Byte>(static_cast<int>(BytesPerSpare));
			}
			///
			///
			///
			void Clear()
			{
				for (int ii=0; ii<_page->Length; ++ii) _page[ii] = 0;
				for (int jj=0; jj<_spare->Length; ++jj) _spare[jj] = 0;
			}
			///
			///
			///
			property array<Byte>^ Main
			{
				array<Byte>^ get() { return _page; }
				void set(array<Byte>^ newval) { newval->CopyTo(_page, 0); }
			}
			///
			///
			///
			property array<Byte>^ Spare
			{
				array<Byte>^ get() { return _spare; }
				void set(array<Byte>^ newval) { newval->CopyTo(_spare, 0); }
			}
		};
		/// <summary>
		/// This class provides the operations on a page of memory.
		/// </summary>
		public ref class CPage : public CNand512
		{
		private:
			/// <summary>
			/// Page storage
			/// </summary>
			CPageData^ _page_data;
			/// <summary>
			///
			/// </summary>
			void _erase(array<Byte>^ data)
			{
				for (int ii=0; ii< data->Length; ++ii)
				{
					data[ii] = Byte::MaxValue;
				}
			}
		public:
			CPage()
			{
				_page_data = gcnew CPageData();
				Erase();
			}
			/// <summary>
			/// Erase the page
			/// </summary>
			void EraseMain()
			{
				_erase(_page_data->Main);
			}
			/// <summary>
			/// Erase the spare area
			/// </summary>
			void EraseSpare()
			{
				_erase(_page_data->Spare);
			}
			/// <summary>
			/// Erase the page and spare area
			/// </summary>
			void Erase()
			{
				EraseMain();
				EraseSpare();
			}
			///
			///
			///
			property array<Byte>^ Main
			{
				array<Byte>^ get() { return _page_data->Main; }

				void set(array<Byte>^ new_src)
				{ 
					if (new_src->Length == BytesPerPage)
					{
						_page_data->Main = new_src;
					}
					else
					{
						throw gcnew ApplicationException("Wrong size for main data area");
					}
				}
			}
			///
			///
			///
			property array<Byte>^ Spare
			{
				array<Byte>^ get() { return _page_data->Spare; }

				void set(array<Byte>^ new_src)
				{
					if (new_src->Length == BytesPerSpare)
					{
						_page_data->Spare = new_src;
					}
					else
					{
						throw gcnew ApplicationException("Wrong size for spare data area");
					}
				}
			}
		};
	}
}
