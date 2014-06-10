using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;
using Ivony.Data.SqlServer;
using System.Threading.Tasks;
using Ivony.Logs;

namespace Ivony.Data.Test
{
  [TestClass]
  public class SqlServerTest
  {


    private static readonly string connectionString = @"Data Source=(local)\SQLEXPRESS;Initial Catalog=TestDatabase;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

    private SqlDbUtility db = new SqlDbUtility( connectionString );


    [TestInitialize]
    public void Initialize()
    {
      db.T( "IF OBJECT_ID(N'[dbo].[Test1]') IS NOT NULL DROP TABLE [dbo].[Test1]" ).ExecuteNonQuery();
      db.T( @"
CREATE TABLE [dbo].[Test1]
(
    [ID] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(50) NOT NULL , 
    [Content] NTEXT NULL, 
    [Index] INT NOT NULL, 
    CONSTRAINT [PK_Test1] PRIMARY KEY ([ID]) 
)" ).ExecuteNonQuery();
    }


    [TestMethod]
    public void Test1()
    {
      Assert.AreEqual( db.T( "SELECT COUNT(*) FROM Test1" ).ExecuteScalar<int>(), 0, "空数据表查询测试失败" );
      Assert.AreEqual( db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery(), 1, "插入数据测试失败" );
      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );

    }


    [TestMethod]
    public void AsyncTest1()
    {
      var task = _AsyncTest1();
      task.Wait();
    }


    public async Task _AsyncTest1()
    {
      Assert.AreEqual( await db.T( "SELECT COUNT(*) FROM Test1" ).ExecuteScalarAsync<int>(), 0, "空数据表查询测试失败" );
      Assert.AreEqual( await db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQueryAsync(), 1, "插入数据测试失败" );
      Assert.AreEqual( (await db.T( "SELECT * FROM Test1" ).ExecuteDynamicsAsync()).Length, 1, "插入数据后查询测试失败" );

    }




    [TestMethod]
    public void TransactionTest()
    {

      using ( var transaction = db.BeginTransaction() )
      {
        Assert.AreEqual( transaction.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( transaction.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );
      }

      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 0, "自动回滚事务测试失败" );

      using ( var transaction = db.BeginTransaction() )
      {
        Assert.AreEqual( transaction.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( transaction.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );

        transaction.Rollback();
      }

      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 0, "手动回滚事务测试失败" );



      using ( var transaction = db.BeginTransaction() )
      {
        Assert.AreEqual( transaction.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( transaction.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );

        transaction.Commit();
      }

      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "手动提交事务测试失败" );

    }


    [TestMethod]
    public void TraceTest()
    {

      var service = new TestTraceService();
      var db = new SqlDbUtility( connectionString, service );

      db.T( "SELECT * FROM Test1" ).ExecuteDataTable();

      var tracing = service.FirstOrDefault();

      Assert.IsNotNull( tracing );
      var logs = tracing.GetLogEntries();
      Assert.AreEqual( logs.Length, 3 );

      Assert.AreEqual( logs[0].Message, "OnExecuting" );
      Assert.AreEqual( logs[1].Message, "OnLoadingData" );
      Assert.AreEqual( logs[2].Message, "OnComplete" );

    }

  }
}
