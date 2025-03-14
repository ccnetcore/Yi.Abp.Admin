using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Options;
// using OpenAI;
// using OpenAI.Managers;
// using OpenAI.ObjectModels;
// using OpenAI.ObjectModels.RequestModels;
// using OpenAI.ObjectModels.ResponseModels;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Yi.Framework.ChatHub.Domain.Shared.Dtos;
using Yi.Framework.ChatHub.Domain.Shared.Options;
namespace Yi.Framework.ChatHub.Domain.Managers
{
    public class AiManager : ISingletonDependency
    {
        public AiManager(IOptions<AiOptions> options)
        {
            // this.OpenAIService = new OpenAIService(new OpenAiOptions()
            // {
            //     ApiKey = options.Value.ApiKey,
            //     BaseDomain = options.Value.BaseDomain
            // });
        }
        // private OpenAIService OpenAIService { get; }

        public async IAsyncEnumerable<string> ChatAsStreamAsync(string model, List<AiChatContextDto> aiChatContextDtos)
        {
            throw new NotImplementedException("准备sk重构");
            yield break;
            // if (aiChatContextDtos.Count == 0)
            // {
            //     yield return null;
            // }
            //
            // List<ChatMessage> messages = aiChatContextDtos.Select(x =>
            // {
            //     if (x.AnswererType == AnswererTypeEnum.Ai)
            //     {
            //         return ChatMessage.FromSystem(x.Message);
            //     }
            //     else
            //     {
            //         return ChatMessage.FromUser(x.Message);
            //     }
            // }).ToList();
            // var completionResult = OpenAIService.ChatCompletion.CreateCompletionAsStream(new ChatCompletionCreateRequest
            // {
            //     Messages = messages,
            //     Model =model 
            // });
            //
            // HttpStatusCode? error = null;
            // await foreach (var result in completionResult)
            // {
            //     if (result.Successful)
            //     {
            //         yield return result.Choices.FirstOrDefault()?.Message.Content ?? null;
            //     }
            //     else
            //     {
            //         error = result.HttpStatusCode;
            //         break;
            //     }
            //
            // }
            // if (error == HttpStatusCode.PaymentRequired)
            // {
            //     yield return "余额不足,请联系站长充值";
            //  
            // }
        }
    }
}
