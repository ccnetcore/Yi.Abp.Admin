using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Yi.Framework.Stock.Domain.Managers.SemanticKernel.Plugins;

public class StockPlugins
{
    private readonly IServiceProvider _serviceProvider;

    public StockPlugins(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    [KernelFunction("save_stocks"), Description("生成并且保存多个股票记录")]
    public async Task SaveAsync(List<StockModel> stockModels)
    {
        var stockMarketManager= _serviceProvider.GetRequiredService<StockMarketManager>();
        await stockMarketManager.SaveStockAsync(stockModels);
    }
}

public class StockModel
{
    [JsonPropertyName("id")]
    [DisplayName("股票id")]
    public Guid Id { get; set; }

    [JsonPropertyName("values")]
    [DisplayName("股票未来24小时价格")]
    public decimal[] Values { get; set; }
}