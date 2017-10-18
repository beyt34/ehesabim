using System;
using System.Linq;
using System.Linq.Expressions;

namespace eHesabim.Core.Data {
    public interface IReadOnlyRepository<T, in TId> {
        IQueryable<T> Table { get; }

        T Detail(TId id);

        IQueryable<T> Query();

        IQueryable<T> Query(Expression<Func<T, bool>> predicate);
    }
}
