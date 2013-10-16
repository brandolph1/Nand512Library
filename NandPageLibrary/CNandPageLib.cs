using System;
using System.Text;
using WelchAllyn.CNandProperties;

namespace WelchAllyn.NandPageLibrary
{
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
        public byte[] Main
        {
            get { return _page; }
            set { value.CopyTo(_page, 0); }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Spare
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
            _page_data = new CPageData();
            Erase();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void _erase(byte[] data)
        {
            for (int ii = 0; ii < data.Length; ++ii)
            {
                data[ii] = byte.MaxValue;
            }
        }
        /// <summary>
        /// Erase the page
        /// </summary>
        public void EraseMain()
        {
            _erase(_page_data.Main);
        }
        /// <summary>
        /// Erase the spare area
        /// </summary>
        public void EraseSpare()
        {
            _erase(_page_data.Spare);
        }
        /// <summary>
        /// Erase the main and spare areas
        /// </summary>
        public void Erase()
        {
            EraseMain();
            EraseSpare();
        }
        /// <summary>
        /// Property Main
        /// </summary>
        public byte[] Main
        {
            get { return _page_data.Main; }
            set
            {
                if (value.Length == BytesPerPage)
                {
                    _page_data.Main = value;
                }
                else
                {
                    throw new ApplicationException("Wrong size for main data area.");
                }
            }
        }
        /// <summary>
        /// Property Spare
        /// </summary>
        public byte[] Spare
        {
            get { return _page_data.Spare; }
            set
            {
                if (value.Length == BytesPerSpare)
                {
                    _page_data.Spare = value;
                }
                else
                {
                    throw new ApplicationException("Wrong size for spare data area.");
                }
            }
        }
    }
}
