using Ivony.Data;
using Ivony.Data.SqlClient;
using System;
using System.Collections.Generic;
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

    private static void Test( SqlDbHandler db )
    {
      db.T( "SELECT ID FROM Test1" ).ExecuteScalar();
      db.T( "SELECT ID FROM Test1" ).ExecuteFirstRow();
      db.T( "SELECT COUNT(*) FROM Test1" ).ExecuteScalar<int>();
      db.T( "INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1 ).ExecuteNonQuery();
      db.T( "SELECT * FROM Test1" ).ExecuteDynamics();
      db.T( "SELECT ID FROM Test1" ).ExecuteFirstRow();
      db.T( "SELECT * FROM Test1" ).ExecuteDynamicObject();
    }

  }
}
