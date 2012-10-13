using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data.Expressions
{
  /// <summary>
  /// 数据表表达式
  /// </summary>
  public class TableExpression : IDbExpression
  {

    private string _tableName;

    public TableExpression( string tableName )
    {
      _tableName = tableName;
    }

    private string[] _fields = null;

    public TableExpression Fields( params string[] fields )
    {
      _fields = fields;
      return this;
    }


    private TemplateExpression _where;

    public TableExpression Where( string whereClause )
    {
      _where = Template( whereClause );
      return this;
    }

    private static TemplateExpression Template( string template, params object[] parameters )
    {
      return new TemplateExpression( template, parameters );
    }



  }
}
