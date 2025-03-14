using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Yi.Framework.Ddd.Application.Contracts
{
    /// <summary>
    /// Yi框架CRUD服务基础接口
    /// </summary>
    /// <typeparam name="TEntityDto">实体DTO类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IYiCrudAppService<TEntityDto, in TKey> : ICrudAppService<TEntityDto, TKey>
    {
    }

    /// <summary>
    /// Yi框架CRUD服务接口（带查询输入）
    /// </summary>
    /// <typeparam name="TEntityDto">实体DTO类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TGetListInput">查询输入类型</typeparam>
    public interface IYiCrudAppService<TEntityDto, in TKey, in TGetListInput> : ICrudAppService<TEntityDto, TKey, TGetListInput>
    {
    }

    /// <summary>
    /// Yi框架CRUD服务接口（带查询输入和创建输入）
    /// </summary>
    /// <typeparam name="TEntityDto">实体DTO类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    /// <typeparam name="TGetListInput">查询输入类型</typeparam>
    /// <typeparam name="TCreateInput">创建输入类型</typeparam>
    public interface IYiCrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput> : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput>
    {
    }

    /// <summary>
    /// Yi框架CRUD服务接口（带查询、创建和更新输入）
    /// </summary>
    public interface IYiCrudAppService<TEntityDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput> : ICrudAppService<TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    {
    }

    /// <summary>
    /// Yi框架完整CRUD服务接口（包含所有操作和批量删除功能）
    /// </summary>
    public interface IYiCrudAppService<TGetOutputDto, TGetListOutputDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput> : ICrudAppService<TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>, IDeletesAppService<TKey>
    {
    }
}
