using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.OperLog;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// OperationLog服务抽象
    /// </summary>
    public interface IOperationLogService : IYiCrudAppService<OperationLogGetListOutputDto, Guid, OperationLogGetListInputVo>
    {

    }
}
