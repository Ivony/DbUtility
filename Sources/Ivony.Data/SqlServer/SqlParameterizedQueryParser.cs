using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Ivony.Data.SqlServer
{
  public class SqlParameterizedQueryParser : IParameterizedQueryParser
  {



    private int index = 0;
    private IList<SqlParameter> parameterList = new List<SqlParameter>();

    public string CreateParameterPlacehold( object parameterValue )
    {
      var name = "Param" + index++;
      var parameter = new SqlParameter( name, parameterValue );

      return name;
    }

    public object CreateCommand( string commandText )
    {
      var command = new SqlCommand();
      command.CommandText = commandText;
      command.Parameters.AddRange( parameterList.ToArray() );

      return command;
    }
  }
}
