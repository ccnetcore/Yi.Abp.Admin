using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Yi.Framework.Rbac.Domain.Entities;

namespace Yi.Framework.Rbac.Domain.EventHandlers
{
    public class StudentEventHandler : ILocalEventHandler<EntityCreatedEventData<StudentEntity>>, ITransientDependency
    {
        public Task HandleEventAsync(EntityCreatedEventData<StudentEntity> eventData)
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(eventData.Entity));
            return Task.CompletedTask;
        }
    }
}
