using System;


namespace Ivony.Data.Queries
{
  public interface IParameterizedQueryParser<TCommand>
  {
    /// <summary>
    /// 创建查询命令
    /// </summary>
    /// <param name="query">参数化查询</param>
    /// <returns>查询命令</returns>
    TCommand Parse( ParameterizedQuery query );
  }
}
