using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Collections;
using System.Configuration;
using System.Threading.Tasks;
using Ivony.Data.Queries;

namespace Ivony.Data
{
  /// <summary>
  /// 用于操作 SQL Server 的数据库访问工具
  /// </summary>
  [Serializable]
  public class SqlDbUtility : IAsyncDbExecutor<TemplateQuery>
  {



    protected string ConnectionString
    {
      get;
      private set;
    }

    public SqlDbUtility( string connectionString )
    {
      ConnectionString = connectionString;
    }


    /// <summary>
    /// 创建数据库访问工具
    /// </summary>
    /// <param name="name">连接字符串名称</param>
    /// <returns>数据库访问工具</returns>
    public static SqlDbUtility Create( string name )
    {
      var setting = ConfigurationManager.ConnectionStrings[name];
      if ( setting == null )
        throw new InvalidOperationException();

      return new SqlDbUtility( setting.ConnectionString );
    }





    IDataReader IDbExecutor<TemplateQuery>.ExecuteReader( TemplateQuery query )
    {
      throw new NotImplementedException();
    }

    Task<IDataReader> IAsyncDbExecutor<TemplateQuery>.ExecuteReaderAsync( TemplateQuery query )
    {
      throw new NotImplementedException();
    }
  }
}
