using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Rbac.Domain.Entities
{
    [SugarTable("File")]
    public class FileAggregateRoot : AggregateRoot<Guid>, IAuditedObject
    {
        public FileAggregateRoot()
        {
        }

        public FileAggregateRoot(Guid fileId)
        {
            this.Id = fileId;
        }

        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        /// <summary>
        /// 文件大小 
        ///</summary>
        [SugarColumn(ColumnName = "FileSize")]
        public decimal FileSize { get; set; }
        /// <summary>
        /// 文件名 
        ///</summary>
        [SugarColumn(ColumnName = "FileName")]
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径 
        ///</summary>
        [SugarColumn(ColumnName = "FilePath")]
        public string FilePath { get; set; }

        public DateTime CreationTime { get; set; }
        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        [SugarColumn(IsIgnore=true)]
        public override ExtraPropertyDictionary ExtraProperties { get; protected set; }
    }
}
