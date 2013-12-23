using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public interface IParameterizedQueryParser<TCommand>
  {
    string CreateParameterPlacehold( object parameterValue );

    TCommand CreateCommand( string commandText );
  }
}
