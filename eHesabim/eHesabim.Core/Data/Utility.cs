using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace eHesabim.Core.Data {
    public static class Utility {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge) {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) {
            return first.Compose(second, Expression.Or);
        }

        public static IQueryable<T> Sort<T>(this IEnumerable<T> source, string sortBy, bool sortDescending) {
            var param = Expression.Parameter(typeof(T), "item");
            var propertyInfo = typeof(T).GetProperty(sortBy);

            switch (propertyInfo.PropertyType.ToString().ToLower()) {
                case "system.datetime":
                    var sortDateTime = Expression.Lambda<Func<T, DateTime>>(Expression.Convert(Expression.Property(param, sortBy), typeof(DateTime)), param);
                    return sortDescending ? source.AsQueryable().OrderByDescending(sortDateTime) : source.AsQueryable().OrderBy(sortDateTime);
                case "system.guid":
                    var sortGuid = Expression.Lambda<Func<T, Guid>>(Expression.Convert(Expression.Property(param, sortBy), typeof(Guid)), param);
                    return sortDescending ? source.AsQueryable().OrderByDescending(sortGuid) : source.AsQueryable().OrderBy(sortGuid);
                case "system.int32":
                case "system.ınt32":
                case "system.nullable`1[system.ınt32]":
                    var sortInt = Expression.Lambda<Func<T, int>>(Expression.Convert(Expression.Property(param, sortBy), typeof(int)), param);
                    return sortDescending ? source.AsQueryable().OrderByDescending(sortInt) : source.AsQueryable().OrderBy(sortInt);
                case "system.string":
                    var sortString = Expression.Lambda<Func<T, string>>(Expression.Convert(Expression.Property(param, sortBy), typeof(string)), param);
                    return sortDescending ? source.AsQueryable().OrderByDescending(sortString) : source.AsQueryable().OrderBy(sortString);
                default:
                    var sortDefault = Expression.Lambda<Func<T, object>>(Expression.Convert(Expression.Property(param, sortBy), typeof(object)), param);
                    return sortDescending ? source.AsQueryable().OrderByDescending(sortDefault) : source.AsQueryable().OrderBy(sortDefault);
            }
        }

        public static Func<T, bool> AndAlso<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2) {
            return arg => predicate1(arg) && predicate2(arg);
        }

        public static Func<T, bool> OrElse<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2) {
            return arg => predicate1(arg) || predicate2(arg);
        }
    }
}