using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TSQL.IO;
using TSQL.Tokens;
using TSQL.Tokens.Parsers;

namespace TSQL
{
    /// <summary>
    /// tsql 分词器
    /// </summary>
    public partial class TSQLTokenizer
    {
        private TSQLCharacterReader _charReader;
        private TSQLToken _current;
        /// <summary>
        /// 是否还有更多(是否还没读完)
        /// </summary>
        private bool _hasMore = true;

        /// <summary>
        /// 所有的字符
        /// </summary>
        private readonly StringBuilder _characterHolder = new StringBuilder();

        public TSQLTokenizer(string tsqlText) : this(new StringReader(tsqlText))
        {
        }

        public TSQLTokenizer(TextReader tsqlStream)
        {
            _charReader = new TSQLCharacterReader(tsqlStream);
        }

        /// <summary>
        /// 使用引证标识符
        /// </summary>
        public bool UseQuotedIdentifiers { get; set; }

        /// <summary>
        /// 是否包含空格
        /// </summary>
        public bool IncludeWhitespace { get; set; }

        /// <summary>
        /// 移动到下一个
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            CheckDisposed();

            _current = null;

            if (_hasMore)
            {
                _hasMore = IncludeWhitespace ? _charReader.Read() : _charReader.ReadNextNonWhitespace();

                if (_hasMore)
                {
                    SetCurrent();
                }
            }

            return _hasMore;
        }

