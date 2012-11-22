using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data
{
  public static class SqlServerHelper
  {

    public IPagingDataSource MakePageSource( TemplateExpression expression, SqlDbUtility dbUtility )
    {
      return new SqlServerPagingDataSource( expression, dbUtility );
    }

  }

  public class SqlServerPagingDataSource : IPagingDataSource<DataRowView>
  {
    private TemplateExpression _expression;
    private SqlDbUtility _dbUtility;

    public SqlServerPagingDataSource( TemplateExpression expression, SqlDbUtility dbUtility )
    {
      _expression = expression;
      _dbUtility = dbUtility;
    }

    public IPagingData CreatePaging( int pageSize )
    {
      return new Paging( _expression, _dbUtility, pageSize );
    }

    private class Paging : IPagingData<DataRowView>
    {
      private TemplateExpression _expression;
      private SqlDbUtility _dbUtility;

      public Paging( TemplateExpression expression, SqlDbUtility dbUtility, int pageSize )
      {
        _expression = expression;
        _dbUtility = dbUtility;
        PageSize = pageSize;
      }

      public int PageSize
      {
        get;
        private set;

      }


      public int Count()
      {
        return _dbUtility.Scalar<int>( "WITH( {0} ) AS DataSource SELECT COUNT(*) FROM DataSource" );
      }


      public IEnumerable<DataRowView> GetPage( int pageIndex )
      {
        var expression = DbExpressions.Template( "WITH( {0} ) AS DataSource SELECT * , ROW_NUMBER() OVER "
      }

      System.Collections.IEnumerable IPagingData.GetPage( int pageIndex )
      {
        return GetPage( pageIndex );
      }

    }


  }
}
