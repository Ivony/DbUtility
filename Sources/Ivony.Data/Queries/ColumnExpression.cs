using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  public struct ColumnExpression
  {
    public ColumnExpression( string fieldName, string tableName, string alias = null ) : this( new FieldExpression( fieldName, tableName ), alias ) { }


    public ColumnExpression( FieldExpression field, string alias = null )
    {
      Field = field;
      Alias = alias;
    }

    public FieldExpression Field { get; private set; }

    public string Alias { get; private set; }

  }
}
