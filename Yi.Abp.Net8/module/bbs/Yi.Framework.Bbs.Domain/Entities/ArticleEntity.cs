using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities
{
    [SugarTable("Article")]
    public class ArticleEntity : Entity<Guid>, ISoftDelete,IAuditedObject
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public bool IsDeleted { get; set; }

        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string Content { get; set; }
        public string Name { get; set; }


        public Guid DiscussId { get; set; }

        public Guid ParentId { get; set; }

        [SugarColumn(IsIgnore = true)]

        public List<ArticleEntity>? Children { get; set; }


        public DateTime CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }

    public static class ArticleEntityExtensions
    {
        /// <summary>
        /// 平铺自己
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<ArticleEntity> Tile(this List<ArticleEntity> entities)
        {
            if (entities is null) return new List<ArticleEntity>();
            var result = new List<ArticleEntity>();
            return StartRecursion(entities, result);
        }

        private static List<ArticleEntity> StartRecursion(List<ArticleEntity> entities, List<ArticleEntity> result)
        {
            foreach (var entity in entities)
            {
                result.Add(entity);
                if (entity.Children is not null && entity.Children.Where(x => x.IsDeleted == false).Count() > 0)
                {
                    StartRecursion(entity.Children, result);
                }
            }
            return result;
        }

    }
}
