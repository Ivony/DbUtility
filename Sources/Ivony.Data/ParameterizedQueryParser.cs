using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public abstract class ParameterizedQueryParser<TCommand, TParameter> : IParameterizedQueryParser<TCommand>
  {


    private object _sync = new object();

    public virtual object SyncRoot
    {
      get { return _sync; }
    }

    /// <summary>
    /// 创建查询命令
    /// </summary>
    /// <param name="query">参数化查询</param>
    /// <returns>查询命令</returns>
    public TCommand Parse( ParameterizedQuery query )
    {


      var length = query.ParameterValues.Length;

      TParameter[] parameters = new TParameter[length];
      string[] parameterPlaceholders = new string[length];

      lock ( SyncRoot )
      {
        var regex = ParameterizedQuery.ParameterPlaceholdRegex;

        var text = regex.Replace( query.TextTemplate, ( match ) =>
        {
          var index = int.Parse( match.Groups["index"].Value );

          if ( index >= length )
            throw new IndexOutOfRangeException( "分析参数化查询时遇到错误，参数索引超出边界" );

          var placeholder = parameterPlaceholders[index];
          if ( placeholder == null )
            placeholder = parameterPlaceholders[index] = GetParameterPlaceholder( query.ParameterValues[index], index, out parameters[index] );

          return placeholder;
        } );


        return CreateCommand( text.Replace( "##", "#" ), parameters.ToArray() );
      }
    }


    /// <summary>
    /// 派生类实现此方法产生一个参数对象，并生成一段占位符字符串。
    /// </summary>
    /// <param name="value">参数值</param>
    /// <param name="index">参数索引位置</param>
    /// <param name="parameter">参数对象</param>
    /// <returns>参数占位符</returns>
    protected abstract string GetParameterPlaceholder( object value, int index, out TParameter parameter );


    /// <summary>
    /// 创建命令对象
    /// </summary>
    /// <param name="commandText">命令文本</param>
    /// <param name="parameters">命令参数</param>
    /// <returns>命令对象</returns>
    protected abstract TCommand CreateCommand( string commandText, TParameter[] parameters );

  }
}
