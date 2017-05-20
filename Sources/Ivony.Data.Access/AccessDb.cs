using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ivony.Data.Access.AccessClient;
namespace Ivony.Data.Access
{
    public static class AccessDb
    {
        static AccessDb()
        {
            DefaultConfiguration = new AccessDbConfiguration();
        }

        public static AccessDbConfiguration DefaultConfiguration { get; private set; }

        public static AccessDbExecutor ConnectFile(string filePath, bool create = true, AccessDbConfiguration configuration = null)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            if (!File.Exists(filePath))
            {
                if (create)
                    CreateAccessDb(filePath);
                else
                {
                    throw new FileNotFoundException("The database file can't exist.");
                }
            }
            var builder = new OleDbConnectionStringBuilder
            {
                DataSource = filePath,
                Provider = "Microsoft.JET.OLEDB.4.0",
                PersistSecurityInfo = false,
            };

            return Connect(builder.ConnectionString, configuration ?? DefaultConfiguration);
        }

        public static AccessDbExecutor Connect(string connectionString, AccessDbConfiguration configuration = null)
        {
            return new AccessDbExecutor(connectionString, configuration);
        }

        public static bool CreateAccessDb(string filePath, bool overwrite = false)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(filePath);

            if (File.Exists(filePath))
            {
                if(overwrite)
                    File.Delete(filePath);
                else
                {
                    throw new InvalidOperationException("File Already existed.");
                }
            }

            try
            {

                //Reference:http://www.cnblogs.com/DasonKwok/archive/2012/08/02/2620194.html
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();

                var stream = assembly.GetManifestResourceStream("Ivony.Data.Access.EmbedResource.emptyDb.mdb");
                if (stream != null)
                {
                    var dataLength = stream.Length;
                    var buffer = new Byte[dataLength];
                    stream.Read(buffer, 0, (int)dataLength);
                    using (var writer = new FileStream(filePath, FileMode.Create))
                    {
                        writer.Write(buffer, 0, (int)dataLength);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
