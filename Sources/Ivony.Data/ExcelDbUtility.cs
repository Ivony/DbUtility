using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public class ExcelDbUtility : DbUtility
  {
    protected override IDbExpressionParser GetExpressionParser()
    {
      throw new NotImplementedException();
    }
  }
}
