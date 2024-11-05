using BepInEx;
using System;

namespace TillaHook.Models
{
    public class PluginInfoWrapper
    {
        public BaseUnityPlugin Plugin;
        public GameModeWrapper[] GameModes;
        public Action<string> OnGameModeJoin, OnGameModeLeave;
    }
}
