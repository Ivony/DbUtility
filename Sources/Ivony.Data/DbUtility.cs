using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;
using Ivony.Data.Expressions;



namespace Ivony.Data
{
  public abstract class DbUtility
  {


    /// <summary>
    /// 由派生类实现，创建IDataAdapter对象
    /// </summary>
    /// <param name="selectCommand">查询命令</param>
    protected abstract IDataAdapter CreateDataAdapter( IDbCommand selectCommand );

    /// <summary>
    /// 由派生类实现，创建IDataParameter对象
    /// </summary>
    /// <param name="name">参数名</param>
    /// <param name="value">参数值</param>
    protected abstract IDataParameter CreateParameter( string name, object value );



    /// <summary>
    /// 获取DbExpressionParser实例，用于分析Sql表达式
    /// </summary>
    /// <returns>DbExpressionParser实例</returns>
    protected abstract IDbExpressionParser GetExpressionParser();

    /// <summary>
    /// 创建 DbCommand 对象
    /// </summary>
    /// <param name="expression">命令表达式</param>
    /// <returns>DbCommand 对象</returns>
    protected virtual IDbCommand CreateCommand( IDbExpression expression )
    {
      var parser = GetExpressionParser();
      return parser.Parse( expression );
    }



    /// <summary>
    /// 填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="template">查询字符串模板</param>
    /// <param name="parameters">查询字符串参数</param>
    /// <returns></returns>
    public DataTable Data( IDbExpression expression )
    {
      return Data( null, null, expression );
    }

    /// <summary>
    /// 填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="dataSet">需要被填充的数据集</param>
    /// <param name="tableName">将最后一个表设置为什么名字</param>
    /// <param name="expression">查询表达式</param>
    /// <returns></returns>
    public virtual DataTable Data( DataSet dataSet, string tableName, IDbExpression expression )
    {

      IDbCommand command = CreateCommand( expression );
      try
      {
        return Data( CreateDataAdapter( command ), dataSet, tableName );
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
    /// <param name="expression">查询表达式</param>
    /// <returns>影响的行数</returns>
    public virtual int NonQuery( IDbExpression expression )
    {
      IDbCommand command = CreateCommand( expression );
      try
      {
        return NonQuery( command );
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
    public virtual object Scalar( IDbExpression expression )
    {
      IDbCommand command = CreateCommand( expression );
      try
      {
        return Scalar( command );
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
    /// <returns></returns>
    public virtual DataRow FirstRow( IDbExpression expression )
    {

      IDbCommand command = CreateCommand( expression );

      try
      {
        return FirstRow( command );
      }
      catch ( DbException e )
      {
        OnError( this, e, command );
        throw;
      }
    }



    /// <summary>
    /// 协助填充DataSet，并将填充的最后一个表返回
    /// </summary>
    /// <param name="adapter">用来填充数据的适配器</param>
    /// <param name="dataSet">需要被填充的数据集</param>
    /// <param name="tableName">将最后一个表设置为什么名字</param>
    /// <returns></returns>
    public static DataTable Data( IDataAdapter adapter, DataSet dataSet, string tableName )
    {
      if ( dataSet == null )
        dataSet = new DataSet();

      adapter.Fill( dataSet );

      DataTable dataTable = dataSet.Tables[dataSet.Tables.Count - 1];


      if ( tableName != null )
        dataTable.TableName = tableName;

      return dataTable;
    }

    /// <summary>
    /// 协助执行无结果的查询
    /// </summary>
    /// <param name="command">查询命令</param>
    /// <returns>受影响的行数。</returns>
    public static int NonQuery( IDbCommand command )
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
    public static object Scalar( IDbCommand command )
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
    public static DataRow FirstRow( IDbCommand command )
    {
      if ( command.Connection == null )
        throw new InvalidOperationException( "数据库连接尚未初始化" );

      if ( command.Connection.State == ConnectionState.Closed )
      {
        try
        {
          command.Connection.Open();
          return ExecuteSingleRowPrivate( command );
        }
        finally
        {
          command.Connection.Close();
        }
      }
      else
      {
        return ExecuteSingleRowPrivate( command );
      }
    }

    private static DataRow ExecuteSingleRowPrivate( IDbCommand command )
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
    /// 当发生错误时
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