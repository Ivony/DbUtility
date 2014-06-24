using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{
  public sealed class DbParameter
  {


    public DbParameter( string name, object value, DbDataType type, ParameterDirection direction )
    {
      Name = name;
      Value = value;
      DbType = type;
      Direction = direction;

    }


    public string Name
    {
      get;
      private set;
    }

    public object Value
    {
      get;
      private set;
    }


    public DbDataType DbType
    {
      get;
      private set;
    }

    public ParameterDirection Direction
    {
      get;
      private set;
    }

  }
}
