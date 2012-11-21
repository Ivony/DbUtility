using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ivony.Data.LegacyAPI
{
  /// <summary>
  /// 提供向下兼容的API
  /// </summary>
  public static class LegacyExtensions
  {

    public static DataTable ExecuteData( this DbUtility dbUtility, string template, params object[] args )
    {
      return dbUtility.Data( template, args );
    }

    public static object ExecuteScalar( this DbUtility dbUtility, string template, params object[] args )
    {
      return dbUtility.Scalar( template, args );
    }

    public static int ExecuteNonQuery( this DbUtility dbUtility, string template, params object[] args )
    {
      return dbUtility.NonQuery( template, args );
    }

    public static DataRow ExecuteSingleRow( this DbUtility dbUtility, string template, params object[] args )
    {
      return dbUtility.FirstRow( template, args );
    }

    public static T[] ExecuteSingleColumn<T>( this DbUtility dbUtility, string template, params object[] args )
    {
      return dbUtility.Data( template, args ).Column<T>();
    }


  }
}
