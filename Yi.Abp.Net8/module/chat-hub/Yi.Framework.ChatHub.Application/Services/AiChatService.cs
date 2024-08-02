using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUglify.Helpers;
using Volo.Abp.Application.Services;
using Yi.Framework.ChatHub.Domain.Managers;
using Yi.Framework.ChatHub.Domain.Shared.Dtos;
using Yi.Framework.ChatHub.Domain.Shared.Model;

namespace Yi.Framework.ChatHub.Application.Services
{
    public class AiChatService : ApplicationService
    {
        private readonly AiManager _aiManager;
        private readonly UserMessageManager _userMessageManager;
        public AiChatService(AiManager aiManager, UserMessageManager userMessageManager) { _aiManager = aiManager; _userMessageManager = userMessageManager; }


        /// <summary>
        /// ai聊天
        /// </summary>
        /// <param name="chatContext"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]

        public async Task ChatAsync([FromBody] List<AiChatContextDto> chatContext)
        {
            const int maxChar = 10;
            var contextId = Guid.NewGuid();
            Queue<string> stringQueue = new Queue<string>();
            await foreach (var aiResult in _aiManager.ChatAsStreamAsync(chatContext))
            {
                stringQueue.Enqueue(aiResult);

                if (stringQueue.Count >= maxChar)
                {
                    StringBuilder currentStr=new StringBuilder();
                    while (stringQueue.Count > 0)
                    {
                        var str = stringQueue.Dequeue();
                        currentStr.Append(str);
                    }
                    await _userMessageManager.SendMessageAsync(MessageContext.CreateAi(currentStr.ToString(), CurrentUser.Id!.Value, contextId));
                }
            }

            StringBuilder currentEndStr = new StringBuilder();
            while (stringQueue.Count > 0)
            {
                var str = stringQueue.Dequeue();
                currentEndStr.Append(str);
            }
            await _userMessageManager.SendMessageAsync(MessageContext.CreateAi(currentEndStr.ToString(), CurrentUser.Id!.Value, contextId));

            //await _userMessageManager.SendMessageAsync(MessageContext.CreateAi(null, CurrentUser.Id!.Value, contextId));
        }
    }
}
