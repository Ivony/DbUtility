using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Ivony.Data.Common;
using Ivony.Data.Queries;
using Ivony.Fluent;
using Npgsql;

namespace Ivony.Data.PostgreSQL.PostgreSqlClient
{
    public class NpgsqlDbExecutor : DbExecutorBase, IAsyncDbExecutor<ParameterizedQuery>, IAsyncDbExecutor<StoredProcedureQuery>, IDbTransactionProvider<NpgsqlDbExecutor>
    {
        protected string ConnectionString { get; private set; }
        protected NpgsqlDbConfiguration Configuration { get; private set; }

        /// <summary>
        /// 初始化 DbExecuterBase 类型
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="configuration">当前要使用的数据库配置</param>
        public NpgsqlDbExecutor(string connectionString, NpgsqlDbConfiguration configuration) : base(configuration)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.ConnectionString = connectionString;
            this.Configuration = configuration;
        }

        protected virtual IDbExecuteContext Execute(NpgsqlCommand command, IDbTracing tracing)
        {
            try
            {
                TryExecuteTracing(tracing, t => t.OnExecuting(command));

                var connection = new NpgsqlConnection(this.ConnectionString);
                connection.Open();
                command.Connection = connection;

                if ( Configuration.QueryExecutingTimeout.HasValue )
                  command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


                var reader = command.ExecuteReader();
                var context = new NpgsqlDbExecuteContext(connection, reader, tracing);

                TryExecuteTracing(tracing, t => t.OnLoadingData(context));

                return context;
            }
            catch (DbException exception)
            {
                TryExecuteTracing(tracing, t => t.OnException(exception));
                throw;
            }
        }

        /// <summary>
        /// 异步执行查询命令并返回执行上下文
        /// </summary>
        /// <param name="command">查询命令</param>
        /// <param name="token">取消指示</param>
        /// <param name="tracing">用于追踪查询过程的追踪器</param>
        /// <returns>查询执行上下文</returns>
        protected virtual async Task<IAsyncDbExecuteContext> ExecuteAsync(NpgsqlCommand command, CancellationToken token, IDbTracing tracing = null)
        {
            try
            {
                TryExecuteTracing(tracing, t => t.OnExecuting(command));

                var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync(token);
                command.Connection = connection;

                if ( Configuration.QueryExecutingTimeout.HasValue )
                  command.CommandTimeout = (int) Configuration.QueryExecutingTimeout.Value.TotalSeconds;


                var reader = await command.ExecuteReaderAsync(token);
                var context = new NpgsqlDbExecuteContext(connection, reader, tracing);

                TryExecuteTracing(tracing, t => t.OnLoadingData(context));

                return context;
            }
            catch (DbException exception)
            {
                TryExecuteTracing(tracing, t => t.OnException(exception));
                throw;
            }
        }

        protected NpgsqlCommand CreateCommand(ParameterizedQuery query)
        {
            return new NpgsqlParameterizedQueryParser().Parse(query);
        }

        /// <summary>
        /// 通过存储过程查询创建 SqlCommand 对象
        /// </summary>
        /// <param name="query">存储过程查询对象</param>
        /// <returns>SQL 查询命令对象</returns>
        protected NpgsqlCommand CreateCommand(StoredProcedureQuery query)
        {
            var command = new NpgsqlCommand(query.Name){
                CommandType = CommandType.StoredProcedure
            };
            query.Parameters.ForAll(pair => command.Parameters.AddWithValue(pair.Key, pair.Value));

            return command;
        }

        #region Implementation of IDbExecutor<in ParameterizedQuery>

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns>查询执行上下文</returns>
        public IDbExecuteContext Execute(ParameterizedQuery query)
        {
            return this.Execute(this.CreateCommand(query), this.TryCreateTracing(this, query));
        }

        #endregion

        #region Implementation of IAsyncDbExecutor<ParameterizedQuery>

