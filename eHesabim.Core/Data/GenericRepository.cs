using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;

using eHesabim.Data;

namespace eHesabim.Core.Data {
    public class GenericRepository<T, TId> : ReadOnlyRepository<T, TId>, IRepository<T, TId> where T : BaseEntity<TId> {
        public GenericRepository(IDataContext context)
            : base(context) {
        }

        public TId AddUpdate(T entity) {
            try {
                if (Entities.Find(entity.Id) == null) {
                    if (typeof(TId) == typeof(Guid)) {
                        entity.Id = (TId)Convert.ChangeType(Guid.NewGuid(), typeof(TId));
                    }

                    entity.IsDeleted = false;
                    entity.CreatedDateTime = DateTime.Now;
                    Entities.Add(entity);
                }
                else {
                    entity.UpdatedDateTime = DateTime.Now;
                }

                DataContext.SaveChanges();
                return entity.Id;
            }
            catch (Exception ex) {
                var s = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw;
            }
        }

        public void InsertBulkData(IEnumerable<T> items) {
            DataContext.SetAutoDetectChanges(false);

            foreach (var item in items) {
                DataContext.Set<T>().Add(item);
            }

            DataContext.SaveChanges();
            DataContext.SetAutoDetectChanges(true);
        }

        public void Delete(TId id) {
            var entity = Detail(id);
            if (entity == null) {
                return;
            }

            entity.IsDeleted = true;
            entity.UpdatedDateTime = DateTime.Now;
            DataContext.SaveChanges();
        }

        public override IQueryable<T> Query() {
            return Table.Where(a => !a.IsDeleted).AsQueryable();
        }

        public override IQueryable<T> Query(Expression<Func<T, bool>> predicate) {
            return Table.Where(a => !a.IsDeleted).Where(predicate).AsQueryable();
        }

        public IQueryable<T> Filter(Expression<Func<T, bool>> filter, int index, int size, string sort, bool sortDescending, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "CreatedDateTime";
                sortDescending = true;
            }

            var skipCount = index * size;
            var resetSet = filter != null ? Table.Where(a => !a.IsDeleted).Where(filter).AsQueryable() : Table.Where(a => !a.IsDeleted).AsQueryable();
            total = resetSet.Count();

            resetSet = resetSet.OrderBy(string.Format("{0} {1} {2}", sort, sortDescending ? "DESC" : string.Empty, ", CreatedDateTime DESC"));
            resetSet = skipCount == 0 ? resetSet.Take(size) : resetSet.Skip(skipCount).Take(size);
            return resetSet.AsQueryable();
        }
    }
}
