using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Volo.Abp.ObjectMapping;

namespace Yi.Framework.Mapster
{
    public class MapsterAutoObjectMappingProvider : IAutoObjectMappingProvider
    {
        public TDestination Map<TSource, TDestination>(object source)
        {
            var sss = typeof(TDestination).Name;
            return source.Adapt<TDestination>();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return source.Adapt<TSource, TDestination>(destination);
        }
    }
}
