using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public static class TransactionExtensions
  {

    /// <summary>
    /// 开启新事务
    /// </summary>
    /// <returns>事务管理器</returns>
    public static ITransactionUtility BeginTransaction( this DbUtility dbUtility )
    {
      if ( dbUtility == null )
        throw new ArgumentNullException( "dbUtility" );

      var transaction = dbUtility.CreateTransaction();
      transaction.Begin();

      return transaction;
    }


  }
}
