using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 数据表表达式
  /// </summary>
  public class TableExpression : IDbExpression
  {

    private string _tableName;

    /// <summary>
    /// 构建数据表表达式
    /// </summary>
    /// <param name="tableName">表名</param>
    public TableExpression( string tableName )
    {
      _tableName = tableName;
    }

    private string[] _fields = null;

    /// <summary>
    /// 设置要获取的字段列表
    /// </summary>
    /// <param name="fields">要获取的字段列表</param>
    /// <returns>返回数据表表达式自身</returns>
    public TableExpression Fields( params string[] fields )
    {
      _fields = fields;
      return this;
    }

    private TemplateExpression _where;

    /// <summary>
    /// 设置要筛选的条件
    /// </summary>
    /// <param name="whereClause">要设置的筛选条件模版</param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public TableExpression Where( string whereClause, params object[] parameters )
    {
      _where = Template( whereClause, parameters );
      return this;
    }



    private static TemplateExpression Template( string template, params object[] parameters )
    {
      return new TemplateExpression( template, parameters );
    }
  }

  public class SelectClause
  {
    public SelectClause AddField( string fieldExpression, string fieldName = null )
    {
      throw new NotImplementedException();
    }


  }




}
