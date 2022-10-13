using System;
using System.Text;

namespace TSQL.Tokens
{
    /// <summary>
    /// tsql 标记(最小字符单位)
    /// </summary>
    public abstract class TSQLToken
    {
        protected internal TSQLToken(int beginPosition, string text)
        {
            BeginPosition = beginPosition;
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// 开始位置
        /// </summary>
        public int BeginPosition { get; }

        /// <summary>
        /// 结束位置
        /// </summary>
        public int EndPosition
        {
            get
            {
                return BeginPosition + Length - 1;
            }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public int Length
        {
            get
            {
                return Text.Length;
            }
        }

        /// <summary>
        /// 文本值
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 类型
        /// </summary>
        public abstract TSQLTokenType Type
        {
            get;
        }

        /// <summary>
        /// 是否完整的
        /// </summary>
        public abstract bool IsComplete
        {
            get;
        }

        /// <summary>
        ///		Overriding ToString() for helpful watch window display.
        ///		重写ToString()以显示有用的监视窗口。
        /// </summary>
        public override string ToString()
        {
            return $"[Type: {Type}; Text: \"{ToLiteral(Text)}\"; BeginPosition: {BeginPosition: #,##0}; Length: {Length: #,##0}]";
        }

        // https://stackoverflow.com/a/14087738
        /// <summary>
        ///		For use in the ToString() output above. Collapses token Text value to a single escaped line.
        ///		在上面的ToString()输出中使用。将令牌文本值折叠为单个转义行。
        /// </summary>
        private static string ToLiteral(string input)
        {
            StringBuilder literal = new StringBuilder(input.Length);
            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'': literal.Append(@"\'"); break;
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e)
                        {
                            literal.Append(c);
                            // As UTF16 escaped character
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            return literal.ToString();
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLBinaryLiteral"/>.
        /// </summary>
        public TSQLBinaryLiteral AsBinaryLiteral
        {
            get
            {
                return this as TSQLBinaryLiteral;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLCharacter"/>.
        /// </summary>
        public TSQLCharacter AsCharacter
        {
            get
            {
                return this as TSQLCharacter;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLComment"/>.
        /// </summary>
        public TSQLComment AsComment
        {
            get
            {
                return this as TSQLComment;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLIdentifier"/>.
        /// </summary>
        public TSQLIdentifier AsIdentifier
        {
            get
            {
                return this as TSQLIdentifier;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLSystemIdentifier"/>.
        /// </summary>
        public TSQLSystemIdentifier AsSystemIdentifier
        {
            get
            {
                return this as TSQLSystemIdentifier;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLSystemColumnIdentifier"/>.
        /// </summary>
        public TSQLSystemColumnIdentifier AsSystemColumnIdentifier
        {
            get
            {
                return this as TSQLSystemColumnIdentifier;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLKeyword"/>.
        /// </summary>
        public TSQLKeyword AsKeyword
        {
            get
            {
                return this as TSQLKeyword;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLLiteral"/>.
        /// </summary>
        public TSQLLiteral AsLiteral
        {
            get
            {
                return this as TSQLLiteral;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLMultilineComment"/>.
        /// </summary>
        public TSQLMultilineComment AsMultilineComment
        {
            get
            {
                return this as TSQLMultilineComment;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLNumericLiteral"/>.
        /// </summary>
        public TSQLNumericLiteral AsNumericLiteral
        {
            get
            {
                return this as TSQLNumericLiteral;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLOperator"/>.
        /// </summary>
        public TSQLOperator AsOperator
        {
            get
            {
                return this as TSQLOperator;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLSingleLineComment"/>.
        /// </summary>
        public TSQLSingleLineComment AsSingleLineComment
        {
            get
            {
                return this as TSQLSingleLineComment;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLStringLiteral"/>.
        /// </summary>
        public TSQLStringLiteral AsStringLiteral
        {
            get
            {
                return this as TSQLStringLiteral;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLVariable"/>.
        /// </summary>
        public TSQLVariable AsVariable
        {
            get
            {
                return this as TSQLVariable;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLSystemVariable"/>.
        /// </summary>
        public TSQLSystemVariable AsSystemVariable
        {
            get
            {
                return this as TSQLSystemVariable;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLWhitespace"/>.
        /// </summary>
        public TSQLWhitespace AsWhitespace
        {
            get
            {
                return this as TSQLWhitespace;
            }
        }

        /// <summary>
        ///		Fluent convenience shortcut for casting object
        ///		as <see cref="TSQL.Tokens.TSQLMoneyLiteral"/>
        /// </summary>
        public TSQLMoneyLiteral AsMoneyLiteral
        {
            get
            {
                return this as TSQLMoneyLiteral;
            }
        }
    }
}