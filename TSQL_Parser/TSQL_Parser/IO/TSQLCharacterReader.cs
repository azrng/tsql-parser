using System.IO;

namespace TSQL.IO
{
    /// <summary>
    /// tsql 字符读取
    /// </summary>
    internal partial class TSQLCharacterReader
    {
        private ICharacterReader _inputStream = null;
        private bool _hasMore = true;
        private bool _hasExtra = false;
        private char _extraChar;

        public TSQLCharacterReader(TextReader inputStream)
        {
            // can't take the risk that the passed in stream is not buffered
            // because of the high call number of Read
            _inputStream = new BufferedTextReader(inputStream);
            Position = -1;
        }

        public bool Read()
        {
            if (_hasMore)
            {
                if (_hasExtra)
                {
                    Current = _extraChar;
                    _hasExtra = false;
                }
                else
                {
                    _hasMore = _inputStream.MoveNext();
                    if (_hasMore)
                    {
                        Current = _inputStream.Current;
                        Position++;
                    }
                    else
                    {
                        Current = char.MinValue;
                    }
                }
            }

            return _hasMore;
        }

        /// <summary>
        /// 读取下一个没有空格的值
        /// </summary>
        /// <returns></returns>
        public bool ReadNextNonWhitespace()
        {
            bool hasNext;

            do
            {
                hasNext = Read();
            } while (hasNext && char.IsWhiteSpace(Current));

            return hasNext;
        }

        /// <summary>
        /// Places the current character back so that it can be read again.
        /// </summary>
        public void Putback()
        {
            _hasExtra = true;
            _extraChar = Current;
            _hasMore = true;
        }

        public char Current
        {
            get;

            private set;
        }

        public int Position
        {
            get;

            private set;
        }

        public bool EOF
        {
            get
            {
                return !_hasMore;
            }
        }
    }
}