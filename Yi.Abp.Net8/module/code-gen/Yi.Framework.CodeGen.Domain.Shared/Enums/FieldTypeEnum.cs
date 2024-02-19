using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.CodeGen.Domain.Shared.Enums
{
    public enum FieldTypeEnum
    {
        [Display(Name = "string", Description = "String")]
        String,

        [Display(Name = "int", Description = "Int32")]
        Int,

        [Display(Name = "long", Description = "Int64")]
        Long,

        [Display(Name = "bool", Description = "Boolean")]
        Bool,

        [Display(Name = "decimal", Description = "Decimal")]
        Decimal,

        [Display(Name = "DateTime", Description = "DateTime")]
        DateTime,

        [Display(Name = "Guid", Description = "Guid")]
        Guid
    }

}
