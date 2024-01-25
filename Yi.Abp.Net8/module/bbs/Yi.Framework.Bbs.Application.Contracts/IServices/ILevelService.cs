using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Level;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    public interface ILevelService : IYiCrudAppService<LevelOutputDto, Guid, LevelGetListInputDto>
    {
    }
}
