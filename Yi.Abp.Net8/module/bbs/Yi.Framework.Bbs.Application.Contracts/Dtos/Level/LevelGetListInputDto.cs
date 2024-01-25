using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Level
{
    public class LevelGetListInputDto:PagedResultRequestDto
    {  /// <summary>
       /// 当前等级
       /// </summary>
        public int? MinLevel { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        public string? Name { get; set; }
    }
}
