using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Access;
using Ivony.Data.Access.AccessClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ivony.Data.Test
{
    [TestClass]
    public class AccessTest
    {
        AccessDbExecutor _accessDbExecutor;

        public AccessTest()
        {
            _accessDbExecutor = AccessDb.ConnectFile(@"accessTestDb.mdb");
            _accessDbExecutor.T(@"
CREATE TABLE [Test1]
(
    [Name] NVARCHAR(50) NOT NULL , 
    [Content] NTEXT NULL, 
    [Index] INT NOT NULL
)").ExecuteNonQuery();
        }


        [TestMethod]
        public void StandTest_For_Access()
        {
            Assert.IsNull(_accessDbExecutor.T("SELECT Name FROM Test1").ExecuteScalar(), "空数据表查询测试失败");
            Assert.IsNull(_accessDbExecutor.T("SELECT Name FROM Test1").ExecuteFirstRow(), "空数据表查询测试失败");
            Assert.AreEqual(_accessDbExecutor.T("SELECT COUNT(*) FROM Test1").ExecuteScalar<long>(), 0, "空数据表查询测试失败");
            Assert.AreEqual(_accessDbExecutor.T("INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {...} )", "Ivony", "Test", 1).ExecuteNonQuery(), 1, "插入数据测试失败");
            Assert.AreEqual(_accessDbExecutor.T("SELECT * FROM Test1").ExecuteDynamics().Length, 1, "插入数据后查询测试失败");
            Assert.IsNotNull(_accessDbExecutor.T("SELECT Name FROM Test1").ExecuteFirstRow(), "插入数据后查询测试失败");

            var dataItem = _accessDbExecutor.T("SELECT * FROM Test1").ExecuteDynamicObject();
            Assert.AreEqual(dataItem.Name, "Ivony", "插入数据后查询测试失败");
            Assert.AreEqual(dataItem["Content"], "Test", "插入数据后查询测试失败");
        }

        [TestCleanup]
        public void Shutdown()
        {
            _accessDbExecutor.Connection.Close();
            OleDbConnection.ReleaseObjectPool();
        }
    }
}
