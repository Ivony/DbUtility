using Ivony.Data.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.MySqlClient
{
  public class MySqlTransactionContext : IDbTransactionContext<MySqlDbUtility>
  {



    public MySqlTransaction Transaction
    {
      get;
      private set;
    }

    public MySqlConnection Connection
    {
      get;
      private set;
    }



    internal MySqlTransactionContext( string connectionString, MySqlDbConfiguration configuration )
    {

      Connection = new MySqlConnection( connectionString );
      DbExecutor = new MySqlDbUtilityWithTransaction( this, configuration );
    }


    public void BeginTransaction()
    {
      Connection.Open();
      Transaction = Connection.BeginTransaction();

    }



    public void Commit()
    {
      Transaction.Commit();
    }

    public void Rollback()
    {
      Transaction.Rollback();
    }

    public MySqlDbUtility DbExecutor
    {
      get;
      private set;
    }

    public object SyncRoot
    {
      get { throw new NotImplementedException(); }
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }


    private class MySqlDbUtilityWithTransaction : MySqlDbUtility
    {

      public MySqlDbUtilityWithTransaction( MySqlTransactionContext transaction, MySqlDbConfiguration configuration )
        : base( transaction.Connection.ConnectionString, configuration )
      {
        _transaction = transaction;
      }


      private MySqlTransactionContext _transaction;


      protected override IDbExecuteContext Execute( MySqlCommand command )
      {
        command.Connection = _transaction.Connection;
        var dataReader = command.ExecuteReader();

        return new MySqlExecuteContext( _transaction, dataReader );
      }

    }

  }
}
