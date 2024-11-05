using BepInEx.Logging;

namespace TillaHook.Tools
{
    internal class Logging
    {
        internal static ManualLogSource Logger;

        public static void Info(object data) => Log(LogLevel.Info, data);

        public static void Warn(object data) => Log(LogLevel.Warning, data);

        public static void Error(object data) => Log(LogLevel.Error, data);

        private static void Log(LogLevel level, object data)
        {
#if DEBUG
            Logger?.Log(level, data);
#endif
        }
    }
}
