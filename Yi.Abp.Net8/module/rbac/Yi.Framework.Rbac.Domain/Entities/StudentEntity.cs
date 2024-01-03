using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Rbac.Domain.Entities
{
    public class StudentEntity : Entity<Guid>
    {

        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public string Name { get; set; }

    }
}
