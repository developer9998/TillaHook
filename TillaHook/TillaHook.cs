using BepInEx;
using BepInEx.Bootstrap;
using System.Linq;
using System;
using TillaHook.Models;
using TillaHook.Tools;
using TillaHook.Extensions;

namespace TillaHook
{
    [BepInDependency("Lofiat.Newtilla", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(Constants.Guid, Constants.Name, Constants.Version), BepInDependency("legoandmars.utilla", "1.6.14")]
    public class TillaHook : BaseUnityPlugin
    {
        // instance

        internal static TillaHook Plugin;

        // hooks

        public static Hook Hook;

        // events

        public static event Action<RoomArgs> OnRoomJoined;

        public static event Action<RoomArgs> OnRoomLeft;

        public static event Action OnGameInitialized;

        public static event Action<string> OnModdedJoin, OnModdedLeave;

        // misc

        internal static ModdedRoomActions ModdedRoomActions => new()
        {
            InvokeJoin = (string gamemode) => OnModdedJoin?.SafeInvoke(gamemode),
            InvokeLeave = (string gamemode) => OnModdedLeave?.SafeInvoke(gamemode),
        };

        internal static PluginInfoWrapper ModdedRoomPlugin
        {
            get
            {
                var moddedRoomActions = ModdedRoomActions;

                return new()
                {
                    Plugin = Plugin,
                    GameModes = null,
                    OnGameModeJoin = moddedRoomActions.InvokeJoin,
                    OnGameModeLeave = moddedRoomActions.InvokeLeave
                };
            }
        }

        public void Awake()
        {
            Plugin = this;
            Logging.Logger = Logger;

            // dictionary (guid, plugin)
            var plugins = Chainloader.PluginInfos;

            // initial hook construction
            var hooks = GetType().Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Hook))).Select(Activator.CreateInstance).Select(hook => (Hook)hook).ToList();
            Hook newHook = null, utillaHook = null;

            // plugininfo for our hook
            PluginInfo pluginInfo = null;

            // finding our utilla hook and a new hook ("new" as in, usually a utilla replacement, like "new"tilla, i didn't think abt the "new" part when making the variable haha)
            for(int i = 0; i < hooks.Count; i++)
            {
                var hook = hooks[i];
                Logging.Info(hook.ToString());

                if (hook.Tilla == "Utilla")
                {
                    // assign designated utilla hook
                    utillaHook = hook;
                    Logging.Info("assigned utilla hook");
                    continue;
                }
                
                // again, referencing 'plugins', its a dict where the value is the plugin with the guid of it's key
                // i.e, {"UTILLA GUID", [the utilla plugin]}
                // i actually never knew that before, might come in handy for something like this later on :3
                if (newHook == null && plugins.TryGetValue(hook.Guid, out pluginInfo))
                {
                    // this will be our hook we will be looking after, just know to keep looking for the utilla hook (smth to fall back on) for now
                    newHook = hook;
                    Logging.Info("assigned new hook");
                }
            }

            // use a new hook, if found, or our utilla hook
            Hook = newHook ?? utillaHook;
            // our plugininfo will only be null when we need to again, fall back on our utilla hook
            pluginInfo ??= plugins[Hook.Guid];

            // instanced hook construction
            Logging.Info($"Constructing hook {Hook}");
            Hook.Construct(pluginInfo);
        }

        internal static void TriggerGameInit() => OnGameInitialized?.SafeInvoke();

        internal static void TriggerRoomJoin(RoomArgs args) => OnRoomJoined?.SafeInvoke(args);

        internal static void TriggerRoomLeave(RoomArgs args) => OnRoomLeft?.SafeInvoke(args);
    }
}
