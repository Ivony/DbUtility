using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Queries;
using System.Data.SQLite;
using Ivony.Data.Common;

namespace Ivony.Data.SQLiteClient
{


  /// <summary>
  /// 用于转换参数化查询为 SQLiteCommand 对象的参数化查询解析器。
  /// </summary>
  public class SQLiteParameterizedQueryParser : ParameterizedQueryParser<SQLiteCommand, SQLiteParameter>
  {
    protected override string GetParameterPlaceholder( object value, int index, out SQLiteParameter parameter )
    {
      var name = ":Param" + index;
      parameter = new SQLiteParameter( name, value );
      return name;

    }

    protected override SQLiteCommand CreateCommand( string commandText, SQLiteParameter[] parameters )
    {
      var command = new SQLiteCommand( commandText );
      command.Parameters.AddRange( parameters );
      return command;
    }
  }
}
