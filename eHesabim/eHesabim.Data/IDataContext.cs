using System.Collections.Generic;
using System.Data.Entity;

namespace eHesabim.Data {
    public interface IDataContext {
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges();

        void ModifyContext<T>(T itemToModify) where T : class;

        void SetAutoDetectChanges(bool enabled);

        string ExecuteStoredProcedure(string procedureName, params object[] sqlParameters);

        IEnumerable<T> ExecuteStoredProcedure<T>(string procedureName, params object[] sqlParameters);
    }
}
