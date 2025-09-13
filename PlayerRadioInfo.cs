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
                }

                player.DisplayName = Plugin.Instance.Config.displayNameToChangeTo;

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
                }

            }
        }
    }
}
