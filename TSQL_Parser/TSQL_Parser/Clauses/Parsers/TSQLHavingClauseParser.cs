using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TSQL.Statements;
using TSQL.Statements.Parsers;
using TSQL.Tokens;

namespace TSQL.Clauses.Parsers
{
	internal class TSQLHavingClauseParser : ITSQLClauseParser
	{
		public TSQLHavingClause Parse(IEnumerator<TSQLToken> tokenizer)
		{
			TSQLHavingClause having = new TSQLHavingClause();

			if (
				tokenizer.Current == null ||
				tokenizer.Current.Type != TSQLTokenType.Keyword ||
				tokenizer.Current.AsKeyword.Keyword != TSQLKeywords.HAVING)
			{
				throw new ApplicationException("HAVING expected.");
			}

			having.Tokens.Add(tokenizer.Current);

			// subqueries
			int nestedLevel = 0;

			while (
				tokenizer.MoveNext() &&
				!(
					tokenizer.Current.Type == TSQLTokenType.Character &&
					tokenizer.Current.AsCharacter.Character == TSQLCharacters.Semicolon
				) &&
				!(
					nestedLevel == 0 &&
					tokenizer.Current.Type == TSQLTokenType.Character &&
					tokenizer.Current.AsCharacter.Character == TSQLCharacters.CloseParentheses
				) &&
				(
					nestedLevel > 0 ||
					tokenizer.Current.Type != TSQLTokenType.Keyword ||
					(
						tokenizer.Current.Type == TSQLTokenType.Keyword &&
						tokenizer.Current.AsKeyword.Keyword.In
						(
							TSQLKeywords.NULL,
							TSQLKeywords.CASE,
							TSQLKeywords.WHEN,
							TSQLKeywords.THEN,
							TSQLKeywords.ELSE,
							TSQLKeywords.AND,
							TSQLKeywords.OR,
							TSQLKeywords.BETWEEN,
							TSQLKeywords.EXISTS,
							TSQLKeywords.END,
							TSQLKeywords.IN,
							TSQLKeywords.IS,
							TSQLKeywords.NOT,
							TSQLKeywords.LIKE
						)
					)
				))
			{
				having.Tokens.Add(tokenizer.Current);

				if (tokenizer.Current.Type == TSQLTokenType.Character)
				{
					TSQLCharacters character = tokenizer.Current.AsCharacter.Character;

					if (character == TSQLCharacters.OpenParentheses)
					{
						// should we recurse for subqueries?
						nestedLevel++;

						if (tokenizer.MoveNext())
						{
							if (
								tokenizer.Current.Type == TSQLTokenType.Keyword &&
								tokenizer.Current.AsKeyword.Keyword == TSQLKeywords.SELECT)
							{
								TSQLSelectStatement selectStatement = new TSQLSelectStatementParser().Parse(tokenizer);

								having.Tokens.AddRange(selectStatement.Tokens);

								if (
									tokenizer.Current != null &&
									tokenizer.Current.Type == TSQLTokenType.Character &&
									tokenizer.Current.AsCharacter.Character == TSQLCharacters.CloseParentheses)
								{
									nestedLevel--;
									having.Tokens.Add(tokenizer.Current);
								}
							}
							else
							{
								having.Tokens.Add(tokenizer.Current);
							}
						}
					}
					else if (character == TSQLCharacters.CloseParentheses)
					{
						nestedLevel--;
					}
				}
			}

			return having;
		}

		TSQLClause ITSQLClauseParser.Parse(IEnumerator<TSQLToken> tokenizer)
		{
			return Parse(tokenizer);
		}
	}
}
