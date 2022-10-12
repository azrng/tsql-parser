using TSQL;
using TSQL.Statements;
using TSQL.Tokens;

var str = File.ReadAllText("sample.txt");
TSQLSelectStatement select = TSQLStatementReader.ParseStatements(str)[0] as TSQLSelectStatement;

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
    Console.WriteLine("\ttype: " + token.Type.ToString() + ", value: " + token.Text + "    comm" + token.IsComplete);
}

Console.WriteLine("测试");