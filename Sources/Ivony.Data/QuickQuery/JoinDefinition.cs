using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{


  /// <summary>
  /// 表之间的连接运算定义
  /// </summary>
  public class JoinDefinition
  {


    public JoinType JoinType
    {
      get;
      private set;
    }


    public TableReference LeftTable
    {
      get;
      private set;
    }

    public TableReference RightTable
    {
      get;
      private set;
    }


  }

  public enum JoinType
  {
    InnerJoin,
    LeftOuterJoin,
    RightOuterJoin,
    FullOuterJoin,
    CrossJoin
  }
}
