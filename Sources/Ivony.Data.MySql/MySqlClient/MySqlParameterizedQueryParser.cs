using Ivony.Data.Common;
using Ivony.Data.Queries;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.MySqlClient
{
  public class MySqlParameterizedQueryParser : ParameterizedQueryParser<MySqlCommand, MySqlParameter>
  {
    protected override string GetParameterPlaceholder( object value, int index, out MySqlParameter parameter )
    {
      var name = "?Param" + index;
      parameter = new MySqlParameter( name, value );

      return name;
    }

    protected override MySqlCommand CreateCommand( string commandText, MySqlParameter[] parameters )
    {
      var command = new MySqlCommand( commandText );
      command.Parameters.AddRange( parameters );

      return command;
    }
  }
}
