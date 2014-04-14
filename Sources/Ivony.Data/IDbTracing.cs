using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{

  /// <summary>
  /// 定义对数据库查询进行追踪的记录器
  /// </summary>
  public interface IDbTracing
  {

    void OnQueryExecuting( IDbQuery query );

    void OnQueryExecuted( IDbQuery query );



    void OnResultConstructing( IDbQuery query );

    void OnResultConstructed( IDbQuery query );


  }

}
