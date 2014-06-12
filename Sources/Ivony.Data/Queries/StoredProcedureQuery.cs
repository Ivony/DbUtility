using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{
  /// <summary>
  /// 存储过程表达式
  /// </summary>
  public class StoredProcedureQuery : IDbQuery
  {

    private string _name;
    private IDictionary<string, object> _parameters;

    /// <summary>
    /// 创建 StoredProcedureExpression 对象
    /// </summary>
    /// <param name="name">存储过程名称</param>
    public StoredProcedureQuery( string name ) : this( name, new Dictionary<string, object>() ) { }

    /// <summary>
    /// 创建 StoredProcedureExpression 对象
    /// </summary>
    /// <param name="name">存储过程名称</param>
    /// <param name="parameters">存储过程参数列表</param>
    public StoredProcedureQuery( string name, IDictionary<string, object> parameters )
    {

      _name = name;
      _parameters = parameters;

    }


    /// <summary>
    /// 存储过程名称
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// 存储过程参数列表
    /// </summary>
    public IDictionary<string, object> Parameters
    {
      get { return _parameters; }
    }
  }
}
