using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Domain.Shared.Etos
{
    public class SeeDiscussEventArgs
    {
        public Guid DiscussId { get; set; }
        public int OldSeeNum { get; set; }
    }
}
