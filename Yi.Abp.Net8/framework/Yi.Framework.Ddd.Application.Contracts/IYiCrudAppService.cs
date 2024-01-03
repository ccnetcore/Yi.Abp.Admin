using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Yi.Framework.Ddd.Application.Contracts
{
    public interface IYiCrudAppService<TEntityDto, in TKey> : ICrudAppService<TEntityDto, TKey>
    {
    }

    public interface IYiCrudAppService<TEntityDto, in TKey, in TGetListInput> : ICrudAppService<TEntityDto, TKey, TGetListInput>
    {
    }

    public interface IYiCrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput> : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput>
    {
    }

    public interface IYiCrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput> : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    {
    }

    public interface IYiCrudAppService<TGetOutputDto, TGetListOutputDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput> : ICrudAppService<TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>, IDeletesAppService<TKey>
    {

    }
}
