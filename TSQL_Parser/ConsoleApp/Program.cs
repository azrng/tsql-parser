using ConsoleApp;
using TSQL;
using TSQL.Statements;
using TSQL.Tokens;

var str = File.ReadAllText("sample.txt");

TSQLSelectStatement select = TSQLStatementReader.ParseStatements(str, false, false)[0] as TSQLSelectStatement;

Console.WriteLine("SELECT:");
foreach (TSQLToken token in select.Select.Tokens)
{
    Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
}

if (select.From != null)
{
    Console.WriteLine("FROM:");
    foreach (TSQLToken token in select.From.Tokens)
    {
        Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
    }
}
Console.WriteLine("WHERE:");
foreach (TSQLToken token in select.Where.Tokens)
{
    Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
}

var aa = select.Where.Tokens.Count(t => t.Type == TSQLTokenType.Connector);
var andCount = select.Where.Tokens.Count(t => t.Type == TSQLTokenType.Connector) == 0 ? 1 : select.Where.Tokens.Count(t => t.Type == TSQLTokenType.Connector) + 1;
var list = new List<AnalysisSqlDetailsDto>(andCount);
Console.WriteLine("WHERE2:");
var lastNum = 1;
for (int i = 0; i < andCount; i++)
{
    list.Add(new AnalysisSqlDetailsDto());

    for (int q = lastNum; q < select.Where.Tokens.Count; q++)
    {
        if (select.Where.Tokens[q].Type == TSQLTokenType.Identifier || select.Where.Tokens[q].Type == TSQLTokenType.Character)
        {
            list[i].Field = list[i].Field + select.Where.Tokens[q].Text;
            lastNum = q;
        }
        else if (select.Where.Tokens[q].Type == TSQLTokenType.Operator)
        {
            list[i].Operator = list[i].Operator + select.Where.Tokens[q].Text + " ";
            lastNum = q;
        }
        else if (select.Where.Tokens[q].Type == TSQLTokenType.Connector)
        {
            lastNum = q;
            lastNum++;
            break;
        }
        else
        {
            list[i].Value = list[i].Value + select.Where.Tokens[q].Text;
            lastNum = q;
        }
    }
}
foreach (var item in list)
{
    Console.WriteLine($"列名：{item.Field} 操作符：{item.Operator} 值：{item.Value}");
}

if (select.OrderBy != null)
{
    Console.WriteLine("OrderBy:");
    foreach (TSQLToken token in select.From.Tokens)
    {
        Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text);
    }
}

Console.WriteLine("测试");