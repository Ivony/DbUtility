using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;
using System.Threading.Tasks;
using Ivony.Logs;
using System.Data;
using Ivony.Data.SqlClient;
using System.Xml.Linq;
using Ivony.Data.Common;

namespace Ivony.Data.Test
{
  [TestClass]
  public class SqlServerTest
  {


    private TestTraceService traceService;
    private SqlDbHandler db;


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
    [XmlContent] XML NULL,
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


    [TestMethod]
    public void XmlFieldTest()
    {

      DbValueConverter.Register( new XDocumentValueConverter() );

      var document = new XDocument( new XDeclaration( "1.0", "utf-8", "yes" ),
        new XElement( "Root",
          new XAttribute( "test", "test-value" ),
          new XElement( "Item" ),
          new XElement( "Item" ),
          new XElement( "Item" )
        ) );

      db.T( "INSERT INTO Test1 ( Name, XmlContent, [Index] ) VALUES ( {...} ) ", "XML content", document, 1 ).ExecuteNonQuery();

      var document1 = db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<XDocument>();

      Assert.AreEqual( document.ToString( SaveOptions.OmitDuplicateNamespaces ), document1.ToString( SaveOptions.OmitDuplicateNamespaces ) );

      db.T( "UPDATE Test1 SET XmlContent = {0} ", null ).ExecuteNonQuery();
      Assert.IsNull( db.T( "SELECT XmlContent FROM Test1 " ).ExecuteScalar<XDocument>() );


      DbValueConverter.Unregister<XDocument>();

    }


    [TestMethod]
    public void NullableConvertTest()
    {
      db.T( "SELECT [Index] FROM Test1" ).ExecuteScalar<int?>();
    }

    [TestMethod]
    public void ConvertExceptionTest()
    {
      db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES( {...} )", "Test", "TestContent", 1 ).ExecuteNonQuery();

      Exception exception = null;
      try
      {
        db.T( "SELECT [Index] FROM Test1" ).ExecuteEntity<WrongEntity>();
      }
      catch ( InvalidCastException e )
      {
        exception = e;
      }

      Assert.IsNotNull( exception, "转换异常测试失败" );
      Assert.IsNotNull( exception.Data["DataColumnName"], "转换异常测试失败" );


    }


    [TestMethod]
    public void EntityTest()
    {
      db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES( {...} )", "Test", "TestContent", 1 ).ExecuteNonQuery();
      db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES( {...} )", "Test", "TestContent", 2 ).ExecuteNonQuery();

      db.T( "SELECT Name, Content, [Index] FROM Test1" ).ExecuteEntity<CorrectEntity>();
      db.T( "SELECT Name, Content, [Index] FROM Test1" ).ExecuteEntities<CorrectEntity>();

      var entity = (CorrectEntity) db.T( "SELECT Name, Content, [Index] FROM Test1" ).ExecuteDynamicObject();
      var entities = db.T( "SELECT Name, Content, [Index] FROM Test1" ).ExecuteDynamics().Select( item => (CorrectEntity) item ).ToArray();

    }


    [TestMethod]
    public void ConvertibleTest()
    {

      db = db.WithTraceService( null )
             .WithCommandTimeout( new TimeSpan( 0, 0, 30 ) );

        

      db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "5", "0.9", 100 ).ExecuteNonQuery();

      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<long>(), 100L );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<int>(), 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<short>(), (short) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<byte>(), (byte) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ulong>(), (ulong) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<uint>(), 100u );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ushort>(), (ushort) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<sbyte>(), (sbyte) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<char>(), (char) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<decimal>(), (decimal) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<double>(), (double) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<float>(), (float) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<string>(), "100" );

      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<long?>(), 100L );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<int?>(), 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<short?>(), (short) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<byte?>(), (byte) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ulong?>(), (ulong) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<uint?>(), 100u );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ushort?>(), (ushort) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<sbyte?>(), (sbyte) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<char?>(), (char) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<decimal?>(), (decimal) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<double?>(), (double) 100 );
      Assert.AreEqual( db.T( "SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<float?>(), (float) 100 );

      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<long>(), 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<int>(), 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<short>(), (short) 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<byte>(), (byte) 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<ulong>(), (ulong) 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<uint>(), 5u );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<ushort>(), (ushort) 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<sbyte>(), (sbyte) 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<decimal>(), (decimal) 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<double>(), (double) 5 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<float>(), (float) 5 );


      Assert.AreEqual( db.T( "SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<decimal>(), (decimal) 0.9m );
      Assert.AreEqual( db.T( "SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<double>(), (double) 0.9 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<float>(), (float) 0.9 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<decimal?>(), (decimal) 0.9m );
      Assert.AreEqual( db.T( "SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<double?>(), (double) 0.9 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<float?>(), (float) 0.9 );
      Assert.AreEqual( db.T( "SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<string>(), "0.9" );


      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<long?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<int?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<short?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<byte?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ulong?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<uint?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ushort?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<sbyte?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<char?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<decimal?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<double?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<float?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<string>(), null );


      db.T( "DELETE Test1" ).ExecuteNonQuery();

      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<long?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<int?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<short?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<byte?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ulong?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<uint?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ushort?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<sbyte?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<char?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<decimal?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<double?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<float?>(), null );
      Assert.AreEqual( db.T( "SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<string>(), null );
    }



    [TestMethod]
    public void EnumTest()
    {

      db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", 
        TestEnum.One.ToString(), TestEnum.Two.ToString(), TestEnum.Three ).ExecuteNonQuery();
      
      
      Assert.AreEqual( db.T( "SELECT Name FROM Test1" ).ExecuteScalar<string>(), "One" );
      Assert.AreEqual( db.T( "SELECT [Index] FROM Test1" ).ExecuteScalar<int>(), 3 );
      Assert.AreEqual( db.T( "SELECT Name FROM Test1" ).ExecuteScalar<TestEnum>(), TestEnum.One );
      Assert.AreEqual( db.T( "SELECT [Index] FROM Test1" ).ExecuteScalar<TestEnum>(), TestEnum.Three );


    }


    enum TestEnum : long
    {
      One = 1,
      Two,
      Three,
    }






    public class WrongEntity
    {
      public string Name { get; set; }
      public string Content { get; set; }
      public TimeSpan Index { get; set; }
    }


    public class CorrectEntity
    {
      public string Name { get; set; }
      public string Content { get; set; }

      [FieldName( "Index" )]
      public long OrderIndex { get; set; }

      [NonField]
      public object NonDataField { get; set; }
    }

  }
}
