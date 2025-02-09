using Mapster;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.DigitalCollectibles.Domain.Entities.Record;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Domain.EventHandlers;

public class SuccessMarketEventHandler : ILocalEventHandler<SuccessMarketEto>, ITransientDependency
{
    private readonly ISqlSugarRepository<MarketRecordAggregateRoot> _marketRecordRepository;

    public SuccessMarketEventHandler(ISqlSugarRepository<MarketRecordAggregateRoot> marketRecordRepository)
    {
        _marketRecordRepository = marketRecordRepository;
    }

    public async Task HandleEventAsync(SuccessMarketEto eventData)
    {
        await _marketRecordRepository.InsertAsync(eventData.Adapt<MarketRecordAggregateRoot>());
    }
}