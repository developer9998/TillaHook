# TillaHook
 A library for Gorilla Tag that provides various room related events based on an existing library

## Installation
 Go to the latest release of this mod and download the ``TillaHook.dll`` file provided with it. This file can be placed in your BepInEx's plugins folder.

 This assumes you have BepInEx downloaded and installed, using a mod manager for Gorilla Tag, such as MonkeModManager, can get this process done for you.

## Usage
 With TillaHook installed, it will look out for any Utilla library and rely on it for providing events and adding your own custom game modes.

 TillaHook supports the original Utilla library, in a official stance, the mod is obsolete, but as a contributor of the repository I have continued to repair the mod and enhance it, you can find this fix in the releases for the fork of the repository on my account.

 In additon to this, TillaHook also supports Newtilla, a stable rewrite of Utilla less prone to breaking and is much simpler.

 All supported libraries incorperate the same events and can generally pull off the same behaviour, hence why TillaHook has nearly everything covered for handling the game.

## Development
 With TillaHook, it's a very simple process to subscribe to room related events and to add your own game modes.

### Game Initialization
 This event is called when the game has completed initialization, typically this is done when a behaviour attached to the local player has been awaken.
 ```cs
using BepInEx;
using TillaHook;

public class Mod : BaseUnityPlugin
{
    public void Awake()
    {
        TillaHook.TillaHook.OnGameInitialized += Initialize;
    }

    public void Initialize()
    {
        Debug.Log("Hello, world!");
    }
}
 ```

### Room Connection
 These events are called when the player joins or leaves a room.
  ```cs
using BepInEx;
using TillaHook;

public class Mod : BaseUnityPlugin
{
    public void Awake()
    {
        TillaHook.TillaHook.OnRoomJoined += RoomJoin;
        TillaHook.TillaHook.OnRoomLeft += RoomLeave;
    }

    public void RoomJoin(RoomArgs room)
    {
        Debug.Log($"Joined room {room.RoomName} ({room.PlayerCount} players, {room.GameModeType})");
    }

    public void RoomLeave(RoomArgs room)
    {
        Debug.Log($"Left room {room.RoomName} ({room.PlayerCount} players, {room.GameModeType})");
    }
}
 ```

### Modded Room Connection
 These events are called when the player joins or leaves a room under a modded game mode. This game mode is specified through the single parameter of the event.
   ```cs
using BepInEx;
using TillaHook;

public class Mod : BaseUnityPlugin
{
    public void Awake()
    {
        TillaHook.TillaHook.OnModdedJoin += ModdedJoin;
        TillaHook.TillaHook.OnModdedLeave += ModdedLeave;
    }

    public void ModdedJoin(string gameMode)
    {
        Debug.Log($"Joined modded room ({gameMode})");
    }

    public void ModdedLeave(string gameMode)
    {
        Debug.Log($"Left modded room ({gameMode})");
    }
}
 ```

 ### Custom Game Modes
 TillaHook has a hook class that processes all the events stored in the main class of the mod, which also serves as it's BepInEx plugin. The current hook used by TillaHook is kept in the "Hook" property of the plugin class, and is used to assign a custom game mode.

 When adding a custom game mode, you will be assigned with adding a list of game modes, you can keep this list to one, or however many you need to that will share the same callback for joining and leaving them. With a game mode, you will assign it's display name, ID, and the type of game mode (i.e "Casual", "Infection", "Hunt", "Paintbrawl"), note you may also add a custom type of the game mode manager you want to use in the mode, though this is only supported through Utilla. If you decide to not assign any game modes, the callbacks will act to any modded room joined by the player.

   ```cs
using BepInEx;
using TillaHook;

public class Mod : BaseUnityPlugin
{
    public void Awake()
    {
        TillaHook.TillaHook.Hook.AddGameMode(new GameModeWrapper[1] {
            new GameModeWrapper()
            {
                DisplayName = "Luna's Party",
                ID = "lunaparty",
                GameModeType = "Casual"
            }
        }, RoomJoin, RoomLeave);
    }

    public void RoomJoin(string gameMode)
    {
        Debug.Log("Joined Kaylie's birthday party!");
    }

    public void RoomLeave(string gameMode)
    {
        Debug.Log("Left Kaylie's birthday party :<");
    }

    // I'm writing this as my lovely partner of about a year and a few months celebrates her 17th birthday. I love you so much, Kaylie. You are my everything. I love every thing about you big or small, your attitude, your identity, your vibe, your body, every last bit of it. <3
}
 ```

## Disclaimer
This product is not affiliated with Another Axiom Inc. or its videogames Gorilla Tag and Orion Drift and is not endorsed or otherwise sponsored by Another Axiom. Portions of the materials contained herein are property of Another Axiom. ©2021 Another Axiom Inc.