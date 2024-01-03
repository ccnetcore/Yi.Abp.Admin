using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Helper
{
    public static class ExpressionHelper
    {


        /// <summary>
        /// Expression表达式树lambda参数拼接组合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="merge"></param>
        /// <returns></returns>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var parameterMap = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = LambdaParameteRebinder.ReplaceParameter(parameterMap, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        /// <summary>
        /// Expression表达式树lambda参数拼接--false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() => f => false;

        /// <summary>
        /// Expression表达式树lambda参数拼接-true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() => f => true;

        /// <summary>
        /// Expression表达式树lambda参数拼接--and
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) => first.Compose(second, Expression.And);

        /// <summary>
        /// Expression表达式树lambda参数拼接--or
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) => first.Compose(second, Expression.Or);
    }

    public class LambdaParameteRebinder : ExpressionVisitor
    {
        /// <summary>
        /// 存放表达式树的参数的字典
        /// </summary>
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="map"></param>
        public LambdaParameteRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// 重载参数访问的方法，访问表达式树参数，如果字典中包含，则取出
        /// </summary>
        /// <param name="node">表达式树参数</param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (map.TryGetValue(node, out ParameterExpression expression))
            {
                node = expression;
            }
            return base.VisitParameter(node);
        }

        public static Expression ReplaceParameter(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new LambdaParameteRebinder(map).Visit(exp);
        }
    }

}
