using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Dept;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Repositories;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Dtos;

namespace Yi.Framework.Rbac.Application.Services.System
{
    /// <summary>
    /// Dept服务实现
    /// </summary>
    public class DeptService : YiCrudAppService<DeptAggregateRoot, DeptGetOutputDto, DeptGetListOutputDto, Guid,
        DeptGetListInputVo, DeptCreateInputVo, DeptUpdateInputVo>, IDeptService
    {
        private IDeptRepository _repository;

        public DeptService(IDeptRepository repository) : base(repository)
        {
            _repository = repository;
        }

        [RemoteService(false)]
        public async Task<List<Guid>> GetChildListAsync(Guid deptId)
        {
            return await _repository.GetChildListAsync(deptId);
        }

        /// <summary>
        /// 通过角色id查询该角色全部部门
        /// </summary>
        /// <returns></returns>
        //[Route("{roleId}")]
        public async Task<List<DeptGetListOutputDto>> GetRoleIdAsync(Guid roleId)
        {
            var entities = await _repository.GetListRoleIdAsync(roleId);
            return await MapToGetListOutputDtosAsync(entities);
        }

        /// <summary>
        /// 多查
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<DeptGetListOutputDto>> GetListAsync(DeptGetListInputVo input)
        {
            RefAsync<int> total = 0;
            var entities = await _repository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.DeptName), u => u.DeptName.Contains(input.DeptName!))
                .WhereIF(input.State is not null, u => u.State == input.State)
                .OrderBy(u => u.OrderNum, OrderByType.Asc)
                .ToListAsync();
            return new PagedResultDto<DeptGetListOutputDto>
            {
                Items = await MapToGetListOutputDtosAsync(entities),
                TotalCount = total
            };
        }

        protected override async Task CheckCreateInputDtoAsync(DeptCreateInputVo input)
        {
            var isExist =
                await _repository.IsAnyAsync(x => x.DeptCode == input.DeptCode);
            if (isExist)
            {
                throw new UserFriendlyException(DeptConst.Exist);
            }
        }

        protected override async Task CheckUpdateInputDtoAsync(DeptAggregateRoot entity, DeptUpdateInputVo input)
        {
            var isExist = await _repository._DbQueryable.Where(x => x.Id != entity.Id)
                .AnyAsync(x => x.DeptCode == input.DeptCode);
            if (isExist)
            {
                throw new UserFriendlyException(DeptConst.Exist);
            }
        }
        
        /// <summary>
        /// 多查
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("dept/list")]
        public async Task<List<DeptGetListOutputDto>> GetAllListAsync(DeptGetListInputVo input)
        {
            var entities = await _repository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.DeptName), u => u.DeptName.Contains(input.DeptName!))
                .WhereIF(input.State is not null, u => u.State == input.State)
                .OrderBy(u => u.OrderNum, OrderByType.Asc)
                .ToListAsync();
            
            return await MapToGetListOutputDtosAsync(entities);
        }
        
                /// <summary>
        /// 获取树形结构的部门列表
        /// </summary>
        /// <returns>树形结构的部门列表</returns>
        public async Task<List<DeptTreeDto>> GetTreeAsync()
        {
            // 获取所有启用的部门
            var entities = await _repository._DbQueryable
                .Where(x => x.State == true)
                .OrderBy(x => x.OrderNum, OrderByType.Asc)
                .ToListAsync();
            return entities.DeptTreeBuild();
        }

        /// <summary>
        /// 获取排除指定部门及其子孙部门的部门列表
        /// </summary>
        /// <param name="id">要排除的部门ID</param>
        /// <returns>排除后的部门列表</returns>
        [HttpGet]
        [Route("dept/list/exclude/{id}")]
        public async Task<List<DeptGetListOutputDto>> GetListExcludeAsync(Guid id)
        {
            // 获取要排除的部门及其所有子孙部门的ID
            var excludeIds = await GetAllChildrenIdsAsync(id);
            excludeIds.Add(id); // 同时排除自己

            // 查询并过滤掉排除的部门
            var result = await _repository._DbQueryable
                .Where(x => !excludeIds.Contains(x.Id))
                .OrderBy(dept => dept.OrderNum, OrderByType.Asc)
                .Select(dept => new DeptGetListOutputDto
                {
                    Id = dept.Id,
                    CreationTime = dept.CreationTime,
                    CreatorId = dept.CreatorId,
                    State = dept.State,
                    DeptName = dept.DeptName,
                    DeptCode = dept.DeptCode,
                    Leader = dept.Leader,
                    ParentId = dept.ParentId,
                    Remark = dept.Remark,
                    OrderNum = dept.OrderNum
                })
                .ToListAsync();

            return result;
        }

        /// <summary>
        /// 递归获取指定部门的所有子孙部门ID
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>所有子孙部门ID列表</returns>
        private async Task<List<Guid>> GetAllChildrenIdsAsync(Guid deptId)
        {
            var result = new List<Guid>();
            
            // 获取所有部门
            var allDepts = await _repository._DbQueryable.ToListAsync();
            
            // 递归获取子孙部门ID
            GetChildrenIdsRecursive(deptId, allDepts, result);
            
            return result;
        }

        /// <summary>
        /// 递归辅助方法：获取子孙部门ID
        /// </summary>
        /// <param name="parentId">父部门ID</param>
        /// <param name="allDepts">所有部门列表</param>
        /// <param name="result">结果列表</param>
        private void GetChildrenIdsRecursive(Guid parentId, List<DeptAggregateRoot> allDepts, List<Guid> result)
        {
            // 查找直接子部门
            var children = allDepts.Where(x => x.ParentId == parentId).ToList();
            
            foreach (var child in children)
            {
                // 添加子部门ID
                result.Add(child.Id);
                
                // 递归获取子部门的子部门
                GetChildrenIdsRecursive(child.Id, allDepts, result);
            }
        }
    }
}