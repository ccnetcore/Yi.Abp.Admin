using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;
using Yi.Framework.Core.Helper;
using Yi.Framework.Rbac.Domain.Shared.Dtos;

namespace Yi.Framework.Rbac.Domain.Entities
{
    /// <summary>
    /// 部门表
    ///</summary>
    [SugarTable("Dept")]
    public class DeptAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject, IOrderNum, IState
    {
        public DeptAggregateRoot()
        {
        }

        public DeptAggregateRoot(Guid Id) { this.Id = Id; ParentId = Guid.Empty; }

        public DeptAggregateRoot(Guid Id, Guid parentId) { this.Id = Id; ParentId = parentId; }
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建者
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;

        /// <summary>
        /// 部门名称 
        ///</summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 部门编码 
        ///</summary>
        [SugarColumn(ColumnName = "DeptCode")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 负责人 
        ///</summary>
        [SugarColumn(ColumnName = "Leader")]
        public string? Leader { get; set; }
        /// <summary>
        /// 父级id 
        ///</summary>
        [SugarColumn(ColumnName = "ParentId")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 描述 
        ///</summary>
        [SugarColumn(ColumnName = "Remark")]
        public string? Remark { get; set; }

    }
    
    /// <summary>
    /// 部门实体扩展
    /// </summary>
    public static class DeptEntityExtensions
    {
        /// <summary>
        /// 构建部门树形列表
        /// </summary>
        /// <param name="depts">部门列表</param>
        /// <returns>树形结构的部门列表</returns>
        public static List<DeptTreeDto> DeptTreeBuild(this List<DeptAggregateRoot> depts)
        {
            // 过滤启用的部门
            var filteredDepts = depts
                .Where(d => d.State == true)
                .ToList();

            List<DeptTreeDto> deptTrees = new();
            foreach (var dept in filteredDepts)
            {
                var deptTree = new DeptTreeDto
                {
                    Id = dept.Id,
                    OrderNum = dept.OrderNum,
                    DeptName = dept.DeptName,
                    State = dept.State,
                    ParentId = dept.ParentId,
                };

                deptTrees.Add(deptTree);
            }
            
            return TreeHelper.SetTree(deptTrees);
        }
    }
}
