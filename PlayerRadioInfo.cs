using AnonymusRadio;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AnonymusRadio.PlayerInfos
{
    public static class PlayerRadioInfo
    {

        public static Dictionary<int, string> originalName = new Dictionary<int, string>();

        public static HashSet<int> playersUsingRadio = new HashSet<int>();

        public static void GetAndSaveInfos(Player player)
        {
            if (!playersUsingRadio.Contains(player.PlayerId))
            {
                playersUsingRadio.Add(player.PlayerId);

                if (!originalName.ContainsKey(player.PlayerId))
                {
                    originalName[player.PlayerId] = player.DisplayName;
                    Logger.Info($"Saved original name for player {player.PlayerId}: {player.DisplayName}");
                }

                player.DisplayName = Plugin.Instance.Config.displayNameToChangeTo;
                Logger.Info($"Player {player.PlayerId} started using radio - changed name to: {player.DisplayName}");

                player.SendHint("Using radio!", 1);

            }
        }

        public static void SetDeafultInfos(Player player)
        {
            if (playersUsingRadio.Contains(player.PlayerId))
            {
                playersUsingRadio.Remove(player.PlayerId);

                if (originalName.TryGetValue(player.PlayerId, out string originalDisplayName))
                {
                    player.DisplayName = originalDisplayName;

                    Logger.Info($"Player {player.PlayerId} stopped using radio - restored name to: {player.DisplayName}");

                    player.SendHint("Stopped using radio!", 1);
                }
                else
                {
                    Logger.Warn($"Could not restore name for player {player.PlayerId} - original name not found!");
                }

            }

        }
    }
}
