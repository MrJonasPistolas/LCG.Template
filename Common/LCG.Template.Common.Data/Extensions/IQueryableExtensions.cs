using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LCG.Template.Common.Data.Extensions
{
    public enum Operators
    {
        Equal,
        Contains,
        GreaterOrEqual,
        LowerOrEqual,
        GreaterThan,
        LessThan,
    }

    public static class IQueryableExtensions
    {

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> query, string propertyName, object value, Operators operators)
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

            value = Convert.ChangeType(value, property.Type);
            ConstantExpression constant = Expression.Constant(value, property.Type);
            Expression body = null;

            if (operators == Operators.Equal)
            {
                body = Expression.Equal(property, constant);
            }
            else if (operators == Operators.Contains)
            {
                body = Expression.Call(property, "Contains", Type.EmptyTypes, Expression.Constant(value));
            }
            else if (operators == Operators.GreaterOrEqual)
            {
                body = Expression.GreaterThanOrEqual(property, constant);
            }
            else if (operators == Operators.GreaterThan)
            {
                body = Expression.GreaterThan(property, constant);
            }
            else if (operators == Operators.LowerOrEqual)
            {
                body = Expression.LessThanOrEqual(property, constant);
            }
            else if (operators == Operators.LessThan)
            {
                body = Expression.LessThan(property, constant);
            }

            var condition = Expression.Lambda(body, new ParameterExpression[] { arg });


            var enumarableType = typeof(System.Linq.Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     //Put more restriction here to ensure selecting the right overload                
                     return parameters.Count == 2;//overload that has 2 parameters
                 }).First();
            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method.MakeGenericMethod(entityType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { query, condition });
            return newQuery;
        }
    }
}
