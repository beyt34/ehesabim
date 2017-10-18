using System.Collections.Generic;

namespace eHesabim.Core.Data {
    public interface IProcedureManager {
        string ExecuteProcedure(string procedureName, Dictionary<string, object> parameters);

        IList<T> ExecuteProcedure<T>(string procedureName, Dictionary<string, object> parameters);
    }
}
