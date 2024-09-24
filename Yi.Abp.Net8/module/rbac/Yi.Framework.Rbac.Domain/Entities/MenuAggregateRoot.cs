using System.Web;
using NUglify.Helpers;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;
using Yi.Framework.Core.Helper;
using Yi.Framework.Rbac.Domain.Shared.Dtos;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Domain.Entities
{
    /// <summary>
    /// 菜单表
    ///</summary>
    [SugarTable("Menu")]
    public partial class MenuAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject, IOrderNum, IState
    {
        public MenuAggregateRoot()
        {
        }

        public MenuAggregateRoot(Guid id)
        {
            Id = id;
            ParentId = Guid.Empty;
        }

        public MenuAggregateRoot(Guid id, Guid parentId)
        {
            Id = id;
            ParentId = parentId;
        }

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
        public bool State { get; set; }

        /// <summary>
        /// 菜单名
        /// </summary>
        public string MenuName { get; set; } 

        /// <summary>
        /// 路由名称
        /// </summary>
        public string? RouterName { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "MenuType")]
        public MenuTypeEnum MenuType { get; set; } = MenuTypeEnum.Menu;

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "PermissionCode")]
        public string? PermissionCode { get; set; }

        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "ParentId")]
        public Guid ParentId { get; set; }

        /// <summary>
        /// 菜单图标 
        ///</summary>
        [SugarColumn(ColumnName = "MenuIcon")]
        public string? MenuIcon { get; set; }

        /// <summary>
        /// 菜单组件路由 
        ///</summary>
        [SugarColumn(ColumnName = "Router")]
        public string? Router { get; set; }

        /// <summary>
        /// 是否为外部链接 
        ///</summary>
        [SugarColumn(ColumnName = "IsLink")]
        public bool IsLink { get; set; }

        /// <summary>
        /// 是否缓存 
        ///</summary>
        [SugarColumn(ColumnName = "IsCache")]
        public bool IsCache { get; set; }

        /// <summary>
        /// 是否显示 
        ///</summary>
        [SugarColumn(ColumnName = "IsShow")]
        public bool IsShow { get; set; } = true;

        /// <summary>
        /// 描述 
        ///</summary>
        [SugarColumn(ColumnName = "Remark")]
        public string? Remark { get; set; }

        /// <summary>
        /// 组件路径 
        ///</summary>
        [SugarColumn(ColumnName = "Component")]
        public string? Component { get; set; }

        /// <summary>
        /// 菜单来源
        /// </summary>
        public MenuSourceEnum MenuSource { get; set; } = MenuSourceEnum.Ruoyi;

        /// <summary>
        /// 路由参数 
        ///</summary>
        [SugarColumn(ColumnName = "Query")]
        public string? Query { get; set; }

        [SugarColumn(IsIgnore = true)] public List<MenuAggregateRoot>? Children { get; set; }
    }

    /// <summary>
    /// 实体扩展
    /// </summary>
    public static class MenuEntityExtensions
    {
        /// <summary>
        /// 构建vue3路由
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public static List<Vue3RouterDto> Vue3RuoYiRouterBuild(this List<MenuAggregateRoot> menus)
        {
            menus = menus
                .Where(m => m.State == true)
                .Where(m => m.MenuType != MenuTypeEnum.Component)
                .Where(m => m.MenuSource == MenuSourceEnum.Ruoyi)
                .ToList();
            List<Vue3RouterDto> routers = new();
            foreach (var m in menus)
            {
                var r = new Vue3RouterDto();
                r.OrderNum = m.OrderNum;
                var routerName = m.Router?.Split("/").LastOrDefault();
                r.Id = m.Id;
                r.ParentId = m.ParentId;

                //开头大写
                r.Name = routerName?.First().ToString().ToUpper() + routerName?.Substring(1);
                r.Path = m.Router!;
                r.Hidden = !m.IsShow;


                if (m.MenuType == MenuTypeEnum.Catalogue)
                {
                    r.Redirect = "noRedirect";
                    r.AlwaysShow = true;

                    //判断是否为最顶层的路由
                    if (Guid.Empty == m.ParentId)
                    {
                        r.Component = "Layout";
                    }
                    else
                    {
                        r.Component = "ParentView";
                    }
                }

                if (m.MenuType == MenuTypeEnum.Menu)
                {
                    r.Redirect = "noRedirect";
                    r.AlwaysShow = true;
                    r.Component = m.Component!;
                    r.AlwaysShow = false;
                }

                r.Meta = new Meta
                {
                    Title = m.MenuName!,
                    Icon = m.MenuIcon!,
                    NoCache = !m.IsCache
                };
                if (m.IsLink)
                {
                    r.Meta.link = m.Router!;
                    r.AlwaysShow = false;
                }

                routers.Add(r);
            }

            return TreeHelper.SetTree(routers);
        }


        /// <summary>
        /// 构建vue3  pure路由
        /// </summary>
        /// <param name="menus"></param>
        /// <returns></returns>
        public static List<Vue3PureRouterDto> Vue3PureRouterBuild(this List<MenuAggregateRoot> menus)
        {
            //pure的菜单为树形
            var allRouters = menus
                .Where(m => m.State == true)
                .Where(m => m.MenuType != MenuTypeEnum.Component)
                .Where(m => m.MenuSource == MenuSourceEnum.Pure)
                .Select(m => new Vue3PureRouterDto
                {
                    Path =m.Router.StartsWith("/")?m.Router:"/"+m.Router,
                    Name =m.IsLink==true?"Link": m.RouterName,
                    component = m.Component,
                    Meta = new MetaPureRouterDto()
                    {
                        showLink = m.IsShow,
                        FrameSrc = m.IsLink == true ? m.Router : null,
                        Auths = new List<string>() { m.PermissionCode },
                        Icon = m.MenuIcon,
                        Title = m.MenuName,
                        
                    },
                    Children =null,
                    Id = m.Id,
                    ParentId = m.ParentId
                })
                .ToList();

            
            var routerDic = allRouters.GroupBy(x => x.ParentId).ToDictionary(x => x.Key,y=>y.ToList());
            //根路由
            if (!routerDic.TryGetValue(Guid.Empty, out var rootRouters))
            {
                return new List<Vue3PureRouterDto>();
            }
            Stack<Vue3PureRouterDto> stack = new Stack<Vue3PureRouterDto>(rootRouters);
            while (stack.Count > 0)
            {
                var currentRouter = stack.Pop();
                if (routerDic.TryGetValue(currentRouter.Id, out var items))
                {
                    currentRouter.Children = items;
                    items?.ForEach(x => stack.Push(x));
                }
            }

            return rootRouters;
        }
    }
}