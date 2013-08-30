using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  /// <summary>
  /// 用于辅助执行通过OleDb 协议操作 Excel 文件的数据访问帮助器
  /// </summary>
  public class ExcelDbUtility : DbUtility
  {


    protected override IDbCommand CreateCommand( IDbExpression expression )
    {
      throw new NotImplementedException();
    }

    
  }
}
