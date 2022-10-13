using System.Linq;

namespace TSQL.Tokens
{
    /// <summary>
    /// 标记类型
    /// </summary>
    public enum TSQLTokenType
    {
        /// <summary>
        ///		Whitespace characters, （空格字符）
        ///		i.e. line feeds or tabs. （即换行或制表符）
        /// </summary>
        Whitespace,

        /// <summary>
        ///		Special character in T-SQL,（T-SQL中的特殊字符）
        ///		e.g. . or ,.
        /// </summary>
        Character,

        /// <summary>
        ///		Object name, alias, or other reference,（对象名称、别名或其他引用）
        ///		e.g. dbo.
        /// </summary>
        Identifier,

        /// <summary>
        ///		Recognized T-SQL built-in reserved system identifier(可识别T-SQL内置保留系统标识符，),
        ///		e.g. OPENROWSET
        /// </summary>
        SystemIdentifier,

        /// <summary>
        ///		Recognized T-SQL keyword(公认的t-sql关键字,),
        ///		e.g. SELECT.
        /// </summary>
        Keyword,

        /// <summary>
        /// 连接符单词 and or
        /// </summary>
        Connector,

        /// <summary>
        ///		Comment starting with -- and continuing until the end of the line(注释从——开始，一直到行末),
        ///		e.g. -- this code creates a new lookup table.
        /// </summary>
        SingleLineComment,

        /// <summary>
        ///		Comment spanning multiple lines starting with /* and ending with */(注释跨越多行，以/*开始，以*/结束)
        ///		e.g. /* here be dragons */.
        /// </summary>
        MultilineComment,

        /// <summary>
        ///		Symbol representing an operation in T-SQL(T-SQL中表示操作的符号，),
        ///		e.g. + or !=.
        /// </summary>
        Operator,

        /// <summary>
        ///		Variable starting with @(以@开头的变量，),
        ///		e.g. @id.
        /// </summary>
        Variable,

        /// <summary>
        ///		Recognized server variables starting with @@(以@@开头的可识别服务器变量),
        ///		e.g. @@ROWCOUNT.
        /// </summary>
        SystemVariable,

        /// <summary>
        ///		Simple numeric value, with or without a decimal, without sign(简单的数值，有或没有小数点，没有符号，),
        ///		e.g. 210.5.
        /// </summary>
        NumericLiteral,

        /// <summary>
        ///		Unicode or non-Unicode string value(Unicode或非Unicode字符串值),
        ///		e.g. 'Cincinnati' or N'München'.
        /// </summary>
        StringLiteral,

        /// <summary>
        ///		Numeric value starting with a currency symbol(以货币符号开头的数值),
        ///		e.g. $4.25 or £3.42.
        /// </summary>
        MoneyLiteral,

        /// <summary>
        ///		Binary value serialized as hexadecimal and starting with 0x(序列化为十六进制的二进制值，从0x开始),
        ///		e.g. 0x69048AEFDD010E.
        /// </summary>
        BinaryLiteral,

        /// <summary>
        ///		The beginning of a multi-line comment(多行注释的开头),
        ///		e.g. /* something to comment.
        /// </summary>
        IncompleteComment,

        /// <summary>
        ///		The beginning of a bracketed identifier(带括号的标识符的开头),
        ///		e.g. [dbo.
        /// </summary>
        IncompleteIdentifier,

        /// <summary>
        ///		The beginning of a string literal(字符串字面值的开始),
        ///		e.g. 'Cincinnat.
        /// </summary>
        IncompleteString,

        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.transactsql.scriptdom.columntype
        // https://docs.microsoft.com/en-us/sql/t-sql/queries/select-clause-transact-sql
        // https://docs.microsoft.com/en-us/sql/t-sql/statements/merge-transact-sql

        /// <summary>
        ///		An identifier specifying a system defined column(指定系统定义的列的标识符),
        ///		e.g. $IDENTITY, $ROWGUID, $ACTION.
        /// </summary>
        SystemColumnIdentifier
    }

    public static class TSQLTokenTypeExtensions
    {
        public static bool In(this TSQLTokenType type, params TSQLTokenType[] types)
        {
            return types.Any(t => type == t);
        }
    }
}