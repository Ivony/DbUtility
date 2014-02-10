using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public static class Db
  {


    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="templateText">模板文本</param>
    /// <param name="paramaters">模板参数</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery Template( string templateText, object[] paramaters )
    {
      return TemplateParser.ParseTemplate( templateText, paramaters );
    }



    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="templateText">模板文本</param>
    /// <param name="paramaters">模板参数</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery T( string templateText, object[] paramaters )
    {
      return Template( templateText, paramaters );
    }


    /// <summary>
    /// 通过连接字符串设置，创建指定类型查询的执行器。
    /// </summary>
    /// <typeparam name="T">要执行的查询类型</typeparam>
    /// <param name="connectionStringName">连接字符串名</param>
    /// <returns>执行器</returns>
    public static IDbExecutor<T> CreateExecutor<T>( string connectionStringName ) where T : IDbQuery
    {
      var connectionStringSetting = ConfigurationManager.ConnectionStrings[connectionStringName];
      var provider = GetDbProvider( connectionStringSetting.ProviderName );

      if ( provider == null )
        return null;

      return provider.GetDbExecutor<T>( connectionStringSetting.ConnectionString );
    }

    private static IDbProvider GetDbProvider( string name )
    {
      return null;
    }
  }
}
