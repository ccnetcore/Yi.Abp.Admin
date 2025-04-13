using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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
        private readonly Kernel _kernel;
        public AiManager(Kernel kernel)
        {
            _kernel = kernel;
        }

        public async IAsyncEnumerable<string?> ChatAsStreamAsync(string model, List<AiChatContextDto> aiChatContextDtos)
        {
            if (aiChatContextDtos.Count == 0)
            {
                yield return null;
            }
            var openSettings = new OpenAIPromptExecutionSettings()
            {
                MaxTokens =1000
            };

            var chatCompletionService = this._kernel.GetRequiredService<IChatCompletionService>(model);

            var  history   =new ChatHistory();
            foreach (var aiChatContextDto in aiChatContextDtos)
            {
                if (aiChatContextDto.AnswererType==AnswererTypeEnum.Ai)
                {
                    history.AddSystemMessage(aiChatContextDto.Message);
                }
                else if(aiChatContextDto.AnswererType==AnswererTypeEnum.User)
                {
                    history.AddUserMessage(aiChatContextDto.Message);
                }
            }
            
            var results = chatCompletionService.GetStreamingChatMessageContentsAsync(
                chatHistory: history,
                executionSettings: openSettings,
                kernel: _kernel);
            if (results is null)
            {
                yield  return null;
            }
          await foreach (var result in results)
          {
              yield return result.Content;
          }
        }
    }
}
