using Hjerpbakk.DIPSBot.Actions;
using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class TrondheimMessageHandler : MessageHandler
    {
        public TrondheimMessageHandler(IServiceContainer serviceContainer)
		{
            actions.Add(serviceContainer.GetInstance<TrondheimKitchenResponsibleAction>());
        }
    }
}
