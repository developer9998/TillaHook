using BepInEx;
using System;
using System.Reflection;
using TillaHook.Models;
using TillaHook.Tools;
using HarmonyLib;

namespace TillaHook.Hooks
{
    internal class NewHook : Hook
    {
        public override string Tilla => "Newtilla";

        public override string Guid => "Lofiat.Newtilla";

        private PluginInfo pluginInfo;

        private RoomArgs room;

        public override void Construct(PluginInfo plugin)
        {
            pluginInfo = plugin;

            var moddedRoomActions = TillaHook.ModdedRoomActions;
            AddEvent("OnJoinModded", moddedRoomActions.InvokeJoin);
            AddEvent("OnLeaveModded", moddedRoomActions.InvokeLeave);

            // events
            GorillaTagger.OnPlayerSpawned(GameInit);
        }

        public void AddEvent(string eventName, Action<string> handler)
        {
            EventInfo eventInfo = pluginInfo.Instance.GetType().GetEvent(eventName, AccessTools.all);
            eventInfo.AddEventHandler(pluginInfo.Instance, handler);
        }

        public override void AddGameMode(GameModeWrapper[] gameModes, Action<string> onGameModeJoin = null, Action<string> onGameModeLeave = null)
        {
            MethodInfo methodInfo = AccessTools.Method(pluginInfo.Instance.GetType(), "AddGameMode", parameters: [typeof(string), typeof(string), typeof(string), typeof(bool), typeof(Action), typeof(Action)]);
            foreach(var gamemode in gameModes)
            {
                methodInfo?.Invoke(null, [gamemode.DisplayName, gamemode.ID, gamemode.GameModeType, true, onGameModeJoin != null ? () => onGameModeJoin(string.Empty) : null, onGameModeLeave != null ? () => onGameModeLeave(string.Empty) : null]);
            }
        }

        public void GameInit()
        {
            Logging.Info("Game initialized");

            TillaHook.OnGameInitialized?.Invoke();

            NetworkSystem.Instance.OnJoinedRoomEvent += RoomJoined;
            NetworkSystem.Instance.OnReturnedToSinglePlayer += RoomLeft;
        }

        public void RoomJoined()
        {
            Logging.Info("Room joined");

            ConstructEventArgs(out room);

            TillaHook.OnRoomJoined?.Invoke(room);
        }

        public void RoomLeft()
        {
            Logging.Info("Room left");

            TillaHook.OnRoomLeft?.Invoke(room);
        }
    }
}
