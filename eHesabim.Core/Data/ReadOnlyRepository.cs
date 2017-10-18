using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using eHesabim.Data;

namespace eHesabim.Core.Data {
    public class ReadOnlyRepository<T, TId> : IReadOnlyRepository<T, TId> where T : ReadOnlyEntity<TId> {
        internal readonly IDataContext DataContext;

        private IDbSet<T> entities;

        public ReadOnlyRepository(IDataContext context) {
            DataContext = context;
        }

        public virtual IQueryable<T> Table {
            get {
                return Entities;
            }
        }

        internal IDbSet<T> Entities {
            get {
                return entities ?? (entities = DataContext.Set<T>());
            }
        }

        public T Detail(TId id) {
            return Query(a => (object)a.Id == (object)id).FirstOrDefault();
        }

        public virtual IQueryable<T> Query() {
            return Table.AsQueryable();
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate) {
            return Table.Where(predicate).AsQueryable();
        }
    }
}
