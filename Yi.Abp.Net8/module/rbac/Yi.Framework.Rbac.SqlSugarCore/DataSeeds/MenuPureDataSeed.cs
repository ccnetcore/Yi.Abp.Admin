using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.SqlSugarCore.DataSeeds
{
    public class MenuPureDataSeed : IDataSeedContributor, ITransientDependency
    {
        private ISqlSugarRepository<MenuAggregateRoot> _repository;
        private IGuidGenerator _guidGenerator;
        public MenuPureDataSeed(ISqlSugarRepository<MenuAggregateRoot> repository, IGuidGenerator guidGenerator)
        {
            _repository = repository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _repository.IsAnyAsync(x => x.MenuName == "系统管理" && x.MenuSource == MenuSourceEnum.Pure))
            {
                await _repository.InsertManyAsync(GetSeedData());
            }
        }
        public List<MenuAggregateRoot> GetSeedData()
        {
            List<MenuAggregateRoot> entities = new List<MenuAggregateRoot>();
            //系统管理
            MenuAggregateRoot system = new MenuAggregateRoot(_guidGenerator.Create(), Guid.Empty)
            {
                MenuName = "系统管理",
                MenuType = MenuTypeEnum.Catalogue,
                Router = "/system",
                MenuIcon = "ri:settings-3-line",
                OrderNum = 100
            };
            entities.Add(system);

            //系统监控
            MenuAggregateRoot monitoring = new MenuAggregateRoot(_guidGenerator.Create())
            {
                MenuName = "系统监控",
                MenuType = MenuTypeEnum.Catalogue,
                Router = "/monitor",
                MenuIcon = "ep:monitor",
                OrderNum = 99,
            };
            entities.Add(monitoring);


            //在线用户
            MenuAggregateRoot online = new MenuAggregateRoot(_guidGenerator.Create(), monitoring.Id)
            {
                MenuName = "在线用户",
                PermissionCode = "monitor:online:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/monitor/online-user",
                MenuIcon = "ri:user-voice-line",
                OrderNum = 100,
                RouterName = "OnlineUser",
                Component = "monitor/online/index"
            };
            entities.Add(online);


            //Yi框架
            MenuAggregateRoot guide = new MenuAggregateRoot(_guidGenerator.Create())
            {
                MenuName = "Yi框架",
                MenuType = MenuTypeEnum.Catalogue,
                Router = "https://ccnetcore.com",
                IsLink = true,
                MenuIcon = "ri:at-line",
                OrderNum = 90,
                Component = null
            };
            entities.Add(guide);

            //用户管理
            MenuAggregateRoot user = new MenuAggregateRoot(_guidGenerator.Create())
            {
                MenuName = "用户管理",
                PermissionCode = "system:user:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/system/user/index",
                MenuIcon = "ri:admin-line",
                OrderNum = 100,
                ParentId = system.Id,
                RouterName = "SystemUser"
            };
            entities.Add(user);

            MenuAggregateRoot userQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "用户查询",
                PermissionCode = "system:user:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = user.Id,
                IsDeleted = false
            };
            entities.Add(userQuery);

            MenuAggregateRoot userAdd = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "用户新增",
                PermissionCode = "system:user:add",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = user.Id,
                IsDeleted = false
            };
            entities.Add(userAdd);

            MenuAggregateRoot userEdit = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "用户修改",
                PermissionCode = "system:user:edit",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = user.Id,
                IsDeleted = false
            };
            entities.Add(userEdit);

            MenuAggregateRoot userRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "用户删除",
                PermissionCode = "system:user:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = user.Id,
                IsDeleted = false
            };
            entities.Add(userRemove);


            MenuAggregateRoot userResetPwd = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "重置密码",
                PermissionCode = "system:user:resetPwd",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = user.Id,
                IsDeleted = false
            };
            entities.Add(userResetPwd);


            //角色管理
            MenuAggregateRoot role = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "角色管理",
                PermissionCode = "system:role:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/system/role/index",
                MenuIcon = "ri:admin-fill",
                OrderNum = 99,
                ParentId = system.Id,
                RouterName = "SystemRole"
            };
            entities.Add(role);

            MenuAggregateRoot roleQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "角色查询",
                PermissionCode = "system:role:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = role.Id,
                IsDeleted = false
            };
            entities.Add(roleQuery);

            MenuAggregateRoot roleAdd = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "角色新增",
                PermissionCode = "system:role:add",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = role.Id,
                IsDeleted = false
            };
            entities.Add(roleAdd);

            MenuAggregateRoot roleEdit = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "角色修改",
                PermissionCode = "system:role:edit",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = role.Id,
                IsDeleted = false
            };
            entities.Add(roleEdit);

            MenuAggregateRoot roleRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "角色删除",
                PermissionCode = "system:role:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = role.Id,
                IsDeleted = false
            };
            entities.Add(roleRemove);


            //菜单管理
            MenuAggregateRoot menu = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "菜单管理",
                PermissionCode = "system:menu:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/system/menu/index",
                MenuIcon = "ep:menu",
                OrderNum = 98,
                ParentId = system.Id,
                RouterName = "SystemMenu"
            };
            entities.Add(menu);

            MenuAggregateRoot menuQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "菜单查询",
                PermissionCode = "system:menu:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = menu.Id,
                IsDeleted = false
            };
            entities.Add(menuQuery);

            MenuAggregateRoot menuAdd = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "菜单新增",
                PermissionCode = "system:menu:add",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = menu.Id,
                IsDeleted = false
            };
            entities.Add(menuAdd);

            MenuAggregateRoot menuEdit = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "菜单修改",
                PermissionCode = "system:menu:edit",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = menu.Id,
                IsDeleted = false
            };
            entities.Add(menuEdit);

            MenuAggregateRoot menuRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "菜单删除",
                PermissionCode = "system:menu:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = menu.Id,
                IsDeleted = false
            };
            entities.Add(menuRemove);

            //部门管理
            MenuAggregateRoot dept = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "部门管理",
                PermissionCode = "system:dept:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/system/dept/index",
                MenuIcon = "ri:git-branch-line",
                OrderNum = 97,
                ParentId = system.Id,
                RouterName = "SystemDept"
            };
            entities.Add(dept);

            MenuAggregateRoot deptQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "部门查询",
                PermissionCode = "system:dept:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = dept.Id,
                IsDeleted = false
            };
            entities.Add(deptQuery);

            MenuAggregateRoot deptAdd = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "部门新增",
                PermissionCode = "system:dept:add",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = dept.Id,
                IsDeleted = false
            };
            entities.Add(deptAdd);

            MenuAggregateRoot deptEdit = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "部门修改",
                PermissionCode = "system:dept:edit",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = dept.Id,
                IsDeleted = false
            };
            entities.Add(deptEdit);

            MenuAggregateRoot deptRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "部门删除",
                PermissionCode = "system:dept:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = dept.Id,
                IsDeleted = false
            };
            entities.Add(deptRemove);



            //岗位管理
            MenuAggregateRoot post = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "岗位管理",
                PermissionCode = "system:post:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/system/post/index",
                MenuIcon = "ant-design:deployment-unit-outlined",
                OrderNum = 96,
                ParentId = system.Id,
                RouterName = "SystemPost"
            };
            entities.Add(post);

            MenuAggregateRoot postQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "岗位查询",
                PermissionCode = "system:post:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = post.Id,
                IsDeleted = false
            };
            entities.Add(postQuery);

            MenuAggregateRoot postAdd = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "岗位新增",
                PermissionCode = "system:post:add",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = post.Id,
                IsDeleted = false
            };
            entities.Add(postAdd);

            MenuAggregateRoot postEdit = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "岗位修改",
                PermissionCode = "system:post:edit",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = post.Id,
                IsDeleted = false
            };
            entities.Add(postEdit);

            MenuAggregateRoot postRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "岗位删除",
                PermissionCode = "system:post:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = post.Id,
                IsDeleted = false
            };
            entities.Add(postRemove);


            //操作日志
            MenuAggregateRoot operationLog = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "操作日志",
                PermissionCode = "monitor:operlog:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/monitor/operation-logs",
                MenuIcon = "ri:history-fill",
                OrderNum = 100,
                ParentId = monitoring.Id,
                RouterName = "OperationLog",
                Component = "monitor/logs/operation/index"
            };
            entities.Add(operationLog);

            MenuAggregateRoot operationLogQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "操作查询",
                PermissionCode = "monitor:operlog:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = operationLog.Id,
                IsDeleted = false
            };
            entities.Add(operationLogQuery);

            MenuAggregateRoot operationLogRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "操作删除",
                PermissionCode = "monitor:operlog:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = operationLog.Id,
                IsDeleted = false
            };
            entities.Add(operationLogRemove);


            //登录日志
            MenuAggregateRoot loginLog = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "登录日志",
                PermissionCode = "monitor:logininfor:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "/monitor/login-logs",
                IsShow = true,
                IsLink = false,
                IsCache = true,
                Component = "monitor/logs/login/index",
                MenuIcon = "ri:window-line",
                OrderNum = 100,
                ParentId = monitoring.Id,
                RouterName = "LoginLog",
            };
            entities.Add(loginLog);

            MenuAggregateRoot loginLogQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "登录查询",
                PermissionCode = "monitor:logininfor:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = loginLog.Id,
                IsDeleted = false
            };
            entities.Add(loginLogQuery);

            MenuAggregateRoot loginLogRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "登录删除",
                PermissionCode = "monitor:logininfor:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = loginLog.Id,
                IsDeleted = false,

            };
            entities.Add(loginLogRemove);

            //参数设置
            MenuAggregateRoot config = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "参数设置",
                PermissionCode = "system:config:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "config",
                IsShow = true,
                IsLink = false,
                IsCache = true,
                Component = "/system/config/index",
                MenuIcon = "ri:edit-box-line",
                OrderNum = 94,
                ParentId = system.Id,
                IsDeleted = false
            };
            entities.Add(config);

            MenuAggregateRoot configQuery = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "参数查询",
                PermissionCode = "system:config:query",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = config.Id,
                IsDeleted = false
            };
            entities.Add(configQuery);

            MenuAggregateRoot configAdd = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "参数新增",
                PermissionCode = "system:config:add",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = config.Id,
                IsDeleted = false
            };
            entities.Add(configAdd);

            MenuAggregateRoot configEdit = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "参数修改",
                PermissionCode = "system:config:edit",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = config.Id,
                IsDeleted = false
            };
            entities.Add(configEdit);

            MenuAggregateRoot configRemove = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "参数删除",
                PermissionCode = "system:config:remove",
                MenuType = MenuTypeEnum.Component,
                OrderNum = 100,
                ParentId = config.Id,
                IsDeleted = false
            };
            entities.Add(configRemove);


            //默认值
            entities.ForEach(m =>
            {
                m.IsDeleted = false;
                m.State = true;
                m.MenuSource = MenuSourceEnum.Pure;
                m.IsShow = true;
            });

            var p = entities.GroupBy(x => x.Id);
            foreach (var k in p)
            {
                if (k.ToList().Count > 1)
                {
                    Console.WriteLine("菜单id重复");
                }

            }
            return entities;
        }
    }
}
