using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{

  /// <summary>
  /// 代表一个参数化查询
  /// </summary>
  public class ParameterizedQuery : IDbQuery, IParameterizedQueryPartial
  {

    /// <summary>
    /// 定义匹配参数占位符的正则表达式
    /// </summary>
    public static readonly Regex ParameterPlaceholdRegex = new Regex( @"#(?<index>[0-9]+)#", RegexOptions.Compiled );



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

      if ( template == null )
        throw new ArgumentNullException( "template" );

      if ( values == null )
        throw new ArgumentNullException( "values" );

      TextTemplate = template;
      ParameterValues = new object[values.Length];
      values.CopyTo( ParameterValues, 0 );
    }


    /// <summary>
    /// 将参数化查询解析为另一个参数化查询的一部分。
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    public void AppendTo( ParameterizedQueryBuilder builder )
    {

      int index = 0;

      foreach ( Match match in ParameterPlaceholdRegex.Matches( TextTemplate ) )
      {

        var length = match.Index - index;
        if ( length > 0 )
          builder.AppendText( TextTemplate.Substring( index, length ) );


        var parameterIndex = int.Parse( match.Groups["index"].Value );
        builder.AppendParameter( ParameterValues[parameterIndex] );

        index = match.Index + match.Length;
      }

      builder.AppendText( TextTemplate.Substring( index, TextTemplate.Length - index ) );
    }


    public override string ToString()
    {
      if ( stringExpression == null )
        stringExpression = GetString();

      return stringExpression;
    }

    private string stringExpression;

    private string GetString()
    {
      var writer = new StringWriter();

      writer.WriteLine( "\"" + TextTemplate.Replace( "\"", "\"\"" ) + "\"" );
      writer.WriteLine();

      for ( int i = 0; i < this.ParameterValues.Length; i++ )
      {
        writer.WriteLine( "#{0}#: {1}", i, ParameterValues[i] );
      }

      return writer.ToString();
    }




    public static ParameterizedQuery operator +( ParameterizedQuery query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }

    public static DbExecutableQuery<ParameterizedQuery> operator +( DbExecutableQuery<ParameterizedQuery> query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }

    public static AsyncDbExecutableQuery<ParameterizedQuery> operator +( AsyncDbExecutableQuery<ParameterizedQuery> query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }


    internal bool IsStartWithWhiteSpace()
    {
      return char.IsWhiteSpace( TextTemplate[0] );
    }
  }
}
