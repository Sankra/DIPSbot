using System.Diagnostics;

namespace Hjerpbakk.DIPSBot.Services
{
    public class DebuggingService : IDebuggingService
    {
        bool debugging;

        public DebuggingService()
        {
            CheckIfDEBUG();
            RunningInDebugMode = debugging;
        }

        public bool RunningInDebugMode { get; }

        [Conditional("DEBUG")]
        void CheckIfDEBUG()
        {
            debugging = true;
        }
    }
}
