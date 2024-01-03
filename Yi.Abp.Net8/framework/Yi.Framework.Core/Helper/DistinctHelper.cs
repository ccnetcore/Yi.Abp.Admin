using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Helper
{
    public class Compare<T, C> : IEqualityComparer<T>
    {
        private Func<T, C> _getField;
        public Compare(Func<T, C> getfield)
        {
            _getField = getfield;
        }
        public bool Equals(T? x, T? y)
        {
            return EqualityComparer<C>.Default.Equals(_getField(x!), _getField(y!));
        }

        public int GetHashCode(T obj)
        {
            return EqualityComparer<C>.Default.GetHashCode(_getField(obj)!);
        }
    }
    public static class DistinctHelper
    {
        /// <summary>
        /// 自定义Distinct扩展方法
        /// </summary>
        /// <typeparam name="T">要去重的对象类</typeparam>
        /// <typeparam name="C">自定义去重的字段类型</typeparam>
        /// <param name="source">要去重的对象</param>
        /// <param name="getfield">获取自定义去重字段的委托</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctNew<T, C>(this IEnumerable<T> source, Func<T, C> getfield)
        {
            return source.Distinct(new Compare<T, C>(getfield));
        }
    }
}
