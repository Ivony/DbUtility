using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 定义事务管理器
  /// </summary>
  public interface ITransactionUtility : IDisposable
  {

    /// <summary>
    /// 开始事务
    /// </summary>
    void Begin();

    /// <summary>
    /// 提交事务
    /// </summary>
    void Commit();

    /// <summary>
    /// 回滚事务
    /// </summary>
    void Rollback();

    /// <summary>
    /// 获取帮助执行SQL语句的DbUtility实例。
    /// </summary>
    DbUtility DbUtility
    {
      get;
    }
  }


  public interface ITransactionUtility<T> : ITransactionUtility where T : DbUtility
  {
    new T DbUtility
    {
      get;
    }
  }


}
