using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{

  /// <summary>
  /// 代表一个参数化查询
  /// </summary>
  public class ParameterizedQuery : IDbQuery, ITemplatePartial
  {

    public static readonly Regex ParameterPlaceholdRegex = new Regex( @"#(?<index>[0-9]+)#" );


    public string TextTemplate
    {
      get;
      private set;
    }


    public object[] ParameterValues
    {
      get;
      private set;
    }



    public ParameterizedQuery( string template, object[] values )
    {
      TextTemplate = template;
      ParameterValues = new object[values.Length];
      values.CopyTo( ParameterValues, 0 );
    }



    public T CreateCommand<T>( IParameterizedQueryParser<T> provider )
    {

      lock ( provider.SyncRoot )
      {

        var text = ParameterPlaceholdRegex.Replace( TextTemplate, ( match ) =>
        {
          var index = int.Parse( match.Groups["index"].Value );
          return provider.CreateParameterPlacehold( ParameterValues[index] );
        } );


        return provider.CreateCommand( text.Replace( "##", "#" ) );
      }
    }

    public void Parse( ParameterizedQueryBuilder builder )
    {

      int index = 0;

      foreach ( Match match in ParameterPlaceholdRegex.Matches( TextTemplate ) )
      {

        var length = match.Index - index;
        if ( length > 0 )
          builder.Append( TextTemplate.Substring( index, length ) );


        var parameterIndex = int.Parse( match.Groups["index"].Value );
        builder.AppendParameter( ParameterValues[parameterIndex] );

        index += match.Length;
      }

      builder.Append( TextTemplate.Substring( index, TextTemplate.Length - index ) );
    }
  }
}
