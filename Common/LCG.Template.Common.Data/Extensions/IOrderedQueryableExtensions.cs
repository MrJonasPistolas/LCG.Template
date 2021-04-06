using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LCG.Template.Common.Data.Extensions
{
    public enum OrderByMethod
    {
        OrderBy,
        OrderByDescending,
    }

    public static class IOrderedQueryableExtensions
    {
        //https://stackoverflow.com/questions/31955025/generate-ef-orderby-expression-by-string
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IEnumerable<TSource> query, string propertyName, OrderByMethod methodName)
        {
            var entityType = typeof(TSource);

            //Create x=>x.PropName
            var propertyArray = propertyName.Split('.');

            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyArray[0]);
            Type propertyType = property.Type;
            for (int i = 1; i < propertyArray.Length; i++)
            {
                property = Expression.Property(property, propertyArray[i]);
                propertyType = property.Type;
            }


            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });
            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(System.Linq.Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == methodName.ToString() && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     //Put more restriction here to ensure selecting the right overload                
                     return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();
            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }

    }
}
