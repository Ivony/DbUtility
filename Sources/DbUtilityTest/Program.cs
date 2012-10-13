using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ivony.Data;

namespace DbUtilityTest
{
  public class Program
  {
    static void Main( string[] args )
    {

      var dbUtility = new SqlDbUtility( "Data Source=113.98.255.70,130;Initial Catalog=TEST_FOCALPRICE_DB;Persist Security Info=True;User ID=focal_db;Password=focal_db?;Pooling=True" );

      var result = dbUtility.Entities<P>( "SELECT ProductName, UnitPrice FROM Products WHERE SKU LIKE 'EP%'" );

    }

    public class P
    {
      [FieldName("ProductName")]
      public string Name { get; set; }
      [NonField]
      public decimal UnitPrice { get; set; }
    }
  }
}
