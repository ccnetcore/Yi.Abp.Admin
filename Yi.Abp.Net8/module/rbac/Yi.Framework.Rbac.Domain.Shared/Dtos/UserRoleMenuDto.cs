using SqlSugar;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Domain.Shared.Dtos
{
    public class UserRoleMenuDto
    {
        public UserDto User { get; set; } = new();
        public HashSet<RoleDto> Roles { get; set; } = new();
        public HashSet<MenuDto> Menus { get; set; } = new();

        public List<string> RoleCodes { get; set; } = new();
        public List<string> PermissionCodes { get; set; } = new();
    }

    public class UserDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid Id { get;  set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 加密盐值
        /// </summary>
        public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nick { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 地址
        /// </summary>

        public string? Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public long? Phone { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string? Introduction { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public SexEnum Sex { get; set; } = SexEnum.Unknown;

        /// <summary>
        /// 部门id
        /// </summary>
        public Guid? DeptId { get; set; }

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
    }
    public class RoleDto
    {
        public Guid Id { get;  set; }

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
        /// 角色名
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色编码 
        ///</summary>
        public string RoleCode { get; set; } = string.Empty;

        /// <summary>
        /// 描述 
        ///</summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 角色数据范围 
        ///</summary>
        public DataScopeEnum DataScope { get; set; } = DataScopeEnum.ALL;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;

    }
    public class MenuDto
    {
        public Guid Id { get;  set; }

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
        public bool State { get; set; }

        /// <summary>
        /// 菜单名
        /// </summary>
        public string MenuName { get; set; } = string.Empty;
        /// <summary>
        ///  
        ///</summary>
        public MenuTypeEnum MenuType { get; set; } = MenuTypeEnum.Menu;
        /// <summary>
        ///  
        ///</summary>
        public string? PermissionCode { get; set; }
        /// <summary>
        ///  
        ///</summary>

        public Guid ParentId { get; set; }

        /// <summary>
        /// 菜单图标 
        ///</summary>

        public string? MenuIcon { get; set; }
        /// <summary>
        /// 菜单组件路由 
        ///</summary>

        public string? Router { get; set; }
        /// <summary>
        /// 是否为外部链接 
        ///</summary>

        public bool IsLink { get; set; }
        /// <summary>
        /// 是否缓存 
        ///</summary>

        public bool IsCache { get; set; }
        /// <summary>
        /// 是否显示 
        ///</summary>
        public bool IsShow { get; set; } = true;

        /// <summary>
        /// 描述 
        ///</summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 组件路径 
        ///</summary>
        public string? Component { get; set; }
        /// <summary>
        /// 路由参数 
        ///</summary>
        public string? Query { get; set; }
    }
}