        /// <summary>
        /// 异步执行查询
        /// </summary>
        /// <param name="query">要执行的查询</param>
        /// <param name="token">取消指示</param>
        /// <returns>查询执行上下文</returns>
        public Task<IAsyncDbExecuteContext> ExecuteAsync(ParameterizedQuery query, CancellationToken token)
        {
            return this.ExecuteAsync(this.CreateCommand(query), token, this.TryCreateTracing(this, query));
        }

        #endregion

        #region Implementation of IDbExecutor<in StoredProcedureQuery>

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns>查询执行上下文</returns>
        public IDbExecuteContext Execute(StoredProcedureQuery query)
        {
            return Execute(CreateCommand(query), TryCreateTracing(this, query));
        }

        #endregion

        #region Implementation of IAsyncDbExecutor<StoredProcedureQuery>

        /// <summary>
        /// 异步执行查询
        /// </summary>
        /// <param name="query">要执行的查询</param>
        /// <param name="token">取消指示</param>
        /// <returns>查询执行上下文</returns>
        public Task<IAsyncDbExecuteContext> ExecuteAsync(StoredProcedureQuery query, CancellationToken token)
        {
            return ExecuteAsync(CreateCommand(query), token, TryCreateTracing(this, query));
        }

        #endregion

        #region Implementation of IDbTransactionProvider<out NpgsqlDbExecutor>

        /// <summary>
        /// 创建一个数据库事务上下文
        /// </summary>
        /// <returns>数据库事务上下文</returns>
        public IDbTransactionContext<NpgsqlDbExecutor> CreateTransaction()
        {
            return new NpgsqlDbTransactionContext(this.ConnectionString, this.Configuration);
        }

        #endregion
    }

    internal class NpgsqlDbExecutorWithTransaction : NpgsqlDbExecutor
    {
        public NpgsqlDbExecutorWithTransaction(NpgsqlDbTransactionContext transaction, NpgsqlDbConfiguration configuration)
            : base(transaction.Connection.ConnectionString, configuration)
        {
            TransactionContext = transaction;
        }


        /// <summary>
        /// 当前所处的事务
        /// </summary>
        protected NpgsqlDbTransactionContext TransactionContext { get; private set; }


        /// <summary>
        /// 重写 ExecuteAsync 方法，在事务中异步执行查询
        /// </summary>
        /// <param name="command">要执行的查询命令</param>
        /// <param name="token">取消指示</param>
        /// <param name="tracing">用于追踪的追踪器</param>
        /// <returns>查询执行上下文</returns>
        protected sealed override async Task<IAsyncDbExecuteContext> ExecuteAsync(NpgsqlCommand command, CancellationToken token, IDbTracing tracing = null)
        {
            try
            {
                TryExecuteTracing(tracing, t => t.OnExecuting(command));

                command.Connection = TransactionContext.Connection;
                command.Transaction = TransactionContext.Transaction;

                var reader = await command.ExecuteReaderAsync(token);
                var context = new NpgsqlDbExecuteContext(TransactionContext, reader, tracing);

                TryExecuteTracing(tracing, t => t.OnLoadingData(context));

                return context;
            }
            catch (DbException exception)
            {
                TryExecuteTracing(tracing, t => t.OnException(exception));
                throw;
            }

        }


        /// <summary>
        /// 执行查询命令并返回执行上下文
        /// </summary>
        /// <param name="command">查询命令</param>
        /// <param name="tracing">用于追踪查询过程的追踪器</param>
        /// <returns>查询执行上下文</returns>
        protected sealed override IDbExecuteContext Execute(NpgsqlCommand command, IDbTracing tracing = null)
        {
            try
            {
                TryExecuteTracing(tracing, t => t.OnExecuting(command));

                command.Connection = TransactionContext.Connection;
                command.Transaction = TransactionContext.Transaction;

                var reader = command.ExecuteReader();
                var context = new NpgsqlDbExecuteContext(TransactionContext, reader, tracing);

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