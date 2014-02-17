using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义数据库事务查询上下文提供程序
  /// </summary>
  /// <typeparam name="TDbExecutor">数据库查询执行程序类型</typeparam>
  public interface IDbTransactionProvider<out TDbExecutor>
  {

    /// <summary>
    /// 创建一个数据库事务上下文
    /// </summary>
    /// <returns>数据库事务上下文</returns>
    IDbTransactionContext<TDbExecutor> CreateTransaction();

  }
}
