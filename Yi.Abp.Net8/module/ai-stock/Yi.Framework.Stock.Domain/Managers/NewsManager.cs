using Volo.Abp.Domain.Services;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.Stock.Domain.Entities;
using Yi.Framework.Stock.Domain.Managers.SemanticKernel;
using Yi.Framework.Stock.Domain.Managers.SemanticKernel.Plugins;
using System.Text;
using System.IO;

namespace Yi.Framework.Stock.Domain.Managers;

public class NewsManager:DomainService
{
    private SemanticKernelClient _skClient;
    private ISqlSugarRepository<StockNewsAggregateRoot> _newsRepository;
    public NewsManager(SemanticKernelClient skClient,ISqlSugarRepository<StockNewsAggregateRoot> newsRepository)
    {
        _skClient = skClient;
        _newsRepository = newsRepository;
    }

    /// <summary>
    /// 获取最近的新闻
    /// </summary>
    /// <param name="count">获取数量</param>
    /// <returns>最近的新闻列表</returns>
    public async Task<List<StockNewsAggregateRoot>> GetRecentNewsAsync(int count = 10)
    {
        return await _newsRepository._DbQueryable
            .OrderByDescending(n => n.CreationTime)
            .Take(count)
            .Select(n => new StockNewsAggregateRoot 
            { 
                Title = n.Title, 
                Summary = n.Summary,
                Source = n.Source,
                CreationTime = n.CreationTime
            })
            .ToListAsync();
    }

    /// <summary>
    /// 生成一个新闻
    /// </summary>
    /// <returns></returns>
    public async Task GenerateNewsAsync()
    {
        // 获取最近10条新闻
        var recentNews = await GetRecentNewsAsync(10);
        
        // 构建新闻背景上下文
        var newsContext = new StringBuilder();
        if (recentNews.Any())
        {
            newsContext.AppendLine("以下是最近的新闻简介：");
            foreach (var news in recentNews)
            {
                newsContext.AppendLine($"- {news.CreationTime:yyyy-MM-dd} 来源：{news.Source}");
                newsContext.AppendLine($"  标题：{news.Title}");
                newsContext.AppendLine($"  简介：{news.Summary}");
                newsContext.AppendLine();
            }
        }
        else
        {
            newsContext.AppendLine("目前没有最近的新闻记录。");
        }
        
        var promptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "stock", "newsPrompt.txt");
        var question = await File.ReadAllTextAsync(promptPath);
        question = question.Replace("{{newsContext}}", newsContext.ToString());
        
        await _skClient.ChatCompletionAsync(question, ("NewsPlugins","save_news"));
    }

    public async Task SaveNewsAsync(NewsModel news)
    {
        var newsEntity = new StockNewsAggregateRoot(
            title: news.Title,
            content: news.Content,
            source: news.Source
        )
        {
            Summary = news.Summary,
            CreationTime = DateTime.Now,
            IsDeleted = false,
            OrderNum = 0
        };
        
        await _newsRepository.InsertAsync(newsEntity);
    }
}