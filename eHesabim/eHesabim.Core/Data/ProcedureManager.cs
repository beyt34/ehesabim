using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

using eHesabim.Data;

namespace eHesabim.Core.Data {
    public class ProcedureManager : IProcedureManager {
        private readonly IDataContext dataContext;
        
        public ProcedureManager(IDataContext dataContext) {
            this.dataContext = dataContext;
        }

        public string ExecuteProcedure(string procedureName, Dictionary<string, object> parameters) {
            var sqlParameterCollection = new object[parameters.Count];

            var i = 0;
            foreach (var parameter in parameters) {
                sqlParameterCollection[i] = new SqlParameter(parameter.Key, parameter.Value);
                i++;
            }

            ((IObjectContextAdapter)dataContext).ObjectContext.CommandTimeout = 300;
            return dataContext.ExecuteStoredProcedure(procedureName, sqlParameterCollection);
        }

        public IList<T> ExecuteProcedure<T>(string procedureName, Dictionary<string, object> parameters) {
            var sqlParameterCollection = new object[parameters.Count];

            var i = 0;
            foreach (var parameter in parameters) {
                sqlParameterCollection[i] = new SqlParameter(parameter.Key, parameter.Value);
                i++;
            }

            ((IObjectContextAdapter)dataContext).ObjectContext.CommandTimeout = 300;
            return dataContext.ExecuteStoredProcedure<T>(procedureName, sqlParameterCollection).ToListNoLock();
        }
    }
}
