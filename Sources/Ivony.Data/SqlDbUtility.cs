using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections;

namespace Ivony.Data
{
  /// <summary>
  /// 用于操作 SQL Server 的数据库访问工具
  /// </summary>
  [Serializable]
  public class SqlDbUtility : DbUtility
  {

    string _connectionString;

    TransactionUtility _transaction;


    /// <summary>
    /// 创建 SqlDbUtility 对象
    /// </summary>
    /// <param name="connectionString">数据库连接字符串</param>
    public SqlDbUtility( string connectionString )
    {
      _connectionString = connectionString;
    }

    private SqlConnection CreateConnection()
    {
      if ( _transaction != null )
        throw new NotSupportedException( "在事务中执行时，禁止创建新连接" );

      //这里不能关闭原来的连接，因为它有可能正在被DataReader所使用。
      return InternalCreateConnection();
    }

    /// <summary>
    /// 创建数据库连接
    /// </summary>
    /// <returns>数据库连接</returns>
    protected virtual SqlConnection InternalCreateConnection()
    {
      return new SqlConnection( _connectionString );
    }

    /// <summary>
    /// 获取查询表达式解析器
    /// </summary>
    /// <returns>查询表达式解析器</returns>
    protected override IDbExpressionParser GetExpressionParser()
    {
      return new SqlServerExpressionParser( this );
    }



    internal SqlCommand CreateCommand()
    {
      if ( _transaction != null )
        return _transaction.CreateCommand();

      return CreateConnection().CreateCommand();
    }


    protected override IDataAdapter CreateDataAdapter( IDbCommand selectCommand )
    {
      return new SqlDataAdapter( (SqlCommand) selectCommand );
    }

    /// <summary>
    /// 创建一个事务
    /// </summary>
    /// <returns>事务管理器</returns>
    public override ITransactionUtility CreateTransaction()
    {
      return new TransactionUtility( this );
    }

    protected virtual SqlDbUtility InternalClone()
    {
      SqlDbUtility dbUtility = new SqlDbUtility( _connectionString );
      return dbUtility;

    }

    protected SqlDbUtility Clone()
    {
      return InternalClone();
    }


    private class TransactionUtility : ITransactionUtility<SqlDbUtility>
    {
      private SqlDbUtility _dbUtility;
      private SqlConnection _connection;
      private SqlTransaction _transaction;
      private bool _disposed = false;

      public TransactionUtility( SqlDbUtility origin )
      {
        _dbUtility = origin.Clone();
        _dbUtility._transaction = this;
      }

      /// <summary>
      /// 开始事务
      /// </summary>
      public void Begin()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
        {
          _connection = new SqlConnection( _dbUtility._connectionString );
          _connection.Open();
          _transaction = _connection.BeginTransaction();
        }
      }

      /// <summary>
      /// 提交事务
      /// </summary>
      public void Commit()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
          throw new InvalidOperationException();

        _transaction.Commit();
        _connection.Close();
        _disposed = true;
      }

      /// <summary>
      /// 回滚事务
      /// </summary>
      public void Rollback()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
          throw new InvalidOperationException();

        _transaction.Rollback();
        _connection.Close();
        _disposed = true;
      }

      /// <summary>
      /// 用于操作数据库的 DbUtility 对象
      /// </summary>
      public SqlDbUtility DbUtility
      {
        get { return _dbUtility; }
      }

      DbUtility ITransactionUtility.DbUtility
      {
        get { return DbUtility; }
      }


      /// <summary>
      /// 销毁事务对象
      /// </summary>
      public void Dispose()
      {

        if ( _connection != null )
          _connection.Dispose();

        if ( _transaction != null )
          _transaction.Dispose();

        _disposed = true;
      }


      /// <summary>
      /// 创建命令对象
      /// </summary>
      /// <returns></returns>
      internal SqlCommand CreateCommand()
      {
        if ( _disposed )
          throw new InvalidOperationException();

        if ( _transaction == null )
          Begin();


        SqlCommand command = _connection.CreateCommand();
        command.Transaction = _transaction;

        return command;
      }

    }

  }
}
