using GorillaGameModes;
using System;

namespace TillaHook.Models
{
    public class GameModeWrapper
    {
        public string DisplayName { get; set; }
        public string ID { get; set; }
        public string GameModeType { get; set; }
        public Type GameManager { get; set; }

        public static implicit operator Utilla.Models.Gamemode(GameModeWrapper nativeGameMode)
        {
            if (nativeGameMode == null) return null;

            if (Enum.TryParse<GameModeType>(nativeGameMode.GameModeType, out var result))
            {
                return new Utilla.Models.Gamemode(nativeGameMode.ID, nativeGameMode.DisplayName, result.ToString());
            }

            if (nativeGameMode.GameModeType != null)
            {
                return new Utilla.Models.Gamemode(nativeGameMode.ID, nativeGameMode.DisplayName, nativeGameMode.GameManager);
            }

            throw new ArgumentException("Utilla.Models.BaseGamemode could not parse from string", "GameMode.GameModeType");
        }
    }
}
