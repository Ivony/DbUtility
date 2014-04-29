using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public abstract class ParameterizedQueryLiteralValueParser<TCommand> : IParameterizedQueryParser<TCommand>
  {


    /// <summary>
    /// 创建查询命令
    /// </summary>
    /// <param name="query">参数化查询</param>
    /// <returns>查询命令</returns>
    public TCommand Parse( ParameterizedQuery query )
    {

      var regex = ParameterizedQuery.ParameterPlaceholdRegex;




      var text = regex.Replace( query.TextTemplate, ( match ) =>
      {
        var index = int.Parse( match.Groups["index"].Value );

        return GetLiteralValue( query.ParameterValues[index] );

      } );


      return CreateCommand( text.Replace( "##", "#" ) );
    }

    protected abstract TCommand CreateCommand( string commandText );

    protected abstract string GetLiteralValue( object value );

  }
}
