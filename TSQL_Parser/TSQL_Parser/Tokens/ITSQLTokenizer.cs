using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSQL.Tokens
{
	public interface ITSQLTokenizer : IEnumerator, IEnumerable, IEnumerator<TSQLToken>, IEnumerable<TSQLToken>
	{
		
	}
}
