using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Volo.Abp.ObjectMapping;

namespace Yi.Framework.Mapster
{
    /// <summary>
    /// Mapster自动对象映射提供程序
    /// 实现IAutoObjectMappingProvider接口，提供对象间的自动映射功能
    /// </summary>
    public class MapsterAutoObjectMappingProvider : IAutoObjectMappingProvider
    {
        /// <summary>
        /// 将源对象映射到目标类型
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>映射后的目标类型实例</returns>
        public TDestination Map<TSource, TDestination>(object source)
        {
            // 使用Mapster的Adapt方法进行对象映射
            return source.Adapt<TDestination>();
        }

        /// <summary>
        /// 将源对象映射到现有的目标对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <returns>映射后的目标对象</returns>
        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            // 使用Mapster的Adapt方法进行对象映射，保留目标对象的实例
            return source.Adapt(destination);
        }
    }
}
