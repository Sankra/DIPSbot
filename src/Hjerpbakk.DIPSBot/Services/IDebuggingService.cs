using Hjerpbakk.DIPSBot.MessageHandlers;

namespace Hjerpbakk.DIPSBot.Services
{
    interface IDebuggingService
    {
        bool RunningInDebugMode { get; }

        string GetVersionInfo(MessageHandler activeMessageHandler);
    }
}
