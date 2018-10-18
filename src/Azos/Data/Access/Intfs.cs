
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

using Azos.Conf;

namespace Azos.Data.Access
{
    /// <summary>
    /// Denotes types of CRUD stores
    /// </summary>
    public enum CRUDDataStoreType
    {
        Relational,
        Document,
        KeyValue,
        Hybrid
    }


    /// <summary>
    /// Describes an entity that performs single (not in transaction/batch)CRUD operations
    /// </summary>
    public interface ICRUDOperations
    {
        /// <summary>
        /// Returns true when backend supports true asynchronous operations, such as the ones that do not create extra threads/empty tasks
        /// </summary>
        bool SupportsTrueAsynchrony { get;}

        Schema GetSchema(Query query);
        Task<Schema> GetSchemaAsync(Query query);

        List<RowsetBase> Load(params Query[] queries);
        Task<List<RowsetBase>> LoadAsync(params Query[] queries);

        RowsetBase LoadOneRowset(Query query);
        Task<RowsetBase> LoadOneRowsetAsync(Query query);

        Doc        LoadOneDoc(Query query);
        Task<Doc>  LoadOneDocAsync(Query query);

        Cursor OpenCursor(Query query);
        Task<Cursor> OpenCursorAsync(Query query);

        int Save(params RowsetBase[] rowsets);
        Task<int> SaveAsync(params RowsetBase[] rowsets);

        int ExecuteWithoutFetch(params Query[] queries);
        Task<int> ExecuteWithoutFetchAsync(params Query[] queries);

        int Insert(Doc row, FieldFilterFunc filter = null);
        Task<int> InsertAsync(Doc row, FieldFilterFunc filter = null);

        int Upsert(Doc row, FieldFilterFunc filter = null);
        Task<int> UpsertAsync(Doc row, FieldFilterFunc filter = null);

        int Update(Doc row, IDataStoreKey key = null, FieldFilterFunc filter = null);
        Task<int> UpdateAsync(Doc row, IDataStoreKey key = null, FieldFilterFunc filter = null);

        int Delete(Doc row, IDataStoreKey key = null);
        Task<int> DeleteAsync(Doc row, IDataStoreKey key = null);
    }

    /// <summary>
    /// Describes an entity that performs single (not in transaction/batch)CRUD operations
    /// </summary>
    public interface ICRUDTransactionOperations
    {
       /// <summary>
        /// Returns true when backend supports transactions. Even if false returned, CRUDDatastore supports CRUDTransaction return from BeginTransaction()
        ///  in which case statements may not be sent to destination until a call to Commit()
        /// </summary>
        bool SupportsTransactions { get;}

       /// <summary>
       /// Returns a transaction object for backend. Even if backend does not support transactions internally, CRUDTransactions save changes
       ///  into the store on commit only
       /// </summary>
       CRUDTransaction BeginTransaction(IsolationLevel iso = IsolationLevel.ReadCommitted, TransactionDisposeBehavior behavior = TransactionDisposeBehavior.CommitOnDispose);

       /// <summary>
       /// Returns a transaction object for backend. Even if backend does not support transactions internally, CRUDTransactions save changes
       ///  into the store on commit only
       /// </summary>
       Task<CRUDTransaction> BeginTransactionAsync(IsolationLevel iso = IsolationLevel.ReadCommitted, TransactionDisposeBehavior behavior = TransactionDisposeBehavior.CommitOnDispose);
    }


    /// <summary>
    /// Represents a DataStore that supports CRUD operations
    /// </summary>
    public interface ICRUDDataStore : ICRUDOperations, ICRUDTransactionOperations
    {
        /// <summary>
        /// Returns default script file suffix, which some providers may use to locate script files
        ///  i.e. for MySql:  ".my.sql" which gets added to script files like so:  name.[suffix].[script ext (i.e. sql)].
        /// This name should uniquely identify the provider
        /// </summary>
        string ScriptFileSuffix { get; }

        /// <summary>
        /// Provides classification for the underlying store
        /// </summary>
        CRUDDataStoreType StoreType { get;}

        /// <summary>
        /// Reolver that turns query into handler
        /// </summary>
        ICRUDQueryResolver QueryResolver { get; }
    }


    public interface ICRUDDataStoreImplementation : ICRUDDataStore, IDataStoreImplementation
    {
        CRUDQueryHandler MakeScriptQueryHandler(QuerySource querySource);
    }

    /// <summary>
    /// Represents a class that resolves Query into suitable handler that can execute it
    /// </summary>
    public interface ICRUDQueryResolver : IConfigurable
    {
        /// <summary>
        /// Retrieves a handler for supplied query. The implementation must be thread-safe
        /// </summary>
        CRUDQueryHandler Resolve(Query query);

        string ScriptAssembly { get; set; }

        IList<string> HandlerLocations { get; }

        Collections.IRegistry<CRUDQueryHandler> Handlers { get; }

        /// <summary>
        /// Registers handler location.
        /// The Resolver must be not started yet. This method is NOT thread safe
        /// </summary>
        void RegisterHandlerLocation(string location);

        /// <summary>
        /// Unregisters handler location returning true if it was found and removed.
        /// The Resolve must be not started yet. This method is NOT thread safe
        /// </summary>
        bool UnregisterHandlerLocation(string location);

    }

    /// <summary>
    /// Represents a context (such as Sql Server connection + transaction scope, or Hadoop connect string etc.) for query execution.
    /// This is a marker interface implemented by particular providers
    /// </summary>
    public interface ICRUDQueryExecutionContext
    {

    }

}