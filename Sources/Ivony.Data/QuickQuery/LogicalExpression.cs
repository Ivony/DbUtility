using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery
{
  public class LogicalExpression : BooleanExpression
  {

    public LogicalOperator Operator
    {
      get;
      private set;
    }

    public BooleanExpression LeftExpression
    {
      get;
      private set;
    }

    public BooleanExpression RightExpression
    {
      get;
      private set;
    }

  }


  public enum LogicalOperator
  {
    And,
    Or
  }

}
