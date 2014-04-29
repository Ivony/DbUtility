using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  public struct FieldExpression
  {

    public FieldExpression( string fieldName, string tableName )
    {
      FieldName = fieldName;
      TableName = tableName;
    }

    public string FieldName { get; private set; }
    public string TableName { get; private set; }

  }
}
