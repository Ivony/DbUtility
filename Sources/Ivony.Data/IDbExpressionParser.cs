using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Data.Expressions;
using System.Data.SqlClient;

namespace Ivony.Data
{
  public interface IDbExpressionParser
  {

    IDbCommand Parse( IDbExpression expression );

  }




  public interface IDbExpression
  {

  }


}
