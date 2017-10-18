using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace eHesabim.Core.Data {
    public static class EntityExtensions {
        public static List<T> ToListNoLock<T>(this IEnumerable<T> query) {
            using (var ts = CreateNoLockTransaction()) {
                return query.ToList();
            }
        }

        ////public static T FirstOrDefault<T>(this IEnumerable<T> query) {
        ////    using (var ts = CreateNoLockTransaction()) {
        ////        return query.FirstOrDefault();
        ////    }
        ////}

        ////public static T LastOrDefaultNoLock<T>(this IEnumerable<T> query) {
        ////    using (var ts = CreateNoLockTransaction()) {
        ////        return query.LastOrDefault();
        ////    }
        ////}

        ////public static T SingleOrDefaultNoLock<T>(this IEnumerable<T> query) {
        ////    using (var ts = CreateNoLockTransaction()) {
        ////        return query.SingleOrDefault();
        ////    }
        ////}

        ////public static bool AnyNoLock<T>(this IEnumerable<T> query) {
        ////    using (var ts = CreateNoLockTransaction()) {
        ////        return query.Any();
        ////    }
        ////}

        private static TransactionScope CreateNoLockTransaction() {
            var options = new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted };
            return new TransactionScope(TransactionScopeOption.Required, options);
        }
    }
}
