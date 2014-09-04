using Ivony.Data;
using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest
{
  class Program
  {


    static void Main( string[] args )
    {

      var db = SqlServerExpress.Connect( "TestDatabase" );

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


      Test( db );

    }

    private static void Test( SqlDbExecutor db )
    {


      Stopwatch watch = new Stopwatch();

      watch.Start();

      for ( int i = 0; i < 10000; i++ )
        db.T( "INSERT INTO Test1 ( Name, Content, XmlContent, [Index] ) VALUES ( {...} )", "Ivony", "Test", null, i ).ExecuteNonQuery();

      db.T( "SELECT * FROM Test1" ).ExecuteDataTable();

      watch.Stop();

      Console.WriteLine( watch.Elapsed );


    }

  }
}
