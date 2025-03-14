using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Stock.Domain.Entities;
using Yi.Framework.Stock.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Stock.Domain.EventHandlers
{
    /// <summary>
    /// 股票交易事件处理器
    /// </summary>
    public class StockTransactionEventHandler : ILocalEventHandler<StockTransactionEto>, ITransientDependency
    {
        private readonly ISqlSugarRepository<StockTransactionEntity> _transactionRepository;

        public StockTransactionEventHandler(
            ISqlSugarRepository<StockTransactionEntity> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task HandleEventAsync(StockTransactionEto eventData)
        {
            // 创建交易记录实体
            var transaction = new StockTransactionEntity(
                eventData.UserId,
                eventData.StockId,
                eventData.StockCode,
                eventData.StockName,
                eventData.TransactionType,
                eventData.Price,
                eventData.Quantity,
                eventData.Fee
            );

            // 保存交易记录
            await _transactionRepository.InsertAsync(transaction);
        }
    }
} 