using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;
using System.Threading.Tasks;
using Ivony.Logs;
using System.Data;
using Ivony.Data.SqlClient;

namespace Ivony.Data.Test
{
  [TestClass]
  public class SqlServerTest
  {


    private TestTraceService traceService;
    private SqlDbExecutor db;


    public SqlServerTest()
    {
      SqlServerExpress.Configuration.TraceService = traceService = new TestTraceService();
      db = SqlServerExpress.Connect( "TestDatabase" );


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


    [TestInitialize]
    public void Initialize()
    {

      db.T( "TRUNCATE TABLE Test1" ).ExecuteNonQuery();

    }


    [TestMethod]
    public void StandardTest1()
    {
      Assert.IsNull( db.T( "SELECT ID FROM Test1" ).ExecuteScalar(), "空数据表查询测试失败" );
      Assert.IsNull( db.T( "SELECT ID FROM Test1" ).ExecuteFirstRow(), "空数据表查询测试失败" );
      Assert.AreEqual( db.T( "SELECT COUNT(*) FROM Test1" ).ExecuteScalar<int>(), 0, "空数据表查询测试失败" );
      Assert.AreEqual( db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery(), 1, "插入数据测试失败" );
      Assert.AreEqual( db.T( "SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );
      Assert.IsNotNull( db.T( "SELECT ID FROM Test1" ).ExecuteFirstRow(), "插入数据后查询测试失败" );

      var dataItem = db.T( "SELECT * FROM Test1" ).ExecuteDynamicObject();
      Assert.AreEqual( dataItem.Name, "Ivony", "插入数据后查询测试失败" );
      Assert.AreEqual( dataItem["Content"], "Test", "插入数据后查询测试失败" );
    }


    [TestMethod]
    public void AsyncTest1()
    {
      var task = _AsyncTest1();
      task.Wait();
    }


    public async Task _AsyncTest1()
    {
      Assert.IsNull( await db.T( "SELECT ID FROM Test1" ).ExecuteScalarAsync(), "空数据表查询测试失败" );
      Assert.IsNull( await db.T( "SELECT ID FROM Test1" ).ExecuteFirstRowAsync(), "空数据表查询测试失败" );
      Assert.AreEqual( await db.T( "SELECT COUNT(*) FROM Test1" ).ExecuteScalarAsync<int>(), 0, "空数据表查询测试失败" );
      Assert.AreEqual( await db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQueryAsync(), 1, "插入数据测试失败" );
      Assert.AreEqual( (await db.T( "SELECT * FROM Test1" ).ExecuteDynamicsAsync()).Length, 1, "插入数据后查询测试失败" );
      Assert.IsNotNull( await db.T( "SELECT ID FROM Test1" ).ExecuteFirstRowAsync(), "插入数据后查询测试失败" );

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



      {
        Exception exception = null;
        var transaction = (SqlDbTransactionContext) db.BeginTransaction();

        try
        {
          using ( transaction )
          {
            transaction.T( "SELECT * FROM Nothing" ).ExecuteNonQuery();
            transaction.Commit();
          }
        }
        catch ( Exception e )
        {
          exception = e;
        }

        Assert.IsNotNull( exception, "事务中出现异常测试失败" );
        Assert.AreEqual( transaction.Connection.State, ConnectionState.Closed );
      }
    }


    [TestMethod]
    public void TraceTest()
    {

      db.T( "SELECT * FROM Test1" ).ExecuteDataTable();

      var tracing = traceService.Last();

      var events = tracing.TraceEvents;
      Assert.AreEqual( events.Length, 3 );

      Assert.IsTrue( tracing.QueryTime >= tracing.ExecutionTime );

      Assert.AreEqual( events[0].EventName, "OnExecuting" );
      Assert.AreEqual( events[1].EventName, "OnLoadingData" );
      Assert.AreEqual( events[2].EventName, "OnComplete" );


      try
      {
        db.T( "SELECT * FROM Nothing" ).ExecuteDynamics();
      }
      catch
      {

      }

      tracing = traceService.Last();

      events = tracing.TraceEvents;
      Assert.AreEqual( events.Length, 2 );

      Assert.AreEqual( events[0].EventName, "OnExecuting" );
      Assert.AreEqual( events[1].EventName, "OnException" );
    }
  }
}
