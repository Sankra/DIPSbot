using System.Diagnostics;
using System.Reflection;
using Hjerpbakk.DIPSBot.MessageHandlers;

namespace Hjerpbakk.DIPSBot.Services
{
    class DebuggingService : IDebuggingService
    {
        readonly string versionAndMode;

        bool debugging;

        public DebuggingService()
        {
            CheckIfDEBUG();
            RunningInDebugMode = debugging;
            versionAndMode = $"{Assembly.GetExecutingAssembly().GetName().Version} {(RunningInDebugMode ? "DEBUG" : "RELEASE")}";
        }

        public bool RunningInDebugMode { get; }

        [Conditional("DEBUG")]
        void CheckIfDEBUG()
        {
            debugging = true;
        }

        public string GetVersionInfo(MessageHandler activeMessageHandler)
            => versionAndMode + $" in {activeMessageHandler?.GetType().Name}";
    }
}
