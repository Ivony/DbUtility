using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace Ivony.Data
{
  /// <summary>
  /// 为系统的 DataSet 和 DataTable 对象提供扩展方法
  /// </summary>
  public static class DataSetExtensions
  {

    /// <summary>
    /// 获取指定 DataTable 的默认视图
    /// </summary>
    /// <param name="table">要获取默认视图的 DataTable</param>
    /// <returns>默认数据视图</returns>
    public static IEnumerable<DataRowView> View( this DataTable table )
    {
      return table.DefaultView.Cast<DataRowView>();
    }


  }
}
