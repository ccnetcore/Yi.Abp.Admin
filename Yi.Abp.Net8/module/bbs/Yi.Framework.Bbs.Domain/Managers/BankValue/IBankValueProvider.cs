using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Domain.Managers.BankValue
{
    public interface IBankValueProvider
    {
        public Task<decimal> GetValueAsync();
    }
}
