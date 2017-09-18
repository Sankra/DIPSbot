using Hjerpbakk.DIPSBot.Actions;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class RegularUserMessageHandler : MessageHandler
    {
		public RegularUserMessageHandler(IServiceContainer serviceContainer)
		{
			actions.Add(serviceContainer.GetInstance<UserKitchenResponsibleAction>());
            actions.Add(serviceContainer.GetInstance<ListCommandsAction>());
		}
    }
}
