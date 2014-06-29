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
    public IReadOnlyList<ParameterDescriptor> Parameters
    {
      get;
      private set;
    }



    /// <summary>
    /// 构建参数化查询对象
    /// </summary>
    /// <param name="template">查询文本模板</param>
    /// <param name="parameters">参数值</param>
    public ParameterizedQuery( string template, ParameterDescriptor[] parameters )
    {

      if ( template == null )
        throw new ArgumentNullException( "template" );

      if ( parameters == null )
        throw new ArgumentNullException( "values" );


      var conflict = parameters.Select( p => p.Name ).Where( name => name != null )
        .GroupBy( name => name, StringComparer.OrdinalIgnoreCase )
        .FirstOrDefault( g => g.Count() > 1 );

      if ( conflict != null )
        throw new ArgumentException( string.Format( "出现多个名为 \"{0}\" 的参数", conflict ), "parameters" );


      TextTemplate = template;
      var array =  new ParameterDescriptor[parameters.Length];
      parameters.CopyTo( array, 0 );
      Parameters = array;
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
        builder.AppendParameter( Parameters[parameterIndex] );

        index = match.Index + match.Length;
      }

      builder.AppendText( TextTemplate.Substring( index, TextTemplate.Length - index ) );
    }



    /// <summary>
    /// 重写 ToString 方法，输出参数化查询的字符串表达形式
    /// </summary>
    /// <returns>字符串表达形式</returns>
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

      for ( int i = 0; i < this.Parameters.Count; i++ )
      {
        writer.WriteLine( "#{0}#: {1}", i, Parameters[i] );
      }

      return writer.ToString();
    }




    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery operator +( ParameterizedQuery query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static DbExecutableQuery<ParameterizedQuery> operator +( DbExecutableQuery<ParameterizedQuery> query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> operator +( AsyncDbExecutableQuery<ParameterizedQuery> query1, ParameterizedQuery query2 )
    {
      return query1.Concat( query2 );
    }


    internal bool IsStartWithWhiteSpace()
    {
      return char.IsWhiteSpace( TextTemplate[0] );
    }



    private object _sync = new object();

    /// <summary>
    /// 设置输出参数的值
    /// </summary>
    /// <param name="values">输出参数的值列表</param>
    public void SetOutputParameterValue( IDictionary<string, object> values )
    {
      lock ( _sync )
      {

        var parameters = Parameters.Where( p => p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output )
          .ToDictionary( p => p.Name, p => p );

        var names = new HashSet<string>( parameters.Keys );
        if ( !names.SetEquals( values.Keys ) )
          throw new ArgumentException( "要设置的参数列表，与参数化查询中的输出参数列表不匹配", "values" );


        foreach ( var pair in values )
          parameters[pair.Key].SetValue( pair.Value );
      }
    }
  }
}
