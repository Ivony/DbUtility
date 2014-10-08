using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Data.Queries;
using System.Text.RegularExpressions;
using Ivony.Data.Common;

namespace Ivony.Data
{

  /// <summary>
  /// 有关模板的扩展方法
  /// </summary>
  public static class TemplateExtensions
  {

    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static DbExecutableQuery<ParameterizedQuery> Template( this IDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return new DbExecutableQuery<ParameterizedQuery>( executor, Db.Template( template, parameters ) );
    }


    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> Template( this IAsyncDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return new AsyncDbExecutableQuery<ParameterizedQuery>( executor, Db.Template( template, parameters ) );
    }


    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static DbExecutableQuery<ParameterizedQuery> T( this IDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return Template( executor, template, parameters );
    }

    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> T( this IAsyncDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return Template( executor, template, parameters );
    }





    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static DbExecutableQuery<ParameterizedQuery> Template( this IDbTransactionContext<IDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }


    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> Template( this IDbTransactionContext<IAsyncDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }


    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static DbExecutableQuery<ParameterizedQuery> T( this IDbTransactionContext<IDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }


    /// <summary>
    /// 根据模板表达式创建参数化查询实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>参数化查询实例</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> T( this IDbTransactionContext<IAsyncDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }



    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="secondQuery">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery Concat( this ParameterizedQuery firstQuery, ParameterizedQuery secondQuery )
    {
      return ConcatQueries( firstQuery, secondQuery );
    }

    /// <summary>
    /// 串联多个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="otherQueries">要串联的其他参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery ConcatQueries( this ParameterizedQuery firstQuery, params ParameterizedQuery[] otherQueries )
    {
      var builder = new ParameterizedQueryBuilder();

      firstQuery.AppendTo( builder );
      foreach ( var query in otherQueries )
      {

        if ( query == null || string.IsNullOrEmpty( query.TextTemplate ) )
          continue;

        if ( !builder.IsEndWithWhiteSpace() && !query.IsStartWithWhiteSpace() && Db.AddWhiteSpaceOnConcat )
          builder.Append( ' ' );

        query.AppendTo( builder );
      }

      return builder.CreateQuery();
    }



    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="secondQuery">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static DbExecutableQuery<ParameterizedQuery> Concat( this DbExecutableQuery<ParameterizedQuery> firstQuery, ParameterizedQuery secondQuery )
    {
      return ConcatQueries( firstQuery, secondQuery );
    }


    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="secondQuery">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> Concat( this AsyncDbExecutableQuery<ParameterizedQuery> firstQuery, ParameterizedQuery secondQuery )
    {
      return ConcatQueries( firstQuery, secondQuery );
    }


    /// <summary>
    /// 串联多个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="otherQueries">要串联的其他参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static DbExecutableQuery<ParameterizedQuery> ConcatQueries( this DbExecutableQuery<ParameterizedQuery> firstQuery, params ParameterizedQuery[] otherQueries )
    {
      var query = ConcatQueries( firstQuery.Query, otherQueries );
      return new DbExecutableQuery<ParameterizedQuery>( firstQuery.Executor, query );
    }


    /// <summary>
    /// 串联多个参数化查询对象
    /// </summary>
    /// <param name="firstQuery">第一个参数化查询对象</param>
    /// <param name="otherQueries">要串联的其他参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> ConcatQueries( this AsyncDbExecutableQuery<ParameterizedQuery> firstQuery, params ParameterizedQuery[] otherQueries )
    {
      var query = ConcatQueries( firstQuery.Query, otherQueries );
      return new AsyncDbExecutableQuery<ParameterizedQuery>( firstQuery.Executor, query );
    }




    /// <summary>
    /// 通过模板产生一个参数化查询对象并串联到现有的参数化查询对象之后
    /// </summary>
    /// <param name="firstQuery">需要被串联的参数化查询对象</param>
    /// <param name="templateText">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static DbExecutableQuery<ParameterizedQuery> Concat( this DbExecutableQuery<ParameterizedQuery> firstQuery, string templateText, params object[] parameters )
    {
      return Concat( firstQuery, Db.T( templateText, parameters ) );
    }


    /// <summary>
    /// 通过模板产生一个参数化查询对象并串联到现有的参数化查询对象之后
    /// </summary>
    /// <param name="firstQuery">需要被串联的参数化查询对象</param>
    /// <param name="templateText">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> Concat( this AsyncDbExecutableQuery<ParameterizedQuery> firstQuery, string templateText, params object[] parameters )
    {
      return Concat( firstQuery, Db.T( templateText, parameters ) );
    }

  }
}
