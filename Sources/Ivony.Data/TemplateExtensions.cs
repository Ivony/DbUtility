using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Data.Expressions;

namespace Ivony.Data
{

  /// <summary>
  /// 有关模板的扩展方法
  /// </summary>
  public static class TemplateExtensions
  {


    /// <summary>
    /// 填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="template">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public static DataTable Data( this DbUtility dbUtility, string template, params object[] parameters )
    {
      return dbUtility.Data( Template( template, parameters ) );
    }

    /// <summary>
    /// 填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="dataSet">需要被填充的数据集</param>
    /// <param name="tableName">将最后一个表设置为什么名字</param>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public static DataTable Data( this DbUtility dbUtility, DataSet dataSet, string tableName, string template, params object[] parameters )
    {
      return dbUtility.Data( dataSet, tableName, Template( template, parameters ) );

    }

    /// <summary>
    /// 执行无结果的查询
    /// </summary>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public static int NonQuery( this DbUtility dbUtility, string commandText, params object[] parameters )
    {
      return dbUtility.NonQuery( Template( commandText, parameters ) );
    }

    /// <summary>
    /// 执行查询，并返回首行首列
    /// </summary>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public static object Scalar( this DbUtility dbUtility, string commandText, params object[] parameters )
    {
      return dbUtility.Scalar( Template( commandText, parameters ) );
    }

    /// <summary>
    /// 执行查询，并返回首行
    /// </summary>
    /// <param name="commandText">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public static DataRow FirstRow( this DbUtility dbUtility, string commandText, params object[] parameters )
    {
      return dbUtility.FirstRow( Template( commandText, parameters ) );
    }

    /// <summary>
    /// 查询数据库并将最后一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbUtility">DbUtility 实例</param>
    /// <param name="template">查询字符串模版</param>
    /// <param name="parameters">模版参数</param>
    /// <returns>实体集</returns>
    public static T[] Entities<T>( this DbUtility dbUtility, string template, params object[] parameters ) where T : new()
    {
      return dbUtility.Entities<T>( TemplateExtensions.Template( template, parameters ) );
    }

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dbUtility">DbUtility 实例</param>
    /// <param name="template">查询字符串模版</param>
    /// <param name="parameters">模版参数</param>
    /// <returns>实体</returns>
    public static T Entity<T>( this DbUtility dbUtility, string template, params object[] parameters ) where T : new()
    {
      return dbUtility.Entity<T>( TemplateExtensions.Template( template, parameters ) );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static TemplateExpression Template( string template, object[] parameters )
    {
      return new TemplateExpression( template, parameters );
    }


  }
}
