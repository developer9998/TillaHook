using BepInEx;
using System;
using TillaHook.Tools;

namespace TillaHook.Models
{
    public abstract class Hook
    {
        // "tilla" info
        public abstract string Tilla { get; }
        public abstract string Guid { get; }

        // Initialization
        public abstract void Construct(PluginInfo info);

        // Game modes
        public abstract void AddGameMode(GameModeWrapper[] gameModes, Action<string> onGameModeJoin = null, Action<string> onGameModeLeave = null);

        internal void ConstructEventArgs(out RoomArgs room)
        {
            room = new RoomArgs()
            {
                RoomName = NetworkSystem.Instance.RoomName,
                PlayerCount = NetworkSystem.Instance.RoomPlayerCount,
                IsPrivate = NetworkSystem.Instance.SessionIsPrivate,
                GameModeString = NetworkSystem.Instance.GameModeString
            };

            Logging.Info($"Game manager null: {GorillaGameManager.instance == null}");
        }

        public override string ToString() => $"{Tilla} ({Guid})";
    }
}
