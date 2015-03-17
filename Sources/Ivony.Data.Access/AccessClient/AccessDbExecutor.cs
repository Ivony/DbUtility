using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using Ivony.Data.Common;
using Ivony.Data.Queries;

namespace Ivony.Data.Access.AccessClient
{
    public class AccessDbExecutor : DbExecutorBase, IDbExecutor<ParameterizedQuery>
    {

        public AccessDbExecutor(string connectionString, AccessDbConfiguration configuration)
            : base(configuration)
        {
            if(string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if(configuration == null)
                throw new ArgumentNullException("configuration");

            Configuration = configuration;
            Connection = new OleDbConnection(connectionString);
        }


        public OleDbConnection Connection { get; private set; }


        protected AccessDbConfiguration Configuration { get; private set; }

        public object SyncRoot { get; private set; }

        public IDbExecuteContext Execute(ParameterizedQuery query)
        {
            var tracing = TryCreateTracing(this, query);
            var command = new AccessDbParameterizedQueryParser().Parse(query);
            return Execue(command, tracing);
        }

        private IDbExecuteContext Execue(OleDbCommand command, IDbTracing tracing)
        {
            try
            {
                TryExecuteTracing(tracing, t => t.OnExecuting(command));

                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();
                command.Connection = Connection;

                if (Configuration.QueryExecutingTimeout.HasValue)
                    command.CommandTimeout = (int)Configuration.QueryExecutingTimeout.Value.TotalSeconds;

                var context = new AccessDbExecuteContext(command.ExecuteReader(), tracing, SyncRoot);

                TryExecuteTracing(tracing, t => t.OnLoadingData(context));
                return context;

            }
            catch (DbException exception)
            {
                TryExecuteTracing(tracing, t => t.OnException(exception));
                throw;
            }
        }



    }
}
