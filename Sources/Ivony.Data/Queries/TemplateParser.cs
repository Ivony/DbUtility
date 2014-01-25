using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  public class TemplateParser
  {

    private static Regex numberRegex = new Regex( "^[0-9]+", RegexOptions.Compiled | RegexOptions.CultureInvariant );


    /// <summary>
    /// 解析查询模板
    /// </summary>
    /// <param name="templateText">模板文本</param>
    /// <param name="paramaters">模板参数</param>
    /// <returns>解析结果</returns>
    public static ParameterizedQuery ParseTemplate( string templateText, object[] paramaters )
    {
      var builder = new ParameterizedQueryBuilder();
      return ParseTemplate( builder, templateText, paramaters );
    }



    /// <summary>
    /// 解析查询模板
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="templateText">模板文本</param>
    /// <param name="paramaters">模板参数</param>
    /// <returns>解析结果</returns>
    public static ParameterizedQuery ParseTemplate( ParameterizedQueryBuilder builder, string templateText, object[] paramaters )
    {

      lock ( builder.SyncRoot )
      {

        for ( var i = 0; i < templateText.Length; i++ )
        {

          var ch = templateText[i];

          if ( ch == '{' )
          {

            if ( templateText[i + 1] == '{' )
            {
              builder.Append( '{' );
              i++;
              continue;
            }

            var match = numberRegex.Match( templateText, i );

            if ( !match.Success )
              throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );

            int parameterIndex;
            if ( !int.TryParse( match.Value, out parameterIndex ) )
              throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );

            if ( parameterIndex >= paramaters.Length )
              throw new ArgumentOutOfRangeException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题，索引超出数组界限", templateText, i ) );


            i += match.Length;

            if ( templateText[i] != '}' )
              throw new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );


            var value = paramaters[parameterIndex];
            var partial = value as ITemplatePartial;
            if ( partial != null )
              partial.Parse( builder );

            else
              builder.AppendParameter( value );
          }

          else if ( ch == '}' )
          {
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
  }
}
