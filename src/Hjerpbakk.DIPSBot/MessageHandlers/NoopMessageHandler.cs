using LightInject;

namespace Hjerpbakk.DIPSBot.MessageHandlers
{
    class NoopMessageHandler : MessageHandler
    {
        public NoopMessageHandler(IServiceContainer serviceContainer) : base(serviceContainer)
        {
        }
    }
}
