using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Ivony.Data.Expressions;

namespace Ivony.Data
{
  public class ExcelDbUtility : DbUtility
  {

    /// <summary>
    /// 获取数据库表达式解析器
    /// </summary>
    /// <returns>数据库表达式解析器</returns>
    protected override IDbExpressionParser GetExpressionParser()
    {
      return new ExcelExpressionParser( this );
    }

    private class ExcelExpressionParser : IDbExpressionParser
    {

      public ExcelDbUtility _dbUtility;

      public ExcelExpressionParser( ExcelDbUtility dbUtility )
      {
        _dbUtility = dbUtility;
      }


      public IDbCommand Parse( IDbExpression expression )
      {
        var templateExpression = expression as TemplateExpression;
        if ( templateExpression != null )
          return ParseTemplate( templateExpression );

        throw new NotSupportedException();
      }

      private IDbCommand ParseTemplate( TemplateExpression templateExpression )
      {
        var context = new ExcelTemplateParseContext();
        var commandText = templateExpression.Parse( context );

        throw new NotImplementedException();
      }


      private class ExcelTemplateParseContext : TemplateParseContext
      {
        public override string CreateParameterExpression( ParameterExpression parameter )
        {
          throw new NotImplementedException();
        }
      }


    }


  }
}
