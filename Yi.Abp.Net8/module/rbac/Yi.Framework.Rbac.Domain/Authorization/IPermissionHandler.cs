namespace Yi.Framework.Rbac.Domain.Authorization
{
    public interface IPermissionHandler
    {
        bool IsPass(string permission);
    }
}
