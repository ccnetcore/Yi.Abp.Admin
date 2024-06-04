using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("Article")]
    [SugarIndex($"index_{nameof(Name)}", nameof(Name), OrderByType.Asc)]
    [SugarIndex($"index_{nameof(ParentId)}", nameof(ParentId), OrderByType.Asc)]
    [SugarIndex($"index_{nameof(DiscussId)}", nameof(DiscussId), OrderByType.Asc)]
    public class ArticleAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject
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

        public List<ArticleAggregateRoot>? Children { get; set; }


        public DateTime CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;
    }

    public static class ArticleEntityExtensions
    {
        /// <summary>
        /// 平铺自己
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<ArticleAggregateRoot> Tile(this List<ArticleAggregateRoot> entities)
        {
            if (entities is null) return new List<ArticleAggregateRoot>();
            var result = new List<ArticleAggregateRoot>();
            return StartRecursion(entities, result);
        }

        private static List<ArticleAggregateRoot> StartRecursion(List<ArticleAggregateRoot> entities, List<ArticleAggregateRoot> result)
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
