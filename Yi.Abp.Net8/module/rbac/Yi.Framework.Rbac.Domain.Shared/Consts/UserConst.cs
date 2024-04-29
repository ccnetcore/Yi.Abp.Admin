using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Rbac.Domain.Shared.Consts
{
    /// <summary>
    /// 常量定义
    /// </summary>

    public class UserConst
    {
        public const string Login_Error = "登录失败！用户名或密码错误！";
        public const string Login_User_No_Exist = "登录失败！用户名不存在！";
        public const string Login_Passworld_Error = "密码为空，添加失败！";
        public const string Create_Passworld_Error = "密码格式错误，长度需大于等于6位";
        public const string User_Exist = "用户已经存在，创建失败！";
        public const string State_Is_State = "该用户已被禁用，请联系管理员进行恢复";
        public const string No_Permission = "登录禁用！该用户分配无任何权限，无意义登录！";
        public const string No_Role = "登录禁用！该用户分配无任何角色，无意义登录！";
        public const string Name_Not_Allowed = "用户名被禁止";
        public const string Phone_Repeat = "手机号已重复";

        //子租户管理员
        public const string Admin = "cc";
        public const string AdminRolesCode = "admin";
        public const string AdminPermissionCode = "*:*:*";

        //租户管理员
        public const string TenantAdmin = "ccadmin";
        public const string TenantAdminPermissionCode = "*";

        public const string DefaultRoleCode = "default";
        public const string CommonRoleName = "common";
    }
}
