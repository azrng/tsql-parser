using System;
using System.Globalization;
using TSQL_Parser.Tokens;

namespace TSQL.Tokens.Parsers
{
    internal class TSQLTokenFactory
    {
        /// <summary>
        /// 转换为最小元素
        /// </summary>
        /// <param name="tokenValue">字符串值</param>
        /// <param name="startPosition">开始位置</param>
        /// <param name="endPosition">结束位置</param>
        /// <param name="useQuotedIdentifiers"></param>
        /// <returns></returns>
        public TSQLToken Parse(string tokenValue, int startPosition, int endPosition, bool useQuotedIdentifiers)
        {
            if (
                char.IsWhiteSpace(tokenValue[0]))
            {
                return new TSQLWhitespace(startPosition, tokenValue);
            }
            else if (tokenValue[0] == '@')
            {
                if (TSQLVariables.IsVariable(tokenValue))
                {
                    return new TSQLSystemVariable(startPosition, tokenValue);
                }
                else
                {
                    return new TSQLVariable(startPosition, tokenValue);
                }
            }
            else if (tokenValue.StartsWith("--"))
            {
                return new TSQLSingleLineComment(startPosition, tokenValue);
            }
            else if (tokenValue.StartsWith("/*"))
            {
                if (tokenValue.EndsWith("*/"))
                {
                    return new TSQLMultilineComment(startPosition, tokenValue);
                }
                else
                {
                    return new TSQLIncompleteComment(startPosition, tokenValue);
                }
            }
            else if (tokenValue.StartsWith("'") || tokenValue.StartsWith("N'"))
            {
                // make sure there's an even number of quotes so that it's closed properly
                if ((tokenValue.Split('\'').Length - 1) % 2 == 0)
                {
                    return new TSQLStringLiteral(startPosition, tokenValue);
                }
                else
                {
                    return new TSQLIncompleteString(startPosition, tokenValue);
                }
            }
            else if (!useQuotedIdentifiers && tokenValue.StartsWith("\""))
            {
                // 确保引号的数量是偶数，这样才能正确关闭
                // make sure there's an even number of quotes so that it's closed properly
                if ((tokenValue.Split('\"').Length - 1) % 2 == 0)
                {
                    return new TSQLStringLiteral(startPosition, tokenValue);
                }
                else
                {
                    return new TSQLIncompleteString(startPosition, tokenValue);
                }
            }
            else if (tokenValue[0] == '$')
            {
                // $IDENTITY
                if (tokenValue.Length > 1 && char.IsLetter(tokenValue[1]))
                {
                    return new TSQLSystemColumnIdentifier(startPosition, tokenValue);
                }
                // $45.56
                else
                {
                    return new TSQLMoneyLiteral(startPosition, tokenValue);
                }
            }
            else if (CharUnicodeInfo.GetUnicodeCategory(tokenValue[0]) == UnicodeCategory.CurrencySymbol)
            {
                return new TSQLMoneyLiteral(startPosition, tokenValue);
            }
            else if (tokenValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return new TSQLBinaryLiteral(startPosition, tokenValue);
            }
            else if (char.IsDigit(tokenValue[0]) ||
                (
                    tokenValue[0] == '.' &&
                    tokenValue.Length > 1 &&
                    char.IsDigit(tokenValue[1])
                ))
            {
                return new TSQLNumericLiteral(startPosition, tokenValue);
            }
            else if (
                tokenValue[0] == '=' ||
                tokenValue[0] == '~' ||
                tokenValue[0] == '-' ||
                tokenValue[0] == '+' ||
                tokenValue[0] == '*' ||
                tokenValue[0] == '/' ||
                tokenValue[0] == '<' ||
                tokenValue[0] == '>' ||
                tokenValue[0] == '!' ||
                tokenValue[0] == '&' ||
                tokenValue[0] == '|' ||
                tokenValue[0] == '^' ||
                tokenValue[0] == '%' ||
                tokenValue[0] == ':' ||
                tokenValue.Equals("like", StringComparison.CurrentCultureIgnoreCase) ||
                tokenValue.Equals("not", StringComparison.CurrentCultureIgnoreCase) ||
                tokenValue.Equals("is", StringComparison.CurrentCultureIgnoreCase) ||
                tokenValue.Equals("!=", StringComparison.CurrentCultureIgnoreCase) ||
                tokenValue.Equals("in", StringComparison.CurrentCultureIgnoreCase)
                     )
            {
                return new TSQLOperator(startPosition, tokenValue);
            }
            else if (TSQLCharacters.IsCharacter(tokenValue))
            {
                return new TSQLCharacter(startPosition, tokenValue);
            } //先判断是否属于连接符
            else if (tokenValue.Equals("and", StringComparison.InvariantCultureIgnoreCase) ||
                tokenValue.Equals("or", StringComparison.CurrentCultureIgnoreCase))
            {
                return new TSQLConnector(startPosition, tokenValue);
            }
            else if (TSQLKeywords.IsKeyword(tokenValue))
            {
                return new TSQLKeyword(startPosition, tokenValue);
            }
            else if (TSQLIdentifiers.IsIdentifier(tokenValue))
            {
                return new TSQLSystemIdentifier(startPosition, tokenValue);
            }
            else
            {
                // see if there's an odd number of quotes(看看是否有奇数个引号)
                var quotes = (tokenValue.Split('\"').Length - 1) % 2 == 1;
                if ((tokenValue.StartsWith("[") && !tokenValue.EndsWith("]")) || (useQuotedIdentifiers && tokenValue.StartsWith("\"") && quotes))
                {
                    return new TSQLIncompleteIdentifier(startPosition, tokenValue);
                }
                else
                {
                    return new TSQLIdentifier(startPosition, tokenValue);
                }
            }
        }
    }
}