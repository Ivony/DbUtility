using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public static class DbExpressions
  {
    public static DataTable ExecuteData( this ExecutableExpression expression )
    {
      return ExecuteData( expression.Expression, expression.DbUtility );
    }

    public static object ExecuteScalar( this ExecutableExpression expression )
    {
      return ExecuteScalar( expression.Expression, expression.DbUtility );
    }

    public static T ExecuteScalar<T>( this ExecutableExpression expression )
    {
      return ExecuteScalar<T>( expression.Expression, expression.DbUtility );
    }

    public static int ExecuteNonQuery( this ExecutableExpression expression )
    {
      return ExecuteNonQuery( expression.Expression, expression.DbUtility );
    }

    public static DataRow ExecuteFirstRow( this ExecutableExpression expression )
    {
      return ExecuteFirstRow( expression.Expression, expression.DbUtility );
    }




    /// <summary>
    /// 执行无结果的查询
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <param name="dbUtility">数据库访问帮助器</param>
    /// <returns>影响的行数</returns>
    public static int ExecuteNonQuery( this IDbExpression expression, DbUtility dbUtility )
    {
      return dbUtility.ExecuteNonQuery( expression );
    }

    /// <summary>
    /// 执行查询，并返回首行首列
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <param name="dbUtility">数据库访问帮助器</param>
    /// <returns>查询结果的首行首列</returns>
    public static object ExecuteScalar( this IDbExpression expression, DbUtility dbUtility )
    {
      return dbUtility.ExecuteScalar( expression );
    }

    /// <summary>
    /// 执行查询，并返回首行首列
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <param name="dbUtility">数据库访问帮助器</param>
    /// <returns>查询结果的首行首列</returns>
    public static T ExecuteScalar<T>( this IDbExpression expression, DbUtility dbUtility )
    {
      return (T) dbUtility.ExecuteScalar( expression );
    }


    /// <summary>
    /// 执行查询，并返回首行
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <param name="dbUtility">数据库访问帮助器</param>
    /// <returns>查询结果的首行</returns>
    public static DataRow ExecuteFirstRow( this IDbExpression expression, DbUtility dbUtility )
    {
      return dbUtility.ExecuteFirstRow( expression );
    }


    /// <summary>
    /// 执行查询，并返回第一个结果集
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <param name="dbUtility">数据库访问帮助器</param>
    /// <returns>第一个结果集</returns>
    public static DataTable ExecuteData( this IDbExpression expression, DbUtility dbUtility )
    {
      return dbUtility.ExecuteData( expression );
    }


    /// <summary>
    /// 创建模版表达式实例
    /// </summary>
    /// <param name="template">SQL 命令模版</param>
    /// <param name="parameters">模版参数列表</param>
    /// <returns>模版表达式</returns>
    public static TemplateExpression Template( string template, params object[] parameters )
    {
      return new TemplateExpression( template, parameters );
    }

  }
}
