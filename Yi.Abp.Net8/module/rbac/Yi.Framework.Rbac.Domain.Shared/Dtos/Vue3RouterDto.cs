using System.Collections.Generic;
using static Yi.Framework.Core.Helper.TreeHelper;

namespace Yi.Framework.Rbac.Domain.Shared.Dtos
{
    public class Vue3RouterDto : ITreeModel<Vue3RouterDto>
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public int OrderNum { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public bool Hidden { get; set; }
        public string Redirect { get; set; } = string.Empty;
        public string Component { get; set; } = string.Empty;
        public bool AlwaysShow { get; set; }
        public Meta Meta { get; set; } = new Meta();
        public List<Vue3RouterDto> Children { get; set; }
    }


    public class Meta
    {
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool NoCache { get; set; }
        public string link { get; set; } = string.Empty;
    }
}
