using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public interface IParameterizedQueryParser
  {
    string CreateParameterPlacehold( object parameterValue );

    object CreateCommand( string commandText );
  }
}
