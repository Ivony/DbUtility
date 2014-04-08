using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Fluent;
using Ivony.Data.Queries;
using System.Text.RegularExpressions;

namespace Ivony.Data
{

  /// <summary>
  /// 有关模板的扩展方法
  /// </summary>
  public static class TemplateExtensions
  {

    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static DbExecutableQuery<ParameterizedQuery> Template( this IDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return new DbExecutableQuery<ParameterizedQuery>( executor, TemplateParser.ParseTemplate( template, parameters ) );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> Template( this IAsyncDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return new AsyncDbExecutableQuery<ParameterizedQuery>( executor, TemplateParser.ParseTemplate( template, parameters ) );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static DbExecutableQuery<ParameterizedQuery> T( this IDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return Template( executor, template, parameters );
    }

    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="executor">查询执行器</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> T( this IAsyncDbExecutor<ParameterizedQuery> executor, string template, params object[] parameters )
    {
      return Template( executor, template, parameters );
    }





    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static DbExecutableQuery<ParameterizedQuery> Template( this IDbTransactionContext<IDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> Template( this IDbTransactionContext<IAsyncDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static DbExecutableQuery<ParameterizedQuery> T( this IDbTransactionContext<IDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="transaction">数据库事务对象</param>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static AsyncDbExecutableQuery<ParameterizedQuery> T( this IDbTransactionContext<IAsyncDbExecutor<ParameterizedQuery>> transaction, string template, params object[] parameters )
    {
      return transaction.DbExecutor.Template( template, parameters );
    }


  }
}
