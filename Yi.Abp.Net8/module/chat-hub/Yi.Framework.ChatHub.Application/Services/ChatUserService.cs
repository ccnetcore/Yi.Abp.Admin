using Mapster;
using Volo.Abp.Application.Services;
using Volo.Abp.EventBus.Local;
using Yi.Framework.ChatHub.Domain.Managers;
using Yi.Framework.ChatHub.Domain.Shared.Model;
using Yi.Framework.Rbac.Domain.Shared.Etos;

namespace Yi.Framework.ChatHub.Application.Services
{
    public class ChatUserService : ApplicationService
    {
        private UserMessageManager _messageManager;
        private ILocalEventBus _localEventBus;
        public ChatUserService(UserMessageManager messageManager, ILocalEventBus localEventBus)
        { _messageManager = messageManager; _localEventBus = localEventBus; }

        public async Task<List<ChatUserModel>> GetListAsync()
        {
            //映射用户信息
            var userList = await _messageManager.GetAllUserAsync();

            var userIds = userList.Select(x => x.UserId).Distinct().ToList();
            UserRoleMenuQueryEventArgs userRoleMenuQuery = new UserRoleMenuQueryEventArgs(userIds.ToArray());

            //调用用户领域事件，获取用户信息，第一个发送者用户信息，第二个为接收者用户信息
            await _localEventBus.PublishAsync(userRoleMenuQuery, false);

            var userInfoDic = userRoleMenuQuery.Result.ToDictionary(x => x.User.Id);
            var output = userList.Adapt<List<ChatUserModel>>();

            foreach (var chatUser in output)
            {
                var currentUserInfo = userInfoDic[chatUser.UserId];
                chatUser.UserName= currentUserInfo.User.UserName;
                chatUser.UserIcon = currentUserInfo.User.Icon;
            }
            return output;
        }
    }
}
