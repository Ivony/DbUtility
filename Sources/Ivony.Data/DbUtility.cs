using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using Ivony.Data.Expressions;



namespace Ivony.Data
{
  /// <summary>
  /// 所有数据库访问帮助器的基类
  /// </summary>
  public abstract class DbUtility
  {


    /// <summary>
    /// 获取DbExpressionParser实例，用于分析数据查询表达式
    /// </summary>
    /// <returns>DbExpressionParser实例</returns>
    protected abstract IDbExpressionParser GetExpressionParser();

    /// <summary>
    /// 创建 DbCommand 对象
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <returns>DbCommand 对象</returns>
    protected virtual IDbCommand CreateCommand( IDbExpression expression )
    {
      var parser = GetExpressionParser();
      return parser.Parse( expression );
    }



    /// <summary>
    /// 执行无结果的查询
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <returns>影响的行数</returns>
    public virtual int ExecuteNonQuery( IDbExpression expression )
    {
      IDbCommand command = CreateCommand( expression );
      try
      {
        OnCommandExecuting( this, command );
        var result = ExecuteNonQuery( command );
        OnCommandExecuted( this, command );

        return result;
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }

    /// <summary>
    /// 执行查询，并返回首行首列
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <returns>查询结果的首行首列</returns>
    public virtual object ExecuteScalar( IDbExpression expression )
    {
      IDbCommand command = CreateCommand( expression );
      try
      {
        OnCommandExecuting( this, command );
        var result = ExecuteScalar( command );
        OnCommandExecuted( this, command );

        return result;
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }

    /// <summary>
    /// 执行查询，并返回首行
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <returns>查询结果的首行</returns>
    public virtual DataRow ExecuteFirstRow( IDbExpression expression )
    {

      IDbCommand command = CreateCommand( expression );

      try
      {
        OnCommandExecuting( this, command );
        var result = ExecuteFirstRow( command );
        OnCommandExecuted( this, command );

        return result;
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }

    /// <summary>
    /// 执行查询，并返回第一个结果集
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <returns>第一个结果集</returns>
    public virtual DataTable ExecuteData( IDbExpression expression )
    {

      var command = CreateCommand( expression );
      try
      {
        OnCommandExecuting( this, command );
        var result = ExecuteData( command );
        OnCommandExecuted( this, command );

        return result;
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }






    /// <summary>
    /// 执行无结果的查询
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns>受影响的行数。</returns>
    public static int ExecuteNonQuery( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return command.ExecuteNonQuery();
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// 协助执行查询，并返回首行首列
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns></returns>
    public static object ExecuteScalar( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return command.ExecuteScalar();
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
        return command.ExecuteScalar();
    }

    /// <summary>
    /// 协助执行查询，并返回首行
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns></returns>
    public static DataRow ExecuteFirstRow( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return ExecuteFirstRowPrivate( command );
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
      {
        return ExecuteFirstRowPrivate( command );
      }
    }

    private static DataRow ExecuteFirstRowPrivate( IDbCommand command )
    {
      using ( IDataReader reader = command.ExecuteReader( CommandBehavior.SingleRow | CommandBehavior.SingleResult ) )
      {
        DataTable table = new DataTable();

        table.Load( reader );

        if ( table.Rows.Count < 1 )
          return null;
        else
          return table.Rows[0];
      }
    }

    /// <summary>
    /// 执行查询，并返回第一个结果集
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns>第一个结果集</returns>
    public static DataTable ExecuteData( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return ExecuteDataPrivate( command );
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
        return ExecuteDataPrivate( command );
    }

    private static DataTable ExecuteDataPrivate( IDbCommand command )
    {
      using ( IDataReader reader = command.ExecuteReader( CommandBehavior.SingleResult ) )
      {
        DataTable table = new DataTable();

        table.Load( reader );

        return table;
      }
    }

    /// <summary>
    /// 利用 DataAdapter 填充 DataSet
    /// </summary>
    /// <param name="adapter">用于填充 DataSet 的 Adapter 对象</param>
    /// <returns>填充后的 DataSet 对象</returns>
    public static DataSet Fill( IDataAdapter adapter )
    {
      return Fill( adapter, null );
    }

    /// <summary>
    /// 利用 DataAdapter 填充 DataSet
    /// </summary>
    /// <param name="adapter">用于填充 DataSet 的 Adapter 对象</param>
    /// <param name="dataSet">要被填充的 DataSet 对象</param>
    /// <returns>填充后的 DataSet 对象</returns>
    public static DataSet Fill( IDataAdapter adapter, DataSet dataSet )
    {

      if ( dataSet == null )
        dataSet = new DataSet();

      adapter.Fill( dataSet );

      return dataSet;

    }



    /// <summary>
    /// 执行命令，并返回DataReader对象，请注意数据库连接将在DataReader关闭的同时关闭。
    /// </summary>
    /// <param name="expression">查询表达式</param>
    /// <returns></returns>
    public IDataReader ExecuteReader( IDbExpression expression )
    {
      IDbCommand command = CreateCommand( expression );

      try
      {
        return ExecuteReader( command );
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }

    /// <summary>
    /// 协助执行查询，并返回DataReader对象
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns></returns>
    public static IDataReader ExecuteReader( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        command.Connection.Open();

        return command.ExecuteReader( CommandBehavior.CloseConnection );
      }
      else
        return command.ExecuteReader();
    }


    /// <summary>
    /// 当发生错误时引发此事件
    /// </summary>
    public event EventHandler<DbErrorEventArgs> Error;

    /// <summary>
    /// 当查询发生异常时
    /// </summary>
    /// <param name="sender">产生事件的源</param>
    /// <param name="exception">异常信息</param>
    /// <param name="command">产生异常的命令对象</param>
    protected void OnError( object sender, DbException exception, IDbCommand command )
    {
      EventHandler<DbErrorEventArgs> handler = Error;
      if ( handler != null )
      {
        handler( sender, new DbErrorEventArgs( exception, command ) );
      }
    }


    /// <summary>
    /// 当执行某个命令前引发此事件
    /// </summary>
    public event EventHandler<DbCommandEventArgs> CommandExecuting;

    protected void OnCommandExecuting( object sender, IDbCommand command )
    {
      if ( CommandExecuting != null )
        CommandExecuting( sender, new DbCommandEventArgs( command ) );
    }

    /// <summary>
    /// 当成功执行某个命令后引发此事件
    /// </summary>
    public event EventHandler<DbCommandEventArgs> CommandExecuted;

    protected void OnCommandExecuted( object sender, IDbCommand command )
    {
      if ( CommandExecuted != null )
        CommandExecuted( sender, new DbCommandEventArgs( command ) );
    }




    /// <summary>
    /// 为数据库操作事件提供事件参数
    /// </summary>
    public class DbCommandEventArgs : EventArgs
    {
      /// <summary>
      /// 创建 DbCommandEventArgs 对象
      /// </summary>
      /// <param name="command">执行的查询命令</param>
      public DbCommandEventArgs( IDbCommand command )
      {
        Command = command;
      }

      /// <summary>
      /// 获取执行的查询命令对象
      /// </summary>
      public IDbCommand Command
      {
        get;
        private set;
      }

    }


    /// <summary>
    /// 为 Error 事件提供参数
    /// </summary>
    public class DbErrorEventArgs : EventArgs
    {
      private DbException _exception;
      private IDbCommand _command;

      public DbErrorEventArgs( DbException exception, IDbCommand command )
      {
        _exception = exception;
        _command = command;
      }

      /// <summary>
      /// 异常信息
      /// </summary>
      public Exception Exception
      {
        get { return _exception; }
      }

      /// <summary>
      /// 导致异常的命令对象
      /// </summary>
      public IDbCommand Command
      {
        get
        {
          return _command;
        }
      }
    }

    /// <summary>
    /// 创建事务管理器
    /// </summary>
    /// <returns>事务管理器</returns>
    public virtual ITransactionUtility CreateTransaction()
    {
      throw new NotSupportedException();
    }
  }
}