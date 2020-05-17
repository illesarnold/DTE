using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Threading.Tasks;
using [BusinessNameSpace].Domain;
using [BusinessNameSpace].DataProxy;
using MySql.Data.MySqlClient;

namespace [DataLayerNameSpace].Repository
{
    public abstract class BaseRepository<T> : I[ProjectName]DataProxy<T> where T : BaseDomain, new()
    {
        protected IDbConnection _dbConnection = null;
        public BaseRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        protected string _baseTableName = GetTableName(typeof(T));
        public bool SupportsTransactions => true;

        public bool IsLatencyProne => false;

        public void Delete(long id)
        {
            CreateConnection().Delete(new T() { ID = id });
        }

        public async Task DeleteAsync(long id)
        {
            await CreateConnection().DeleteAsync(new T() { ID = id });
        }

        public IEnumerable<T> GetAll()
        {
            return CreateConnection().GetAll<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await CreateConnection().GetAllAsync<T>();
        }

        public T GetByID(long id)
        {
            return CreateConnection().Get<T>(id);
        }

        public async Task<T> GetByIDAsync(long id)
        {
            return await CreateConnection().GetAsync<T>(id);
        }

        public T Insert(T entity)
        {
            var id = CreateConnection().Insert(entity);
            return CreateConnection().Get<T>(id);
        }

        public async Task<T> InsertAsync(T entity)
        {
            var id = await CreateConnection().InsertAsync(entity);
            return await CreateConnection().GetAsync<T>(id);
        }

        public T Update(T entity)
        {
            CreateConnection().Update(entity);
            return CreateConnection().Get<T>(entity.ID);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await CreateConnection().UpdateAsync(entity);
            return await CreateConnection().GetAsync<T>(entity.ID);
        }

        public IDbConnection CreateConnection()
        {
            if (_dbConnection is MySqlConnection)
                return new MySqlConnection(_dbConnection.ConnectionString);
            if (_dbConnection is MySqlConnection)
                return new SqlConnection(_dbConnection.ConnectionString);

            return null;
        }
        private static string GetTableName(Type obj)
        {
            var tAttribute = (TableAttribute)obj.GetCustomAttributes(typeof(TableAttribute), true)[0];

            return tAttribute.Name;
        }
    }
}
