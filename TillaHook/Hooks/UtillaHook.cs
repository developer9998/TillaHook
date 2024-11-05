using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TillaHook.Models;
using TillaHook.Tools;

namespace TillaHook.Hooks
{
    internal class UtillaHook : Hook
    {
        public override string Tilla => "Utilla";
        public override string Guid => "org.legoandmars.gorillatag.utilla";

        private PluginInfo pluginInfo;
        private Utilla.GamemodeManager gamemodeManager;

        private RoomArgs room;

        private readonly List<PluginInfoWrapper> gameModesToAdd = [];

        public override void Construct(PluginInfo plugin)
        {
            pluginInfo = plugin;

            // events
            Utilla.Events.GameInitialized += GameInit;
            Utilla.Events.RoomJoined += RoomJoined;
            Utilla.Events.RoomLeft += RoomLeft;

            // modded room actions
            gameModesToAdd.Add(TillaHook.ModdedRoomPlugin);
        }

        public void GameInit(object sender, EventArgs e)
        {
            Logging.Info("Game initialized");

            TillaHook.TriggerGameInit();

            SetGameModeManager();
        }

        public void RoomJoined(object sender, EventArgs e)
        {
            Logging.Info("Room joined");

            ConstructEventArgs(out room);

            TillaHook.TriggerRoomJoin(room);
        }

        public void RoomLeft(object sender, EventArgs e)
        {
            Logging.Info("Room left");

            TillaHook.TriggerRoomLeave(room);
        }

        public override void AddGameMode(GameModeWrapper[] gameModes, Action<string> onGameModeJoin = null, Action<string> onGameModeLeave = null)
        {
            gameModesToAdd.Add(new PluginInfoWrapper()
            {
                Plugin = pluginInfo.Instance,
                GameModes = gameModes,
                OnGameModeJoin = onGameModeJoin,
                OnGameModeLeave = onGameModeLeave
            });

            if (gamemodeManager) AddPluginInfos();
        }

        public async void SetGameModeManager()
        {
            while (gamemodeManager == null)
            {
                gamemodeManager = Utilla.GamemodeManager.Instance;
                await Task.Yield();
            }

            var pluginInfos = AccessTools.Field(gamemodeManager.GetType(), "pluginInfos");
            while (((IList)pluginInfos.GetValue(gamemodeManager)) == null)
            {
                await Task.Yield();
            }

            AddPluginInfos();
        }

        public void AddPluginInfos()
        {
            var pluginInfos = (IList)AccessTools.Field(gamemodeManager.GetType(), "pluginInfos").GetValue(gamemodeManager);

            var defaultGameModes = ((List<Utilla.Models.Gamemode>)AccessTools.Field(gamemodeManager.GetType(), "DefaultModdedGamemodes").GetValue(gamemodeManager)).ToArray();

            foreach (var wrapper in gameModesToAdd)
            {
                try
                {
                    if (wrapper == null)
                    {
                        throw new NullReferenceException("PluginInfoWrapper cannot be null");
                    }

                    bool useDefaultGameModes = (wrapper.GameModes == null || wrapper.GameModes.Length == 0 || wrapper.GameModes.All(gamemode => gamemode == null));
                    Utilla.Models.Gamemode[] gameModeArray = useDefaultGameModes ? null : wrapper.GameModes.Where(gamemode => gamemode != null).Select(gamemode => (Utilla.Models.Gamemode)gamemode).ToArray();

                    if (!useDefaultGameModes)
                    {
                        var uniqueGamemodes = gameModeArray.Where(gamemode => !gamemodeManager.Gamemodes.Contains(gamemode)).ToArray();
                        gamemodeManager.Gamemodes.AddRange(uniqueGamemodes);
                        uniqueGamemodes.ForEach(gamemode => AccessTools.Method(gamemodeManager.GetType(), "AddGamemodeToPrefabPool").Invoke(gamemodeManager, [gamemode]));
                    }

                    var plugin = new Utilla.PluginInfo
                    {
                        Plugin = wrapper.Plugin,
                        Gamemodes = gameModeArray ?? defaultGameModes,
                        OnGamemodeJoin = wrapper.OnGameModeJoin,
                        OnGamemodeLeave = wrapper.OnGameModeLeave
                    };

                    AccessTools.Method(pluginInfos.GetType(), "Add").Invoke(pluginInfos, [plugin]);
                }
                catch(Exception ex)
                {
                    if (wrapper == null || wrapper.Plugin == null) continue;
                    Logging.Error($"Exception thrown when adding gamemode from plugin {wrapper.Plugin.GetType().FullName}: {ex}");
                }
            }

            gameModesToAdd.Clear();
        }
    }
}
