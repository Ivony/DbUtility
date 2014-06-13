using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Fluent;
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
      return new DbExecutableQuery<ParameterizedQuery>( executor, TemplateParser.ParseTemplate( template, parameters ) );
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
      return new AsyncDbExecutableQuery<ParameterizedQuery>( executor, TemplateParser.ParseTemplate( template, parameters ) );
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



    public static ParameterizedQuery Concat( this ParameterizedQuery firstQuery, params ParameterizedQuery[] otherQuerys )
    {
      var builder = new ParameterizedQueryBuilder();

      firstQuery.AppendTo( builder );
      foreach ( var query in otherQuerys )
      {
        if ( !builder.IsEndWithWhiteSpace() && !query.IsStartWithWhiteSpace() && Db.AddWhiteSpaceOnConcat )
          builder.Append( ' ' );

        query.AppendTo( builder );
      }

      return builder.CreateQuery();
    }


    public static DbExecutableQuery<ParameterizedQuery> Concat( this DbExecutableQuery<ParameterizedQuery> firstQuery, params ParameterizedQuery[] otherQueries )
    {
      var query = Concat( firstQuery.Query, otherQueries );
      return new DbExecutableQuery<ParameterizedQuery>( firstQuery.Executor, query );
    }


    public static AsyncDbExecutableQuery<ParameterizedQuery> Concat( this AsyncDbExecutableQuery<ParameterizedQuery> firstQuery, params ParameterizedQuery[] otherQueries )
    {
      var query = Concat( firstQuery.Query, otherQueries );
      return new AsyncDbExecutableQuery<ParameterizedQuery>( firstQuery.Executor, query );
    }




    public static DbExecutableQuery<ParameterizedQuery> Concat( this DbExecutableQuery<ParameterizedQuery> firstQuery, string templateText, params object[] parameters )
    {
      return Concat( firstQuery, Db.T( templateText, parameters ) );
    }


    public static AsyncDbExecutableQuery<ParameterizedQuery> Concat( this AsyncDbExecutableQuery<ParameterizedQuery> firstQuery, string templateText, params object[] parameters )
    {
      return Concat( firstQuery, Db.T( templateText, parameters ) );
    }

  }
}
