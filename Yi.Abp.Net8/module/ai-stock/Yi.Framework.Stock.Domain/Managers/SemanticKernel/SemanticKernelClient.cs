using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Volo.Abp.DependencyInjection;

namespace Yi.Framework.Stock.Domain.Managers.SemanticKernel;

public class SemanticKernelClient:ITransientDependency
{
    public Kernel Kernel { get;}

    public SemanticKernelClient(Kernel kernel)
    {
        this.Kernel = kernel;
    }
    /// <summary>
    /// 执行插件
    /// </summary>
    /// <param name="input"></param>
    /// <param name="pluginName"></param>
    /// <param name="functionName"></param>
    /// <returns></returns>
    public async Task<string> InovkerFunctionAsync(string input, string pluginName, string functionName)
    {
        KernelFunction jsonFun = this.Kernel.Plugins.GetFunction(pluginName, functionName);
        var result = await this.Kernel.InvokeAsync(function: jsonFun, new KernelArguments() { ["input"] = input });
        return result.GetValue<string>();
    }

    /// <summary>
    /// 聊天对话,调用方法
    /// </summary>
    /// <returns></returns>
    public async  Task<IReadOnlyList<ChatMessageContent>>  ChatCompletionAsync(string question,params (string,string)[] functions)
    {
        if (functions is null)
        {
            throw new Exception("请选择插件");
        }
        var openSettings = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions.Select(x=>this.Kernel.Plugins.GetFunction(x.Item1, x.Item2)).ToList(),true),
            // ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            MaxTokens =1000
        };

        var chatCompletionService = this.Kernel.GetRequiredService<IChatCompletionService>();

        var results =await chatCompletionService.GetChatMessageContentsAsync(
            question,
            executionSettings: openSettings,
            kernel: Kernel);
        return results;
    }
}