using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 提供有关事务的一些扩展方法
  /// </summary>
  public static class TransactionExtensions
  {

    /// <summary>
    /// 创建并开启一个事务
    /// </summary>
    /// <typeparam name="TDbExecutor">数据库查询执行程序类型</typeparam>
    /// <param name="provider">数据库事务提供程序</param>
    /// <returns>数据库事务上下文</returns>
    public static IDbTransactionContext<TDbExecutor> BeginTransaction<TDbExecutor>( this IDbTransactionProvider<TDbExecutor> provider )
    {
      var transaction = provider.CreateTransaction();
      transaction.BeginTransaction();
      return transaction;
    }

  }
}
