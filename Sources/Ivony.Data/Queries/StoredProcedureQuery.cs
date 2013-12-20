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
  public class StoredProcedureQuery : DbQuery
  {

    private string _name;
    private IDictionary<string, object> _parameters;

    /// <summary>
    /// 创建 StoredProcedureExpression 对象
    /// </summary>
    /// <param name="name">存储过程名称</param>
    public StoredProcedureQuery( IDbExecutor<StoredProcedureQuery> executor, string name ) : this( executor, name, new Dictionary<string, object>() ) { }

    /// <summary>
    /// 创建 StoredProcedureExpression 对象
    /// </summary>
    /// <param name="name">存储过程名称</param>
    /// <param name="parameters">存储过程参数列表</param>
    public StoredProcedureQuery( IDbExecutor<StoredProcedureQuery> executor, string name, IDictionary<string, object> parameters )
    {

      _name = name;
      _parameters = parameters;

      DbExecutor = executor;
    }



    public override IDbExecuteContext Execute()
    {
      return DbExecutor.Execute( this );
    }

    public override Task<IDbExecuteContext> ExecuteAsync()
    {
      var asyncExecutor = DbExecutor as IAsyncDbExecutor<StoredProcedureQuery>;
      if ( asyncExecutor != null )
        return asyncExecutor.ExecuteAsync( this );

      return new Task<IDbExecuteContext>( Execute );
    }




    protected IDbExecutor<StoredProcedureQuery> DbExecutor { get; private set; }


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
