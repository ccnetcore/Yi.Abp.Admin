using Yi.Framework.Ddd;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Config
{
    /// <summary>
    /// ≈‰÷√≤È—Ø≤Œ ˝
    /// </summary>
    public class ConfigGetListInputVo : PagedAllResultRequestDto
    {
        /// <summary>
        /// ≈‰÷√√˚≥∆
        /// </summary>
        public string? ConfigName { get; set; }

        /// <summary>
        /// ≈‰÷√º¸
        /// </summary>
        public string? ConfigKey { get; set; }

    }
}
