using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar.DistributedSystem.Snowflake;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.SqlSugarCore.DataSeeds
{
    public class BbsMenuDataSeed : IDataSeedContributor, ITransientDependency
    {
        private IGuidGenerator _guidGenerator;
        private ISqlSugarRepository<MenuAggregateRoot, Guid> _repository;
        public BbsMenuDataSeed(ISqlSugarRepository<MenuAggregateRoot,Guid> repository, IGuidGenerator guidGenerator)
        {
            _repository=repository;
            _guidGenerator=guidGenerator;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            if (!await _repository.IsAnyAsync(x => x.MenuName == "BBS"))
            {
                await _repository.InsertManyAsync(GetSeedData());
            }
        }

        public List<MenuAggregateRoot> GetSeedData()
        {
            List<MenuAggregateRoot> entities = new List<MenuAggregateRoot>();
            //BBS
            MenuAggregateRoot bbs = new MenuAggregateRoot(_guidGenerator.Create())
            {
                MenuName = "BBS",
                MenuType = MenuTypeEnum.Catalogue,
                Router = "/bbs",
                IsShow = true,
                IsLink = false,
                MenuIcon = "monitor",
                OrderNum = 91,
                IsDeleted = false
            };
            entities.Add(bbs);



            //板块管理
            MenuAggregateRoot plate = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "板块管理",
                PermissionCode = "bbs:plate:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "plate",
                IsShow = true,
                IsLink = false,
                IsCache = true,
                Component = "bbs/plate/index",
                MenuIcon = "component",
                OrderNum = 100,
                ParentId = bbs.Id,
                IsDeleted = false
            };
            entities.Add(plate);

            //文章管理
            MenuAggregateRoot article = new MenuAggregateRoot(_guidGenerator.Create())
            {

                MenuName = "文章管理",
                PermissionCode = "bbs:article:list",
                MenuType = MenuTypeEnum.Menu,
                Router = "article",
                IsShow = true,
                IsLink = false,
                IsCache = true,
                Component = "bbs/article/index",
                MenuIcon = "documentation",
                OrderNum = 99,
                ParentId = bbs.Id,
                IsDeleted = false
            };
            entities.Add(article);


            return entities;
        }
    }
}
