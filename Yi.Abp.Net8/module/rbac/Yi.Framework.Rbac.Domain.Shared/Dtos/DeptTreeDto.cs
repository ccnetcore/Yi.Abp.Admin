using System.Collections.Generic;
using static Yi.Framework.Core.Helper.TreeHelper;

namespace Yi.Framework.Rbac.Domain.Shared.Dtos
{
    /// <summary>
    /// 部门树形DTO
    /// </summary>
    public class DeptTreeDto : ITreeModel<DeptTreeDto>
    {
        /// <summary>
        /// 部门ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 父部门ID
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int OrderNum { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; } = string.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        
        /// <summary>
        /// 子部门列表
        /// </summary>
        public List<DeptTreeDto>? Children { get; set; }
    }
}

