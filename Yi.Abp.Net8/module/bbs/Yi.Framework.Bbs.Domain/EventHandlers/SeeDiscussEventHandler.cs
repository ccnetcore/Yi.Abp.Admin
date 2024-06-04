using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Shared.Etos;

namespace Yi.Framework.Bbs.Domain.EventHandlers
{
    public class SeeDiscussEventHandler : ILocalEventHandler<SeeDiscussEventArgs>, ITransientDependency
    {
        private IRepository<DiscussAggregateRoot, Guid> _repository;
        public SeeDiscussEventHandler(IRepository<DiscussAggregateRoot, Guid> repository)
        {
            _repository = repository;
        }
        public async Task HandleEventAsync(SeeDiscussEventArgs eventData)
        {
            var entity = await _repository.GetAsync(eventData.DiscussId);
            if (entity is not null)
            {
                entity.SeeNum += 1;
                await _repository.UpdateAsync(entity);
            }
        }




    }
}
