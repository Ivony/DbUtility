using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.Test
{
  [TestClass]
  public class SqlServerTest
  {


    private SqlDbUtility db = new SqlDbUtility( "Data Source=(local);Initial Catalog=TestDatabase;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False" );


    [TestInitialize]
    public void Initialize()
    {
      db.Template( "TRUNCATE TABLE Test1;" ).ExecuteNonQuery();
    }


    [TestMethod]
    public void Test1()
    {
      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDataTable().Rows.Count, 0, "空数据表查询测试失败" );
      Assert.AreEqual( db.T( "INSERT INTO Test1 ( Name, Data, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery(), 1, "插入数据测试失败" );
      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );
    }
  }
}
