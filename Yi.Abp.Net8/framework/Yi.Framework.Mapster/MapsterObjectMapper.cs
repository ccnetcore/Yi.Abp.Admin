using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Yi.Framework.Mapster
{
    /// <summary>
    /// Mapster对象映射器
    /// 实现IObjectMapper接口，提供对象映射功能
    /// </summary>
    public class MapsterObjectMapper : IObjectMapper
    {
        private readonly IAutoObjectMappingProvider _autoObjectMappingProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="autoObjectMappingProvider">自动对象映射提供程序</param>
        public MapsterObjectMapper(IAutoObjectMappingProvider autoObjectMappingProvider)
        {
            _autoObjectMappingProvider = autoObjectMappingProvider;
        }

        /// <summary>
        /// 获取自动对象映射提供程序
        /// </summary>
        public IAutoObjectMappingProvider AutoObjectMappingProvider => _autoObjectMappingProvider;

        /// <summary>
        /// 将源对象映射到目标类型
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>映射后的目标类型实例</returns>
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return AutoObjectMappingProvider.Map<TSource, TDestination>(source);
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
            return AutoObjectMappingProvider.Map(source, destination);
        }
    }
}
