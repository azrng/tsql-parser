using TSQL;
using TSQL.Tokens;

namespace TSQL_Parser.Tokens
{
    /// <summary>
    /// tsql 连接符
    /// </summary>
    public class TSQLConnector : TSQLKeyword
    {
        public TSQLConnector(int beginPosition, string text) : base(beginPosition, text)
        {
            Keyword = TSQLKeywords.Parse(text);
        }

#pragma warning disable 1591

        public override TSQLTokenType Type
        {
            get
            {
                return TSQLTokenType.Connector;
            }
        }

#pragma warning restore 1591

        public TSQLKeywords Keyword
        {
            get;
            private set;
        }

        public override bool IsComplete
        {
            get
            {
                return true;
            }
        }
    }
}