        /// <summary>
        /// 设置当前的值
        /// </summary>
        private void SetCurrent()
        {
            _characterHolder.Length = 0;
            // TODO: review Position property viability for situations like network streams
            var startPosition = _charReader.Position;

            // 包含空格并且当前就是空格
            if (IncludeWhitespace && char.IsWhiteSpace(_charReader.Current))
            {
                do
                {
                    _characterHolder.Append(_charReader.Current);
                } while (_charReader.Read() && char.IsWhiteSpace(_charReader.Current));

                if (!_charReader.EOF)
                {
                    _charReader.Putback();
                }
            }
            else
            {
                _characterHolder.Append(_charReader.Current);

                switch (_charReader.Current)
                {
                    // period can signal the start of a numeric literal if followed by a number
                    case '.':
                        {
                            if (_charReader.Read())
                            {
                                if (
                                    _charReader.Current == '0' ||
                                    _charReader.Current == '1' ||
                                    _charReader.Current == '2' ||
                                    _charReader.Current == '3' ||
                                    _charReader.Current == '4' ||
                                    _charReader.Current == '5' ||
                                    _charReader.Current == '6' ||
                                    _charReader.Current == '7' ||
                                    _charReader.Current == '8' ||
                                    _charReader.Current == '9'
                                )
                                {
                                    _characterHolder.Append(_charReader.Current);

                                    goto case '0';
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // all single character sequences with no optional double character sequence
                    case ',':
                    case ';':
                    case '(':
                    case ')':
                    case '~':
                        {
                            break;
                        }
                    // --
                    // -=
                    // -
                    case '-':
                        {
                            if (_charReader.Read())
                            {
                                if (_charReader.Current == '-')
                                {
                                    do
                                    {
                                        _characterHolder.Append(_charReader.Current);
                                    } while (
                                        _charReader.Read() &&
                                        _charReader.Current != '\r' &&
                                        _charReader.Current != '\n');

                                    if (!_charReader.EOF)
                                    {
                                        _charReader.Putback();
                                    }
                                }
                                else if (_charReader.Current == '=')
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // /* */
                    // /=
                    // /
                    case '/':
                        {
                            if (_charReader.Read())
                            {
                                if (_charReader.Current == '*')
                                {
                                    _characterHolder.Append(_charReader.Current);

                                    // supporting nested comments
                                    int currentLevel = 1;

                                    bool lastWasStar = false;
                                    bool lastWasSlash = false;

                                    while (
                                        _charReader.Read() &&
                                        (
                                            currentLevel > 1 ||
                                            // */
                                            !(
                                                lastWasStar &&
                                                _charReader.Current == '/'
                                            )
                                        ))
                                    {
                                        // /*
                                        if (
                                            lastWasSlash &&
                                            _charReader.Current == '*')
                                        {
                                            currentLevel++;
                                            lastWasSlash = false;
                                            lastWasStar = false;
                                        }
                                        // */
                                        else if (
                                            lastWasStar &&
                                            _charReader.Current == '/')
                                        {
                                            currentLevel--;
                                            lastWasSlash = false;
                                            lastWasStar = false;
                                        }
                                        else
                                        {
                                            lastWasSlash = _charReader.Current == '/';
                                            lastWasStar = _charReader.Current == '*';
                                        }

                                        _characterHolder.Append(_charReader.Current);
                                    }

                                    if (!_charReader.EOF)
                                    {
                                        _characterHolder.Append(_charReader.Current);
                                    }
                                }
                                else if (_charReader.Current == '=')
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // <>
                    // <=
                    // <
                    case '<':
                        {
                            if (_charReader.Read())
                            {
                                if (
                                    _charReader.Current == '>' ||
                                    _charReader.Current == '='
                                )
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // !=
                    // !<
                    // !>
                    case '!':
                        {
                            if (_charReader.Read())
                            {
                                if (
                                    _charReader.Current == '=' ||
                                    _charReader.Current == '<' ||
                                    _charReader.Current == '>'
                                )
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // =*
                    // =
                    case '=':
                        {
                            if (_charReader.Read())
                            {
                                if (
                                    _charReader.Current == '*'
                                )
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // &=
                    case '&':
                    // |=
                    case '|':
                    // ^=
                    case '^':
                    // +=
                    case '+':
                    // *=
                    case '*':
                    // %=
                    case '%':
                    // >=
                    case '>':
                        {
                            if (_charReader.Read())
                            {
                                if (_charReader.Current == '=')
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // N''
                    case 'N':
                        {
                            if (_charReader.Read())
                            {
                                if (_charReader.Current == '\'')
                                {
                                    _characterHolder.Append(_charReader.Current);

                                    goto case '\'';
                                }
                                else
                                {
                                    _charReader.Putback();

                                    goto default;
                                }
                            }

                            break;
                        }
                    // ::
                    case ':':
                        {
                            if (_charReader.Read())
                            {
                                if (_charReader.Current == ':')
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                                else
                                {
                                    _charReader.Putback();
                                }
                            }

                            break;
                        }
                    // ''
                    case '\'':
                    // ""
                    case '\"':
                    // [dbo]
                    case '[':
                        {
                            char escapeChar;

                            if (_charReader.Current == '[')
                            {
                                escapeChar = ']';
                            }
                            else
                            {
                                escapeChar = _charReader.Current;
                            }

                            bool stillEscaped;

                            // read until '
                            // UNLESS the ' is doubled up
                            do
                            {
                                while (
                                    _charReader.Read() &&
                                    _charReader.Current != escapeChar)
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }

                                if (!_charReader.EOF)
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }

                                stillEscaped =
                                    !_charReader.EOF &&
                                    _charReader.Read() &&
                                    _charReader.Current == escapeChar;

                                if (stillEscaped)
                                {
                                    _characterHolder.Append(_charReader.Current);
                                }
                            } while (stillEscaped);

                            if (!_charReader.EOF)
                            {
                                _charReader.Putback();
                            }

                            break;
                        }
                    // 0 can start a numeric or binary literal with different parsing logic
                    // 0x69048AEFDD010E
                    // 0x
                    case '0':
                        {
                            if (_charReader.Read())
                            {
                                if (
                                    _charReader.Current == 'x' ||
                                    _charReader.Current == 'X')
                                {
                                    _characterHolder.Append(_charReader.Current);

                                    var foundEnd = false;

                                    while (
                                        !foundEnd &&
                                        _charReader.Read())
                                    {
                                        switch (_charReader.Current)
                                        {
                                            case '0':
                                            case '1':
                                            case '2':
                                            case '3':
                                            case '4':
                                            case '5':
                                            case '6':
                                            case '7':
                                            case '8':
                                            case '9':
                                            case 'a':
                                            case 'b':
                                            case 'c':
                                            case 'd':
                                            case 'e':
                                            case 'f':
                                            case 'A':
                                            case 'B':
                                            case 'C':
                                            case 'D':
                                            case 'E':
                                            case 'F':
                                                {
                                                    _characterHolder.Append(_charReader.Current);

                                                    break;
                                                }
                                            // backslash line continuation
                                            // https://docs.microsoft.com/en-us/sql/t-sql/language-elements/sql-server-utilities-statements-backslash?view=sql-server-2017
                                            case '\\':
                                                {
                                                    _characterHolder.Append(_charReader.Current);

                                                    if (
                                                        !foundEnd &&
                                                        _charReader.Read())
                                                    {
                                                        // should be \r or \n
                                                        _characterHolder.Append(_charReader.Current);

                                                        if (_charReader.Current == '\r')
                                                        {
                                                            if (
                                                                !foundEnd &&
                                                                _charReader.Read())
                                                            {
                                                                // should be \n
                                                                _characterHolder.Append(_charReader.Current);
                                                            }
                                                        }
                                                    }

                                                    break;
                                                }
                                            default:
                                                {
                                                    foundEnd = true;

                                                    break;
                                                }
                                        }
                                    }

                                    if (foundEnd)
                                    {
                                        _charReader.Putback();
                                    }
                                }
                                else
                                {
                                    _charReader.Putback();

                                    goto case '1';
                                }
                            }

                            break;
                        }
                    // numeric literals
                    // 1894.1204
                    // 0.5E-2
                    // 123E-3
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            bool foundEnd = false;
                            bool foundPeriod = false;

                            while (
                                !foundEnd &&
                                _charReader.Read())
                            {
                                switch (_charReader.Current)
                                {
                                    case 'e':
                                    case 'E':
                                        {
                                            _characterHolder.Append(_charReader.Current);

                                            if (_charReader.Read())
                                            {
                                                switch (_charReader.Current)
                                                {
                                                    case '-':
                                                    case '+':
                                                        {
                                                            _characterHolder.Append(_charReader.Current);

                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            _charReader.Putback();

                                                            break;
                                                        }
                                                }
                                            }

                                            while (!foundEnd && _charReader.Read())
                                            {
                                                switch (_charReader.Current)
                                                {
                                                    case '0':
                                                    case '1':
                                                    case '2':
                                                    case '3':
                                                    case '4':
                                                    case '5':
                                                    case '6':
                                                    case '7':
                                                    case '8':
                                                    case '9':
                                                        {
                                                            _characterHolder.Append(_charReader.Current);

                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            foundEnd = true;

                                                            break;
                                                        }
                                                }
                                            }

                                            break;
                                        }
                                    case '.':
                                        {
                                            if (foundPeriod)
                                            {
                                                foundEnd = true;
                                            }
                                            else
                                            {
                                                _characterHolder.Append(_charReader.Current);

                                                foundPeriod = true;
                                            }

                                            break;
                                        }
                                    case '0':
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5':
                                    case '6':
                                    case '7':
                                    case '8':
                                    case '9':
                                        {
                                            _characterHolder.Append(_charReader.Current);

                                            break;
                                        }
                                    // running into a special character signals the end of a previous grouping of normal characters
                                    default:
                                        {
                                            foundEnd = true;

                                            break;
                                        }
                                }
                            }

                            if (foundEnd)
                            {
                                _charReader.Putback();
                            }

                            break;
                        }
                    // $45.56
                    // $IDENTITY
                    case '$':
                        {
                            if (_charReader.Read())
                            {
                                if (
                                    _charReader.Current == '-' ||
                                    _charReader.Current == '+' ||
                                    _charReader.Current == '.' ||
                                    _charReader.Current == '0' ||
                                    _charReader.Current == '1' ||
                                    _charReader.Current == '2' ||
                                    _charReader.Current == '3' ||
                                    _charReader.Current == '4' ||
                                    _charReader.Current == '5' ||
                                    _charReader.Current == '6' ||
                                    _charReader.Current == '7' ||
                                    _charReader.Current == '8' ||
                                    _charReader.Current == '9'
                                    )
                                {
                                    _charReader.Putback();

                                    goto case '£';
                                }
                                else
                                {
                                    _charReader.Putback();

                                    goto default;
                                }
                            }

                            break;
                        }
                    // other Unicode currency symbols recognized by SSMS
                    case '£':
                    case '¢':
                    case '¤':
                    case '¥':
                    case '€':
                    case '₡':
                    case '₱':
                    case '﷼':
                    case '₩':
                    case '₮':
                    case '₨':
                    case '₫':
                    case '฿':
                    case '៛':
                    case '₪':
                    case '₭':
                    case '₦':
                    case '৲':
                    case '৳':
                    case '﹩':
                    case '₠':
                    case '₢':
                    case '₣':
                    case '₤':
                    case '₥':
                    case '₧':
                    case '₯':
                    case '₰':
                    case '＄':
                    case '￠':
                    case '￡':
                    case '￥':
                    case '￦':
                        {
                            bool foundEnd = false;
                            bool foundPeriod = false;

                            if (_charReader.Read())
                            {
                                switch (_charReader.Current)
                                {
                                    case '-':
                                    case '+':
                                        {
                                            _characterHolder.Append(_charReader.Current);

                                            break;
                                        }
                                    default:
                                        {
                                            _charReader.Putback();

                                            break;
                                        }
                                }
                            }

                            while (!foundEnd && _charReader.Read())
                            {
                                switch (_charReader.Current)
                                {
                                    case '.':
                                        {
                                            if (foundPeriod)
                                            {
                                                foundEnd = true;
                                            }
                                            else
                                            {
                                                _characterHolder.Append(_charReader.Current);

                                                foundPeriod = true;
                                            }

                                            break;
                                        }
                                    case '0':
                                    case '1':
                                    case '2':
                                    case '3':
                                    case '4':
                                    case '5':
                                    case '6':
                                    case '7':
                                    case '8':
                                    case '9':
                                        {
                                            _characterHolder.Append(_charReader.Current);

                                            break;
                                        }
                                    default:
                                        {
                                            foundEnd = true;

                                            break;
                                        }
                                }
                            }

                            if (foundEnd)
                            {
                                _charReader.Putback();
                            }

                            break;
                        }
                    default:
                        {
                            //是否读取到结束
                            bool foundEnd = false;

                            while (!foundEnd && _charReader.Read())
                            {
                                switch (_charReader.Current)
                                {
                                    // 遇到一个特殊字符标志着前一组正常字符的结束
                                    // running into a special character signals the end of a previous grouping of normal characters
                                    case ' ':
                                    case '\t':
                                    case '\r':
                                    case '\n':
                                    // pgsql中.不是结束
                                    //case '.':
                                    case ',':
                                    case ';':
                                    case '(':
                                    case ')':
                                    case '+':
                                    case '-':
                                    case '*':
                                    case '=':
                                    case '/':
                                    case '<':
                                    case '>':
                                    case '!':
                                    case '%':
                                    case '^':
                                    case '&':
                                    case '|':
                                    case '~':
                                    case ':':
                                    case '[':
                                    // 反斜杠(延续)行
                                    // Backslash (Line Continuation)
                                    case '\\':
                                    case '£':
                                    case '¢':
                                    case '¤':
                                    case '¥':
                                    case '€':
                                    case '₡':
                                    case '₱':
                                    case '﷼':
                                    case '₩':
                                    case '₮':
                                    case '₨':
                                    case '₫':
                                    case '฿':
                                    case '៛':
                                    case '₪':
                                    case '₭':
                                    case '₦':
                                    case '৲':
                                    case '৳':
                                    case '﹩':
                                    case '₠':
                                    case '₢':
                                    case '₣':
                                    case '₤':
                                    case '₥':
                                    case '₧':
                                    case '₯':
                                    case '₰':
                                    case '＄':
                                    case '￠':
                                    case '￡':
                                    case '￥':
                                    case '￦':
                                        {
                                            foundEnd = true;

                                            break;
                                        }
                                    default:
                                        {
                                            _characterHolder.Append(_charReader.Current);

                                            break;
                                        }
                                }
                            }

                            if (foundEnd)
                            {
                                _charReader.Putback();
                            }

                            break;
                        }
                }
            }

            _current = new TSQLTokenFactory().Parse(
                _characterHolder.ToString(),
                startPosition,
                startPosition + _characterHolder.Length - 1,
                UseQuotedIdentifiers);
        }

        public TSQLToken Current
        {
            get
            {
                CheckDisposed();
                return _current;
            }
        }

        public static List<TSQLToken> ParseTokens(string tsqlText, bool useQuotedIdentifiers = false, bool includeWhitespace = false)
        {
            return new TSQLTokenizer(tsqlText)
            {
                UseQuotedIdentifiers = useQuotedIdentifiers,
                IncludeWhitespace = includeWhitespace
            }.ToList();
        }
    }
}