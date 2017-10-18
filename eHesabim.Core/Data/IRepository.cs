using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace eHesabim.Core.Data {
    public interface IRepository<T, TId> : IReadOnlyRepository<T, TId> {
        TId AddUpdate(T entity);

        void InsertBulkData(IEnumerable<T> items);

        void Delete(TId id);

        IQueryable<T> Filter(Expression<Func<T, bool>> filter, int index, int size, string sort, bool sortDescending, out int total);
    }
}
