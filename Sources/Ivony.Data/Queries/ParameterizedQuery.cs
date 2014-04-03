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

    /// <summary>
    /// 定义匹配参数占位符的正则表达式
    /// </summary>
    public static readonly Regex ParameterPlaceholdRegex = new Regex( @"#(?<index>[0-9]+)#" );



    /// <summary>
    /// 获取查询文本模板
    /// </summary>
    public string TextTemplate
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取参数值
    /// </summary>
    public object[] ParameterValues
    {
      get;
      private set;
    }



    /// <summary>
    /// 构建参数化查询对象
    /// </summary>
    /// <param name="template">查询文本模板</param>
    /// <param name="values">参数值</param>
    public ParameterizedQuery( string template, object[] values )
    {
      TextTemplate = template;
      ParameterValues = new object[values.Length];
      values.CopyTo( ParameterValues, 0 );
    }



    /// <summary>
    /// 创建查询命令
    /// </summary>
    /// <typeparam name="T">查询命令类型</typeparam>
    /// <param name="provider">参数化查询命令提供程序</param>
    /// <returns>查询命令</returns>
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


    /// <summary>
    /// 将参数化查询解析为另一个参数化查询的一部分。
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
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
