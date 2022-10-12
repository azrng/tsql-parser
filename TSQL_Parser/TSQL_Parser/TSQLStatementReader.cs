using System.Collections.Generic;
using System.IO;
using System.Linq;

using TSQL.Statements;
using TSQL.Statements.Parsers;
using TSQL.Tokens;

namespace TSQL
{
    public partial class TSQLStatementReader
    {
        private TSQLTokenizer _tokenizer;
        private bool _hasMore = true;
        private TSQLStatement _current;

        public TSQLStatementReader(string tsqlText) : this(new StringReader(tsqlText))
        {
        }

        public TSQLStatementReader(TextReader tsqlStream)
        {
            _tokenizer = new TSQLTokenizer(tsqlStream);
            // move to first token
            _tokenizer.MoveNext();
        }

        public bool UseQuotedIdentifiers
        {
            get
            {
                return _tokenizer.UseQuotedIdentifiers;
            }

            set
            {
                _tokenizer.UseQuotedIdentifiers = value;
            }
        }

        /// <summary>
        /// 是否包含空格
        /// </summary>
        public bool IncludeWhitespace
        {
            get
            {
                return _tokenizer.IncludeWhitespace;
            }

            set
            {
                _tokenizer.IncludeWhitespace = value;
            }
        }

        public bool MoveNext()
        {
            CheckDisposed();

            if (_hasMore)
            {
                // eat up any tokens inbetween statements until we get to something that might start a new statement
                // which should be a keyword if the batch is valid

                // if the last statement parser did not swallow the final semicolon, or there were multiple semicolons, we will swallow it also
                while (
                    _tokenizer.Current != null &&
                    (
                        _tokenizer.Current.IsWhitespace() ||
                        _tokenizer.Current.IsComment() ||
                        (
                            _tokenizer.Current.Type == TSQLTokenType.Character &&
                            _tokenizer.Current.AsCharacter.Character == TSQLCharacters.Semicolon
                        )
                    ) &&
                    _tokenizer.MoveNext()) ;

                if (_tokenizer.Current == null)
                {
                    _hasMore = false;

                    return _hasMore;
                }

                _current = new TSQLStatementParserFactory().Create(_tokenizer).Parse();
            }

            return _hasMore;
        }

        public TSQLStatement Current
        {
            get
            {
                CheckDisposed();

                return _current;
            }
        }

        /// <summary>
        /// 转换报告
        /// </summary>
        /// <param name="tsqlText"></param>
        /// <param name="useQuotedIdentifiers"></param>
        /// <param name="includeWhitespace">是否包含空格</param>
        /// <returns></returns>
        public static List<TSQLStatement> ParseStatements(
            string tsqlText,
            bool useQuotedIdentifiers = false,
            bool includeWhitespace = false)
        {
            return new TSQLStatementReader(tsqlText)
            {
                IncludeWhitespace = includeWhitespace,
                UseQuotedIdentifiers = useQuotedIdentifiers
            }.ToList();
        }
    }
}