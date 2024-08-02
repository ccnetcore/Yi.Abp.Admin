using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.ChatHub.Domain.Shared.Dtos
{
    public class AiChatContextDto
    {
        public AnswererTypeEnum AnswererType { get; set; }

        public string Message { get; set; }


        public int Number { get; set; }
    }



    public enum AnswererTypeEnum
    {
        Ai,
        User
    }
}
