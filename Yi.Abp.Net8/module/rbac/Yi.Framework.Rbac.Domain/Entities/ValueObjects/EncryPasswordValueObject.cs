using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Values;

namespace Yi.Framework.Rbac.Domain.Entities.ValueObjects
{
    public class EncryPasswordValueObject : ValueObject
    {
        public EncryPasswordValueObject() { }
        public EncryPasswordValueObject(string password) { this.Password = password; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 加密盐值
        /// </summary>
        public string Salt { get; set; } = string.Empty;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Password;
            yield return Salt;
        }
    }
}
