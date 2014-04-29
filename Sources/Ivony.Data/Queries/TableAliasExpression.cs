using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  public struct TableAliasExpression
  {

    public TableAliasExpression( string tableName, string alias )
    {
      TableName = tableName;
      Alias = alias;
    }


    public string TableName { get; private set; }
    public string Alias { get; private set; }

  }
}
