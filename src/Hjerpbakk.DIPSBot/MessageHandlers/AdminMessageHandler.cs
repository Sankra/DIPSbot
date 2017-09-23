using Hjerpbakk.DIPSBot.Actions;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class AdminMessageHandler : MessageHandler
    {
        public AdminMessageHandler(IServiceContainer serviceContainer)
        {
            actions.Add(serviceContainer.GetInstance<AddDevelopersToUtviklingChannelAction>());
            actions.Add(serviceContainer.GetInstance<UserKitchenResponsibleAction>());
            actions.Add(serviceContainer.GetInstance<AddEmployeeAction>());
            actions.Add(serviceContainer.GetInstance<ListCommandsAction>());
        }
    }
}
