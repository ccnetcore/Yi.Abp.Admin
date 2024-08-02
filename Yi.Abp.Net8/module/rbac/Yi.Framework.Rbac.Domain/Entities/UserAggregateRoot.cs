using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;
using Yi.Framework.Core.Helper;
using Yi.Framework.Rbac.Domain.Entities.ValueObjects;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Domain.Entities
{
    /// <summary>
    /// 用户表
    /// </summary>
    [SugarTable("User")]
    [SugarIndex($"index_{nameof(UserName)}", nameof(UserName), OrderByType.Asc)]
    public class UserAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject, IOrderNum, IState
    {
        public UserAggregateRoot()
        {

        }
        public UserAggregateRoot(string userName, string password, long phone, string nick = "萌新")
        {
            UserName = userName;
            EncryPassword.Password = password;
            Phone = phone;
            Nick = nick+"-"+userName;
            BuildPassword();
        }

        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 加密密码
        /// </summary>
        [SugarColumn(IsOwnsOne = true)]
        public EncryPasswordValueObject EncryPassword { get; set; } = new EncryPasswordValueObject();

        ///// <summary>
        ///// 密码
        ///// </summary>
        //public string Password { get; set; } = string.Empty;

        ///// <summary>
        ///// 加密盐值
        ///// </summary>
        //public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// 头像
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string? Nick { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// 地址
        /// </summary>

        public string? Address { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public long? Phone { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string? Introduction { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public SexEnum Sex { get; set; } = SexEnum.Unknown;

        /// <summary>
        /// 部门id
        /// </summary>
        public Guid? DeptId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建者
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;


        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;


        /// <summary>
        /// 角色
        /// </summary>
        [Navigate(typeof(UserRoleEntity), nameof(UserRoleEntity.UserId), nameof(UserRoleEntity.RoleId))]
        public List<RoleAggregateRoot> Roles { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>

        [Navigate(typeof(UserPostEntity), nameof(UserPostEntity.UserId), nameof(UserPostEntity.PostId))]
        public List<PostAggregateRoot> Posts { get; set; }

        /// <summary>
        /// 部门
        /// </summary>

        [Navigate(NavigateType.OneToOne, nameof(DeptId))]
        public DeptAggregateRoot? Dept { get; set; }

        /// <summary>
        /// 构建密码，MD5盐值加密
        /// </summary>
        public UserAggregateRoot BuildPassword(string password = null)
        {
            //如果不传值，那就把自己的password当作传进来的password
            if (password == null)
            {
                if (EncryPassword?.Password == null)
                {
                    throw new ArgumentNullException(nameof(EncryPassword.Password));
                }
                password = EncryPassword.Password;
            }
            EncryPassword.Salt = MD5Helper.GenerateSalt();
            EncryPassword.Password = MD5Helper.SHA2Encode(password, EncryPassword.Salt);
            return this;
        }

        /// <summary>
        /// 判断密码和加密后的密码是否相同
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool JudgePassword(string password)
        {
            if (EncryPassword.Salt is null)
            {
                throw new ArgumentNullException(EncryPassword.Salt);
            }
            var p = MD5Helper.SHA2Encode(password, EncryPassword.Salt);
            if (EncryPassword.Password == MD5Helper.SHA2Encode(password, EncryPassword.Salt))
            {
                return true;
            }
            return false;
        }
    }


}
