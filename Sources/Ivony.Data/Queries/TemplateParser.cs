using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ivony.Data.Queries;

namespace Ivony.Data
{
  public class TemplateParser
  {

    private static Regex numberRegex = new Regex( @"\G\{(?<index>[0-9]+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );
    private static Regex rangeRegex = new Regex( @"\G\{(?<begin>[0-9]+)..(?<end>[0-9]+)?\}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );
    private static Regex allRegex = new Regex( @"\G\{...\}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );


    /// <summary>
    /// 解析查询模板
    /// </summary>
    /// <param name="templateText">模板文本</param>
    /// <param name="paramaters">模板参数</param>
    /// <returns>解析结果</returns>
    public static ParameterizedQuery ParseTemplate( string templateText, params object[] paramaters )
    {
      var builder = new ParameterizedQueryBuilder();
      return ParseTemplate( builder, templateText, paramaters );
    }



    /// <summary>
    /// 解析查询模板
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="templateText">模板文本</param>
    /// <param name="parameters">模板参数</param>
    /// <returns>解析结果</returns>
    public static ParameterizedQuery ParseTemplate( ParameterizedQueryBuilder builder, string templateText, params object[] parameters )
    {

      lock ( builder.SyncRoot )
      {

        for ( var i = 0; i < templateText.Length; i++ )
        {

          var ch = templateText[i];

          if ( ch == '{' )
          {

            if ( i == templateText.Length - 1 )
              throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );

            if ( templateText[i + 1] == '{' )
            {
              builder.Append( '{' );
              i++;
              continue;
            }

            var match = numberRegex.Match( templateText, i );

            if ( match.Success )
            {

              int parameterIndex;
              if ( !int.TryParse( match.Groups["index"].Value, out parameterIndex ) )
                throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );

              AddParameter( builder, parameters[parameterIndex] );

              i += match.Length - 1;
            }

            match = rangeRegex.Match( templateText, i );
            if ( match.Success )
            {
              int begin, end;
              if ( !int.TryParse( match.Groups["begin"].Value, out begin ) )
                throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );

              if ( match.Groups["end"] != null )
              {
                if ( !int.TryParse( match.Groups["end"].Value, out end ) )
                  throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );
              }
              else
                end = parameters.Length - 1;


              if ( begin > end || end >= parameters.Length )
                throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );


              for ( int parameterIndex = begin; parameterIndex < end; parameterIndex++ )
              {
                AddParameter( builder, parameters[parameterIndex] );
                builder.Append( "," );
              }

              AddParameter( builder, parameters[end] );

              i += match.Length - 1;
            }
          }

          else if ( ch == '}' )
          {
            if ( i == templateText.Length - 1 )
              throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );

            if ( templateText[i + 1] == '}' )
            {
              builder.Append( '}' );
              i++;
              continue;
            }
          }

          else
            builder.Append( ch );

        }


        return builder.CreateQuery();

      }
    }

    private static void AddParameter( ParameterizedQueryBuilder builder, object value )
    {
      var partial = value as ITemplatePartial;
      if ( partial != null )
        partial.Parse( builder );

      else
        builder.AppendParameter( value );
    }
  }
}
