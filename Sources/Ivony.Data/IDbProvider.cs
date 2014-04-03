using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{

  /// <summary>
  /// 定义 IDbExecutor 的提供程序
  /// </summary>
  public interface IDbProvider
  {

    /// <summary>
    /// 获取指定类型查询的查询器
    /// </summary>
    /// <typeparam name="T">指定类型的查询</typeparam>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>数据库查询器</returns>
    IDbExecutor<T> GetDbExecutor<T>( string connectionString ) where T : IDbQuery;

  }
}
