using Ivony.Data.SQLiteClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Test
{

  [TestClass]
  public class SQLiteTest
  {

    SQLiteExecutor db;

    public SQLiteTest()
    {
      db = SQLite.ConnectFile( @"C:\Temp\1.db" );
      db.T( "DROP TABLE IF EXISTS Test1" ).ExecuteNonQuery();
      db.T( @"
CREATE TABLE [Test1]
(
    [Name] NVARCHAR(50) NOT NULL , 
    [Content] NTEXT NULL, 
    [Index] INT NOT NULL
)" ).ExecuteNonQuery();
    }


    [TestMethod]
    public void StandardTest1()
    {
      Assert.IsNull( db.T( "SELECT Name FROM Test1" ).ExecuteScalar(), "空数据表查询测试失败" );
      Assert.IsNull( db.T( "SELECT Name FROM Test1" ).ExecuteFirstRow(), "空数据表查询测试失败" );
      Assert.AreEqual( db.T( "SELECT COUNT(*) FROM Test1" ).ExecuteScalar<int>(), 0, "空数据表查询测试失败" );
      Assert.AreEqual( db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery(), 1, "插入数据测试失败" );
      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );
      Assert.IsNotNull( db.T( "SELECT Name FROM Test1" ).ExecuteFirstRow(), "插入数据后查询测试失败" );

      var dataItem = db.T( "SELECT * FROM Test1" ).ExecuteDynamicObject();
      Assert.AreEqual( dataItem.Name, "Ivony", "插入数据后查询测试失败" );
      Assert.AreEqual( dataItem["Content"], "Test", "插入数据后查询测试失败" );
    }

    [TestCleanup]
    public void Shutdown()
    {
      System.Data.SQLite.SQLiteConnection.Shutdown( true, false );
      SQLiteConnection.ClearAllPools();
    }

  }
}
