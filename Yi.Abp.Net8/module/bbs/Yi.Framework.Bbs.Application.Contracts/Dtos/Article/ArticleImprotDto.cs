using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    public class ArticleImprotDto
    {
        /// <summary>
        /// 主题id
        /// </summary>
        [Required]
        public Guid DiscussId { get; set; }

        public Guid ArticleParentId { get; set; }= Guid.Empty;

        public ArticleImportTypeEnum ImportType { get; set; } = ArticleImportTypeEnum.Default;
}
}
