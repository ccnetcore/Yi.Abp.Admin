using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Yi.Framework.Stock.Domain.Managers.SemanticKernel.Plugins;

public class NewsPlugins 
{
    private readonly IServiceProvider _serviceProvider;

    public NewsPlugins(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [KernelFunction("save_news"), Description("生成并保存一个新闻")]
    public async Task SaveAsync(NewsModel news)
    {
        var newsManager = _serviceProvider.GetRequiredService<NewsManager>();
        await newsManager.SaveNewsAsync(news);
    }
}

public class NewsModel
{
    [JsonPropertyName("title")]
    [DisplayName("新闻标题")]
    public string Title { get; set; }

    [JsonPropertyName("content")]
    [DisplayName("新闻内容")]
    public string Content { get; set; }
    
    [JsonPropertyName("summary")]
    [DisplayName("新闻简介")]
    public string Summary { get; set; }
    
    [JsonPropertyName("source")]
    [DisplayName("新闻来源")]
    public string Source { get; set; }
}